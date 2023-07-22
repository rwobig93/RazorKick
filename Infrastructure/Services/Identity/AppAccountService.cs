﻿using System.Globalization;
using System.Security.Claims;
using Application.Constants.Communication;
using Application.Constants.Identity;
using Application.Constants.Web;
using Application.Helpers.Auth;
using Application.Helpers.Communication;
using Application.Helpers.Identity;
using Application.Helpers.Web;
using Application.Mappers.Identity;
using Application.Models.Identity.User;
using Application.Models.Identity.UserExtensions;
using Application.Models.Lifecycle;
using Application.Models.Web;
using Application.Repositories.Identity;
using Application.Requests.Api;
using Application.Requests.Identity.User;
using Application.Responses.Api;
using Application.Responses.Identity;
using Application.Services.Identity;
using Application.Services.Lifecycle;
using Application.Services.System;
using Application.Settings.AppSettings;
using Blazored.LocalStorage;
using Domain.DatabaseEntities.Identity;
using Domain.Enums.Auth;
using Domain.Enums.Database;
using Domain.Enums.Identity;
using Domain.Models.Database;
using Domain.Models.Identity;
using FluentEmail.Core;
using Infrastructure.Services.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using IResult = Application.Models.Web.IResult;

namespace Infrastructure.Services.Identity;

public class AppAccountService : IAppAccountService 
{
    private readonly IAppUserRepository _userRepository;
    private readonly IFluentEmail _mailService;
    private readonly IAppRoleRepository _roleRepository;
    private readonly IAppPermissionRepository _appPermissionRepository;
    private readonly AppConfiguration _appConfig;
    private readonly ILocalStorageService _localStorage;
    private readonly AuthStateProvider _authProvider;
    private readonly HttpClient _httpClient;
    private readonly IDateTimeService _dateTime;
    private readonly IRunningServerState _serverState;
    private readonly ISerializerService _serializer;
    private readonly IAuditTrailService _auditService;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly SecurityConfiguration _securityConfig;
    private readonly ICurrentUserService _currentUserService;
    private readonly LifecycleConfiguration _lifecycleConfig;

    public AppAccountService(IOptions<AppConfiguration> appConfig, IAppPermissionRepository appPermissionRepository, IAppRoleRepository 
    roleRepository,
        IAppUserRepository userRepository, ILocalStorageService localStorage, AuthStateProvider authProvider, IHttpClientFactory httpClientFactory,
        IFluentEmail mailService, IDateTimeService dateTime, IRunningServerState serverState, ISerializerService serializer,
        IAuditTrailService auditService, IHttpContextAccessor contextAccessor, IOptions<SecurityConfiguration> securityConfig,
        ICurrentUserService currentUserService, IOptions<LifecycleConfiguration> lifecycleConfig)
    {
        _appConfig = appConfig.Value;
        _appPermissionRepository = appPermissionRepository;
        _roleRepository = roleRepository;
        _userRepository = userRepository;
        _localStorage = localStorage;
        _authProvider = authProvider;
        _httpClient = httpClientFactory.CreateClient("Default");
        _mailService = mailService;
        _dateTime = dateTime;
        _serverState = serverState;
        _serializer = serializer;
        _auditService = auditService;
        _contextAccessor = contextAccessor;
        _currentUserService = currentUserService;
        _lifecycleConfig = lifecycleConfig.Value;
        _securityConfig = securityConfig.Value;
    }

    private static async Task<IResult<UserLoginResponse>> VerifyAccountIsLoginReady(AppUserSecurityDb userSecurity)
    {
        // Email isn't confirmed so we don't allow login
        if (!userSecurity.EmailConfirmed)
            return await Result<UserLoginResponse>.FailAsync(ErrorMessageConstants.EmailNotConfirmedError);

        return userSecurity.AuthState switch
        {
            AuthState.Disabled => await Result<UserLoginResponse>.FailAsync(ErrorMessageConstants.AccountDisabledError),
            AuthState.LockedOut => await Result<UserLoginResponse>.FailAsync(ErrorMessageConstants.AccountLockedOutError),
            _ => await Result<UserLoginResponse>.SuccessAsync()
        };
    }

    private async Task<IResult<UserLoginResponse>> VerifyPasswordIsCorrect(AppUserSecurityDb userSecurity, string password)
    {
        var passwordValid = await IsPasswordCorrect(userSecurity.Id, password);
        
        // Password is valid, return success
        if (passwordValid.Data) return await Result<UserLoginResponse>.SuccessAsync();
        
        // Password provided is invalid, handle bad password
        userSecurity.BadPasswordAttempts += 1;
        userSecurity.LastBadPassword = _dateTime.NowDatabaseTime;
        await _userRepository.UpdateSecurityAsync(userSecurity.ToSecurityUpdate());
            
        // Account isn't locked out yet but a bad password was entered
        if (userSecurity.BadPasswordAttempts < _securityConfig.MaxBadPasswordAttempts)
            return await Result<UserLoginResponse>.FailAsync(ErrorMessageConstants.CredentialsInvalidError);
            
        // Account is now locked out due to bad password attempts
        userSecurity.AuthState = AuthState.LockedOut;
        userSecurity.AuthStateTimestamp = _dateTime.NowDatabaseTime;
        await _userRepository.UpdateSecurityAsync(userSecurity.ToSecurityUpdate());
        return await Result<UserLoginResponse>.FailAsync(ErrorMessageConstants.AccountLockedOutError);
    }

    public async Task<IResult<UserLoginResponse>> LoginAsync(UserLoginRequest loginRequest)
    {
        var userSecurity = (await _userRepository.GetByUsernameSecurityAsync(loginRequest.Username)).Result;
        if (userSecurity is null)
            return await Result<UserLoginResponse>.FailAsync(ErrorMessageConstants.CredentialsInvalidError);

        var accountIsLoginReady = await VerifyAccountIsLoginReady(userSecurity);
        if (!accountIsLoginReady.Succeeded)
            return accountIsLoginReady;

        var passwordIsValid = await VerifyPasswordIsCorrect(userSecurity, loginRequest.Password);
        if (!passwordIsValid.Succeeded)
            return passwordIsValid;
        
        // Entered password is correct so we reset previous bad password attempts and indicate full login timestamp
        userSecurity.BadPasswordAttempts = 0;
        userSecurity.LastFullLogin = _dateTime.NowDatabaseTime;
        var updateSecurity = await _userRepository.UpdateSecurityAsync(userSecurity.ToSecurityUpdate());
        if (!updateSecurity.Success)
            return await Result<UserLoginResponse>.FailAsync(updateSecurity.ErrorMessage);
        
        // Generate and register client id as a successful login with user+pass, only registered client id's can re-auth w/ refresh tokens
        var clientId = AccountHelpers.GenerateClientId();
        var newExtendedAttribute = new AppUserExtendedAttributeCreate
        {
            OwnerId = userSecurity.Id,
            Name = "FullLoginClientId",
            Type = ExtendedAttributeType.UserClientId,
            Value = clientId,
            Description = ""
        };
        var addAttributeRequest = await _userRepository.AddExtendedAttributeAsync(newExtendedAttribute);
        if (!addAttributeRequest.Success)
            return await Result<UserLoginResponse>.FailAsync(addAttributeRequest.ErrorMessage);

        // Generate the JWT and return
        var token = await GenerateJwtAsync(userSecurity.ToUserDb());
        var refreshToken = JwtHelpers.GenerateUserJwtRefreshToken(_dateTime, _securityConfig, _appConfig, userSecurity.Id);
        var refreshTokenExpiration = JwtHelpers.GetJwtExpirationTime(refreshToken);
        var response = new UserLoginResponse() { ClientId = clientId, Token = token, RefreshToken = refreshToken,
            RefreshTokenExpiryTime = refreshTokenExpiration };

        // Create audit log for login if configured
        if (_lifecycleConfig.AuditLoginLogout)
        {
            await _auditService.CreateAsync(new AuditTrailCreate
            {
                TableName = "AuthState",
                RecordId = userSecurity.Id,
                ChangedBy = _serverState.SystemUserId,
                Action = DatabaseActionType.Login,
                Before = _serializer.Serialize(new Dictionary<string, string>() {{"Username", ""}, {"AuthState", ""}}),
                After = _serializer.Serialize(new Dictionary<string, string>() 
                    {{"Username", userSecurity.Username}, {"AuthState", userSecurity.AuthState.ToString()}})
            });
        }
        
        return await Result<UserLoginResponse>.SuccessAsync(response);
    }

    public async Task<IResult<UserLoginResponse>> LoginGuiAsync(UserLoginRequest loginRequest)
    {
        try
        {
            var loginResponse = await LoginAsync(loginRequest);
            if (!loginResponse.Succeeded)
                return loginResponse;
            
            var result = await CacheAuthTokens(loginResponse);
            if (!result.Succeeded)
                return await Result<UserLoginResponse>.FailAsync(result.Messages.FirstOrDefault()!);

            return await Result<UserLoginResponse>.SuccessAsync(loginResponse.Data);
        }
        catch (Exception ex)
        {
            return await Result<UserLoginResponse>.FailAsync($"Failure occurred attempting to login: {ex.Message}");
        }
    }

    public async Task<IResult<ApiTokenResponse>> GetApiAuthToken(ApiGetTokenRequest tokenRequest)
    {
        var userSecurity = (await _userRepository.GetByUsernameSecurityAsync(tokenRequest.Username)).Result;
        if (userSecurity is null)
            return await Result<ApiTokenResponse>.FailAsync(ErrorMessageConstants.CredentialsInvalidError);
        
        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
        switch (userSecurity.AuthState)
        {
            case AuthState.Disabled:
                return await Result<ApiTokenResponse>.FailAsync(ErrorMessageConstants.AccountDisabledError);
            case AuthState.LockedOut:
                return await Result<ApiTokenResponse>.FailAsync(ErrorMessageConstants.AccountLockedOutError);
        }
        
        // TODO: Add a way to generate service accounts
        return userSecurity.AccountType switch
        {
            AccountType.User => await HandleApiAuthUserAccount(tokenRequest, userSecurity),
            AccountType.Service => await HandleApiAuthServiceAccount(tokenRequest, userSecurity),
            _ => await Result<ApiTokenResponse>.FailAsync(ErrorMessageConstants.CredentialsInvalidError)
        };
    }

    private async Task<IResult<ApiTokenResponse>> HandleApiAuthUserAccount(ApiGetTokenRequest tokenRequest, AppUserSecurityDb userSecurity)
    {
        var userApiTokensRequest = await _userRepository.GetUserExtendedAttributesByTypeAndValueAsync(
            userSecurity.Id, ExtendedAttributeType.UserApiToken, tokenRequest.Password);
        if (!userApiTokensRequest.Success || userApiTokensRequest.Result is null || !userApiTokensRequest.Result.Any())
        {
            userSecurity.BadPasswordAttempts += 1;
            userSecurity.LastBadPassword = _dateTime.NowDatabaseTime;
            await _userRepository.UpdateSecurityAsync(userSecurity.ToSecurityUpdate());

            // Account isn't locked out yet but a bad api token was provided
            if (userSecurity.BadPasswordAttempts < _securityConfig.MaxBadPasswordAttempts)
                return await Result<ApiTokenResponse>.FailAsync(ErrorMessageConstants.CredentialsInvalidError);

            // Account is now locked out due to bad api tokens being provided
            userSecurity.AuthState = AuthState.LockedOut;
            userSecurity.AuthStateTimestamp = _dateTime.NowDatabaseTime;
            await _userRepository.UpdateSecurityAsync(userSecurity.ToSecurityUpdate());
            return await Result<ApiTokenResponse>.FailAsync(ErrorMessageConstants.AccountLockedOutError);
        }

        // Provided api token is correct so we reset previous bad password attempts
        userSecurity.BadPasswordAttempts = 0;

        var updateSecurity = await _userRepository.UpdateSecurityAsync(userSecurity.ToSecurityUpdate());
        if (!updateSecurity.Success)
            return await Result<ApiTokenResponse>.FailAsync(updateSecurity.ErrorMessage);

        var token = await GenerateJwtAsync(userSecurity.ToUserDb(), true);
        var response = new ApiTokenResponse() {Token = token, TokenExpiration = JwtHelpers.GetJwtExpirationTime(token)};

        return await Result<ApiTokenResponse>.SuccessAsync(response);
    }

    private async Task<IResult<ApiTokenResponse>> HandleApiAuthServiceAccount(ApiGetTokenRequest tokenRequest, AppUserSecurityDb userSecurity)
    {
        var passwordValid = await IsPasswordCorrect(userSecurity.Id, tokenRequest.Password);
        if (!passwordValid.Data)
        {
            userSecurity.BadPasswordAttempts += 1;
            userSecurity.LastBadPassword = _dateTime.NowDatabaseTime;
            await _userRepository.UpdateSecurityAsync(userSecurity.ToSecurityUpdate());

            // Account isn't locked out yet but a bad password was entered
            if (userSecurity.BadPasswordAttempts < _securityConfig.MaxBadPasswordAttempts)
                return await Result<ApiTokenResponse>.FailAsync(ErrorMessageConstants.CredentialsInvalidError);

            // Account is now locked out due to bad password attempts
            userSecurity.AuthState = AuthState.LockedOut;
            userSecurity.AuthStateTimestamp = _dateTime.NowDatabaseTime;
            await _userRepository.UpdateSecurityAsync(userSecurity.ToSecurityUpdate());
            return await Result<ApiTokenResponse>.FailAsync(ErrorMessageConstants.AccountLockedOutError);
        }

        // Entered password is correct so we reset previous bad password attempts
        userSecurity.BadPasswordAttempts = 0;

        var updateSecurity = await _userRepository.UpdateSecurityAsync(userSecurity.ToSecurityUpdate());
        if (!updateSecurity.Success)
            return await Result<ApiTokenResponse>.FailAsync(updateSecurity.ErrorMessage);

        var token = await GenerateJwtAsync(userSecurity.ToUserDb(), true);
        var response = new ApiTokenResponse() {Token = token, TokenExpiration = JwtHelpers.GetJwtExpirationTime(token)};

        return await Result<ApiTokenResponse>.SuccessAsync(response);
    }

    public async Task<IResult> CacheAuthTokens(IResult<UserLoginResponse> loginResponse)
    {
        try
        {
            await _localStorage.SetItemAsync(LocalStorageConstants.ClientId, loginResponse.Data.ClientId);
            await _localStorage.SetItemAsync(LocalStorageConstants.AuthToken, loginResponse.Data.Token);
            await _localStorage.SetItemAsync(LocalStorageConstants.AuthTokenRefresh, loginResponse.Data.RefreshToken);

            await _authProvider.GetAuthenticationStateAsync(loginResponse.Data.Token);
            return await Result.SuccessAsync();
        }
        catch (Exception ex)
        {
            return await Result.FailAsync(ex.Message);
        }
    }

    public async Task<IResult> LogoutGuiAsync(Guid userId)
    {
        try
        {
            // Remove client id and tokens from local client storage and deauthenticate
            await _localStorage.RemoveItemAsync(LocalStorageConstants.ClientId);
            await _localStorage.RemoveItemAsync(LocalStorageConstants.AuthToken);
            await _localStorage.RemoveItemAsync(LocalStorageConstants.AuthTokenRefresh);
            _authProvider.DeauthenticateUser();
            _httpClient.DefaultRequestHeaders.Authorization = null;
            
            if (!_lifecycleConfig.AuditLoginLogout) return await Result.SuccessAsync();
            
            // Create audit log for logout if configured
            if (userId == Guid.Empty)
                userId = await _currentUserService.GetCurrentUserId() ?? Guid.Empty;
                
            var user = (await _userRepository.GetByIdAsync(userId)).Result ??
                       new AppUserSecurityDb() {Username = "Unknown", Email = "User@Unknown.void"};

            await _auditService.CreateAsync(new AuditTrailCreate
            {
                TableName = "AuthState",
                RecordId = userId,
                ChangedBy = _serverState.SystemUserId,
                Action = DatabaseActionType.Logout,
                Before = _serializer.Serialize(new Dictionary<string, string>()
                    {{"Username", user.Username}, {"AuthState", user.AuthState.ToString()}}),
                After = _serializer.Serialize(new Dictionary<string, string>() {{"Username", ""}, {"AuthState", ""}})
            });

            return await Result.SuccessAsync();
        }
        catch
        {
            return await Result.FailAsync();
        }
    }

    public async Task<IResult<bool>> PasswordMeetsRequirements(string password)
    {
        try
        {
            return await Result<bool>.SuccessAsync(AccountHelpers.DoesPasswordMeetRequirements(password));
        }
        catch (Exception ex)
        {
            return await Result<bool>.FailAsync(ex.Message);
        }
    }

    private async Task<DatabaseActionResult<Guid>> CreateAsync(AppUserDb user, string password)
    {
        var createUser = user.ToCreateObject();
        var passwordMeetsRequirements = (await PasswordMeetsRequirements(password)).Data;
        if (!passwordMeetsRequirements)
        {
            var passwordFailResult = new DatabaseActionResult<Guid>();
            passwordFailResult.Fail("Provided password doesn't meet the requirements");
            return passwordFailResult;
        }

        AccountHelpers.GenerateHashAndSalt(password, _securityConfig.PasswordPepper, out var salt, out var hash);
        createUser.CreatedOn = _dateTime.NowDatabaseTime;
        createUser.CreatedBy = _serverState.SystemUserId;

        var userSecurity = new AppUserSecurityAttributeCreate
        {
            PasswordSalt = salt,
            PasswordHash = hash
        };
        await _userRepository.CreateSecurityAsync(userSecurity);

        return await _userRepository.CreateAsync(createUser);
    }

    public async Task<IResult> RegisterAsync(UserRegisterRequest registerRequest)
    {
        if (!AccountHelpers.IsValidEmailAddress(registerRequest.Email))
            return await Result.FailAsync($"The email address {registerRequest.Email} provided isn't a valid email, please try again");
        
        var matchingEmail = (await _userRepository.GetByEmailAsync(registerRequest.Email)).Result;
        if (matchingEmail is not null)
            return await Result.FailAsync(
                $"The email address {registerRequest.Email} is already in use, are you sure you don't have an account already?");
        
        var matchingUserName = (await _userRepository.GetByUsernameAsync(registerRequest.Username)).Result;
        if (matchingUserName != null)
        {
            return await Result.FailAsync(string.Format($"Username {registerRequest.Username} is already in use, please try again"));
        }
        
        var newUser = new AppUserDb()
        {
            Email = registerRequest.Email,
            Username = registerRequest.Username,
        };
        
        var createUserResult = await CreateAsync(newUser, registerRequest.Password);
        if (!createUserResult.Success)
            return await Result.FailAsync(createUserResult.ErrorMessage);

        var caveatMessage = "";
        await _userRepository.GetByUsernameAsync(UserConstants.DefaultUsers.SystemUsername);
        var defaultRole = (await _roleRepository.GetByNameAsync(RoleConstants.DefaultRoles.DefaultName)).Result;
        var addToRoleResult = await _roleRepository.AddUserToRoleAsync(createUserResult.Result, defaultRole!.Id);
        if (!addToRoleResult.Success)
            caveatMessage = $",{Environment.NewLine} Default permissions could not be added to this account, " +
                            $"please contact the administrator for assistance";

        var confirmationUrl = (await GetEmailConfirmationUrl(createUserResult.Result)).Data;
        if (string.IsNullOrWhiteSpace(confirmationUrl))
            return await Result.FailAsync("Failure occurred generating confirmation URL, please contact the administrator");
        
        // TODO: Look into why calling this as a BackgroundJob.Enqueue(() => send) fails
        var response = await _mailService.SendRegistrationEmail(newUser.Email, newUser.Username, confirmationUrl);

        if (!response.Successful)
            return await Result.FailAsync(
                $"Account was registered successfully but a failure occurred attempting to send an email to " +
                $"the address provided, please contact the administrator for assistance{caveatMessage}");
        
        return await Result<Guid>.SuccessAsync(newUser.Id, 
            $"Account {newUser.Username} successfully registered, please check your email to confirm!{caveatMessage}");
    }

    public async Task<IResult<string>> GetEmailConfirmationUrl(Guid userId)
    {
        var previousConfirmations =
            (await _userRepository.GetUserExtendedAttributesByTypeAsync(userId, ExtendedAttributeType.EmailConfirmationToken)).Result;
        var previousConfirmation = previousConfirmations?.FirstOrDefault();
        
        var endpointUri = new Uri(string.Concat(_appConfig.BaseUrl, AppRouteConstants.Identity.ConfirmEmail));
        var confirmationUri = QueryHelpers.AddQueryString(endpointUri.ToString(), "userId", userId.ToString());
        
        // Previous pending account registration exists, return current value
        if (previousConfirmation is not null)
            return await Result<string>.SuccessAsync(QueryHelpers.AddQueryString(
                confirmationUri, "confirmationCode", previousConfirmation.Value));

        // No currently pending account registration exists so we'll generate a new one, add it to the provided user
        //   and return the generated confirmation uri
        var confirmationCode = UrlHelpers.GenerateToken();
        var newExtendedAttribute = new AppUserExtendedAttributeCreate
        {
            OwnerId = userId,
            Name = "AccountEmailConfirmation",
            Type = ExtendedAttributeType.EmailConfirmationToken,
            Value = confirmationCode,
            Description = ""
        };
        var addAttributeRequest = await _userRepository.AddExtendedAttributeAsync(newExtendedAttribute);
        if (!addAttributeRequest.Success)
            return await Result<string>.FailAsync(addAttributeRequest.ErrorMessage);

        return await Result<string>.SuccessAsync(QueryHelpers.AddQueryString(confirmationUri, "confirmationCode", confirmationCode));
    }

    public async Task<IResult<string>> ConfirmEmailAsync(Guid userId, string confirmationCode)
    {
        var userSecurity = (await _userRepository.GetByIdSecurityAsync(userId)).Result;
        if (userSecurity is null)
            return await Result<string>.FailAsync(ErrorMessageConstants.UserNotFoundError);

        var previousConfirmations =
            (await _userRepository.GetUserExtendedAttributesByTypeAsync(userSecurity.Id, ExtendedAttributeType.EmailConfirmationToken)).Result;
        var previousConfirmation = previousConfirmations?.FirstOrDefault();

        if (previousConfirmation is null)
        {
            await _auditService.CreateAsync(new AuditTrailCreate
            {
                TableName = "EmailConfirmation",
                RecordId = userId,
                ChangedBy = _serverState.SystemUserId,
                Timestamp = _dateTime.NowDatabaseTime,
                Action = DatabaseActionType.Troubleshooting,
                Before = _serializer.Serialize(new Dictionary<string, string>()),
                After = _serializer.Serialize(new Dictionary<string, string>
                {
                    {"UserId", userSecurity.Id.ToString()},
                    {"Username", userSecurity.Username},
                    {"Email", userSecurity.Email},
                    {"Details", "Email confirmation was attempted when an email confirmation isn't currently pending"}
                })
            });
            return await Result<string>.FailAsync(
                $"An error occurred attempting to confirm account: {userSecurity.Id}, please contact the administrator");
        }
        if (previousConfirmation.Value != confirmationCode)
        {
            await _auditService.CreateAsync(new AuditTrailCreate
            {
                TableName = "EmailConfirmation",
                RecordId = userId,
                ChangedBy = _serverState.SystemUserId,
                Timestamp = _dateTime.NowDatabaseTime,
                Action = DatabaseActionType.Troubleshooting,
                Before = _serializer.Serialize(new Dictionary<string, string>()),
                After = _serializer.Serialize(new Dictionary<string, string>
                {
                    {"UserId", userSecurity.Id.ToString()},
                    {"Username", userSecurity.Username},
                    {"Email", userSecurity.Email},
                    {"Details", "Email confirmation was attempted with an invalid confirmation code"},
                    {"Correct Code ", previousConfirmation.Value},
                    {"Provided Code", confirmationCode}
                })
            });
            return await Result<string>.FailAsync(
                $"An error occurred attempting to confirm account: {userSecurity.Id}, please contact the administrator");
        }
        
        userSecurity.AuthState = AuthState.Enabled;
        userSecurity.EmailConfirmed = true;
        userSecurity.LastModifiedBy = _serverState.SystemUserId;
        userSecurity.LastModifiedOn = _dateTime.NowDatabaseTime;

        var updateSecurity = await _userRepository.UpdateSecurityAsync(userSecurity.ToSecurityUpdate());
        if (!updateSecurity.Success)
            return await Result<string>.FailAsync(updateSecurity.ErrorMessage);

        var confirmEmail = await _userRepository.UpdateAsync(userSecurity.ToUserUpdate());
        if (!confirmEmail.Success)
            return await Result<string>.FailAsync(
                $"An error occurred attempting to confirm account: {userSecurity.Id}, please contact the administrator");
        await _userRepository.RemoveExtendedAttributeAsync(previousConfirmation.Id);
        
        return await Result<string>.SuccessAsync(userSecurity.Id.ToString(), $"Account Confirmed for {userSecurity.Username}.");
    }

    public async Task<IResult> SetUserPassword(Guid userId, string newPassword)
    {
        try
        {
            AccountHelpers.GenerateHashAndSalt(newPassword, _securityConfig.PasswordPepper, out var salt, out var hash);
            var securityUpdate = new AppUserSecurityAttributeUpdate
            {
                OwnerId = userId,
                PasswordSalt = salt,
                PasswordHash = hash
            };
            var securityResult = await _userRepository.UpdateSecurityAsync(securityUpdate);
            if (!securityResult.Success)
                return await Result.FailAsync(securityResult.ErrorMessage);

            var updateUser = new AppUserUpdate
            {
                Id = userId,
                LastModifiedBy = _serverState.SystemUserId,
                LastModifiedOn = _dateTime.NowDatabaseTime
            };
            var userResult = await _userRepository.UpdateAsync(updateUser);
            if (!userResult.Success)
                return await Result.FailAsync(userResult.ErrorMessage);
            
            return await Result.SuccessAsync();
        }
        catch (Exception ex)
        {
            return await Result.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<bool>> IsPasswordCorrect(Guid userId, string password)
    {
        try
        {
            var matchingSecurity = (await _userRepository.GetSecurityAsync(userId)).Result;
            var passwordCorrect = AccountHelpers.IsPasswordCorrect(
                password, matchingSecurity!.PasswordSalt, _securityConfig.PasswordPepper, matchingSecurity.PasswordHash);
            return await Result<bool>.SuccessAsync(passwordCorrect);
        }
        catch (Exception ex)
        {
            return await Result<bool>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult> ForgotPasswordAsync(ForgotPasswordRequest forgotRequest)
    {
        var foundUser = (await _userRepository.GetByEmailAsync(forgotRequest.Email!)).Result;
        if (foundUser is null)
            return await Result.FailAsync(ErrorMessageConstants.GenericError);

        if (!foundUser.EmailConfirmed)
            return await Result.FailAsync(ErrorMessageConstants.EmailNotConfirmedError);

        var previousResets =
            (await _userRepository.GetUserExtendedAttributesByTypeAsync(foundUser.Id, ExtendedAttributeType.PasswordResetToken)).Result;
        var previousReset = previousResets?.FirstOrDefault();
        
        var endpointUri = new Uri(string.Concat(_appConfig.BaseUrl, AppRouteConstants.Identity.ForgotPassword));
        var confirmationUri = QueryHelpers.AddQueryString(endpointUri.ToString(), "userId", foundUser.Id.ToString());
        
        // Previous pending forgot password exists, return current value
        if (previousReset is not null)
        {
            confirmationUri = QueryHelpers.AddQueryString(confirmationUri, "confirmationCode", previousReset.Value);
        }
        else
        {
            // No currently pending forgot password request exists so we'll generate a new one, add it to the provided user
            //   and return the generated reset uri
            var confirmationCode = UrlHelpers.GenerateToken();
            var newExtendedAttribute = new AppUserExtendedAttributeCreate()
            {
                OwnerId = foundUser.Id,
                Name = "ForgotPasswordReset",
                Type = ExtendedAttributeType.PasswordResetToken,
                Value = confirmationCode,
                Description = ""
            };
            await _userRepository.AddExtendedAttributeAsync(newExtendedAttribute);
            confirmationUri = QueryHelpers.AddQueryString(confirmationUri, "confirmationCode", newExtendedAttribute.Value);
        }

        // TODO: Look into why calling this as a BackgroundJob.Enqueue(() => send) fails
        var response = await _mailService.SendPasswordResetEmail(foundUser.Email, foundUser.Username, confirmationUri);
        if (!response.Successful)
            return await Result.FailAsync(
                "Failure occurred attempting to send the password reset email, please reach out to the administrator");

        return await Result.SuccessAsync("Successfully sent password reset to the email provided!");
    }

    public async Task<IResult> ForgotPasswordConfirmationAsync(Guid userId, string confirmationCode, string password, string confirmPassword)
    {
        var foundUser = (await _userRepository.GetByIdAsync(userId)).Result;
        if (foundUser is null)
            return await Result.FailAsync(ErrorMessageConstants.GenericError);

        var previousResets =
            (await _userRepository.GetUserExtendedAttributesByTypeAsync(foundUser.Id, ExtendedAttributeType.PasswordResetToken)).Result;
        var previousReset = previousResets?.FirstOrDefault();
        
        if (previousReset is null)
            return await Result.FailAsync(ErrorMessageConstants.GenericError);
        if (password != confirmPassword)
            return await Result.FailAsync(ErrorMessageConstants.PasswordsNoMatchError);
        if (confirmationCode != previousReset.Value)
            return await Result.FailAsync(ErrorMessageConstants.TokenInvalidError);

        var passwordMeetsRequirements = (await PasswordMeetsRequirements(password)).Data;
        if (!passwordMeetsRequirements)
            return await Result.FailAsync("Password provided doesn't meet the minimum requirements, please try again");
        
        await SetUserPassword(foundUser.Id, password);

        await _userRepository.RemoveExtendedAttributeAsync(previousReset.Id);
        await SetAuthState(userId, AuthState.Enabled);

        return await Result.SuccessAsync("Password reset was successful, please log back in with your fresh new password!");
    }

    public async Task<IResult<UserLoginResponse>> ReAuthUsingRefreshTokenAsync(RefreshTokenRequest? refreshRequest)
    {
        if (refreshRequest is null)
            return await Result<UserLoginResponse>.FailAsync(ErrorMessageConstants.TokenInvalidError);
        
        var tokenUserId = JwtHelpers.GetJwtUserId(refreshRequest.Token);
        var refreshTokenUserId = JwtHelpers.GetJwtUserId(refreshRequest.RefreshToken);
        
        // Validate provided token and refresh token user ID's match, otherwise the request is suspicious
        if (tokenUserId != refreshTokenUserId)
            return await Result<UserLoginResponse>.FailAsync(ErrorMessageConstants.TokenInvalidError);
        
        var user = (await _userRepository.GetByIdAsync(refreshTokenUserId)).Result;
        if (user == null)
            return await Result<UserLoginResponse>.FailAsync(ErrorMessageConstants.TokenInvalidError);
        
        // Validate the provided client id has been registered, if not this client can't use a refresh token
        var clientIdRequest = await _userRepository.GetUserExtendedAttributesByTypeAndValueAsync(
            user.Id, ExtendedAttributeType.UserClientId, refreshRequest.ClientId);
        if (!clientIdRequest.Success || clientIdRequest.Result is null || !clientIdRequest.Result.Any())
            return await Result<UserLoginResponse>.FailAsync(ErrorMessageConstants.TokenInvalidError);
        
        if (!JwtHelpers.IsJwtValid(refreshRequest.RefreshToken, _securityConfig, _appConfig))
            return await Result<UserLoginResponse>.FailAsync(ErrorMessageConstants.TokenInvalidError);
        
        // Token & Refresh Token have been validated and matched, now re-auth the user and generate new tokens
        var token = JwtHelpers.GenerateUserJwtEncryptedToken(await GetClaimsAsync(user.ToUserDb()), _dateTime, _securityConfig, _appConfig);
        var refreshToken = JwtHelpers.GenerateUserJwtRefreshToken(_dateTime, _securityConfig, _appConfig, user.Id);
        var refreshTokenExpiration = JwtHelpers.GetJwtExpirationTime(refreshToken);

        var response = new UserLoginResponse { ClientId = refreshRequest.ClientId, Token = token, RefreshToken = refreshToken,
            RefreshTokenExpiryTime = refreshTokenExpiration };
        return await Result<UserLoginResponse>.SuccessAsync(response);
    }

    public async Task<IResult> UpdatePreferences(Guid userId, AppUserPreferenceUpdate preferenceUpdate)
    {
        var updateRequest = await _userRepository.UpdatePreferences(userId, preferenceUpdate);
        if (!updateRequest.Success)
            return await Result.FailAsync($"Failure occurred attempting to update preferences: {updateRequest.ErrorMessage}");

        return await Result.SuccessAsync("Preferences updated successfully");
    }

    public async Task<IResult<AppUserPreferenceFull>> GetPreferences(Guid userId)
    {
        var preferences = await _userRepository.GetPreferences(userId);
        if (!preferences.Success)
            return await Result<AppUserPreferenceFull>.FailAsync($"Failure occurred getting preferences: {preferences.ErrorMessage}");

        if (preferences.Result is null)
            return await Result<AppUserPreferenceFull>.FailAsync("Preferences couldn't be found for the UserId provided");

        var preferencesFull = preferences.Result.ToFull();
            
        preferencesFull.CustomThemeOne = JsonConvert.DeserializeObject<AppThemeCustom>(preferences.Result.CustomThemeOne!);
        preferencesFull.CustomThemeTwo = JsonConvert.DeserializeObject<AppThemeCustom>(preferences.Result.CustomThemeTwo!);
        preferencesFull.CustomThemeThree = JsonConvert.DeserializeObject<AppThemeCustom>(preferences.Result.CustomThemeThree!);

        return await Result<AppUserPreferenceFull>.SuccessAsync(preferencesFull);
    }

    public async Task<IResult> ForceUserLogin(Guid userId)
    {
        var userSecurity = await _userRepository.GetByIdSecurityAsync(userId);
        if (!userSecurity.Success || userSecurity.Result is null)
            return await Result.FailAsync(ErrorMessageConstants.UserNotFoundError);

        // Update account auth state to indicate login is required
        userSecurity.Result.AuthState = AuthState.LoginRequired;
        var updateSecurity = await _userRepository.UpdateSecurityAsync(userSecurity.Result.ToSecurityUpdate());
        if (!updateSecurity.Success)
            return await Result.FailAsync(updateSecurity.ErrorMessage);
        
        // Grab all registered client id's for the user account
        var userClientIdRequest =
            await _userRepository.GetUserExtendedAttributesByTypeAsync(userSecurity.Result.Id, ExtendedAttributeType.UserClientId);
        if (!userClientIdRequest.Success)
            return await Result.FailAsync(userClientIdRequest.ErrorMessage);

        var messages = new List<string>();
        
        // Remove all client id's for the specified user account which will require a user to login when the primary JWT expires
        //   This also means a refresh token cannot be used for any client without doing a full user+pass authentication
        var userClientIds = (userClientIdRequest.Result ?? new List<AppUserExtendedAttributeDb>()).ToArray();
        if (userClientIds.Any())
        {
            foreach (var clientId in userClientIds)
            {
                var removeRequest = await _userRepository.RemoveExtendedAttributeAsync(clientId.Id);
                if (!removeRequest.Success)
                    messages.Add(removeRequest.ErrorMessage);
            }
        }

        if (messages.Any())
            return await Result.FailAsync(messages);

        return await Result.SuccessAsync();
    }

    public async Task<IResult> ForceUserPasswordReset(Guid userId)
    {
        var userSecurity = await _userRepository.GetByIdSecurityAsync(userId);
        if (!userSecurity.Success)
            return await Result.FailAsync(userSecurity.ErrorMessage);

        var forceLoginRequest = await ForceUserLogin(userId);
        if (!forceLoginRequest.Succeeded)
            return await Result.FailAsync(forceLoginRequest.Messages);
        
        await SetUserPassword(userId, UrlHelpers.GenerateToken());
        return await ForgotPasswordAsync(new ForgotPasswordRequest() { Email = userSecurity.Result!.Email });
    }

    public async Task<IResult> SetTwoFactorEnabled(Guid userId, bool enabled)
    {
        var userSecurity = await _userRepository.GetSecurityAsync(userId);
        if (!userSecurity.Success || userSecurity.Result is null)
            return await Result.FailAsync(userSecurity.ErrorMessage);

        userSecurity.Result.TwoFactorEnabled = enabled;

        var updateSecurity = await _userRepository.UpdateSecurityAsync(userSecurity.Result.ToUpdate());
        if (!updateSecurity.Success)
            return await Result.FailAsync(updateSecurity.ErrorMessage);

        return await Result.SuccessAsync();
    }

    public async Task<IResult> SetTwoFactorKey(Guid userId, string key)
    {
        var userSecurity = await _userRepository.GetSecurityAsync(userId);
        if (!userSecurity.Success || userSecurity.Result is null)
            return await Result.FailAsync(userSecurity.ErrorMessage);

        userSecurity.Result.TwoFactorKey = key;

        var updateSecurity = await _userRepository.UpdateSecurityAsync(userSecurity.Result.ToUpdate());
        if (!updateSecurity.Success)
            return await Result.FailAsync(updateSecurity.ErrorMessage);

        return await Result.SuccessAsync();
    }

    private async Task<string> GenerateJwtAsync(AppUserDb user, bool isApiToken = false)
    {
        return isApiToken ?
            JwtHelpers.GenerateApiJwtEncryptedToken(await GetClaimsAsync(user), _dateTime, _securityConfig, _appConfig) : 
            JwtHelpers.GenerateUserJwtEncryptedToken(await GetClaimsAsync(user), _dateTime, _securityConfig, _appConfig);
    }

    private async Task<IEnumerable<Claim>> GetClaimsAsync(AppUserDb user)
    {
        var allUserAndRolePermissions = 
            (await _appPermissionRepository.GetAllIncludingRolesForUserAsync(user.Id)).Result?.ToClaims() ?? new List<Claim>();
        var allRoles = 
            (await _roleRepository.GetRolesForUser(user.Id)).Result?.ToClaims() ?? new List<Claim>();

        var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Name, user.Username)
            }
        .Union(allUserAndRolePermissions)
        .Union(allRoles);

        return claims;
    }

    private async Task<string> GetAuthTokenFromSession()
    {
        var authToken = GetTokenFromHttpAuthorizationHeader();
        if (!string.IsNullOrWhiteSpace(authToken))
            return authToken;
        
        authToken = await GetTokenFromLocalStorage();

        return authToken;
    }

    private string GetTokenFromHttpAuthorizationHeader()
    {
        try
        {
            var headerHasValue = _contextAccessor.HttpContext!.Request.Headers.TryGetValue("Authorization", out var bearer);
            if (!headerHasValue)
                return "";
            
            // Authorization header should always be: <scheme> <token>, which in our case is: Bearer JWT
            return bearer.ToString().Split(' ')[1];
        }
        catch
        {
            return "";
        }
    }

    private async Task<string> GetTokenFromLocalStorage()
    {
        try
        {
            return await _localStorage.GetItemAsync<string>(LocalStorageConstants.AuthToken);
        }
        catch
        {
            // Since Blazor Server pre-rendering has the state received twice and we can't have JSInterop run while rendering is occurring
            //   we have to do this to keep our sanity, would love to find a working solution to this at some point
            return "";
        }
    }

    public async Task<IResult<bool>> IsCurrentSessionValid()
    {
        try
        {
            var authToken = await GetAuthTokenFromSession();
            var sessionIsValid = JwtHelpers.IsJwtValid(authToken, _securityConfig, _appConfig);

            return await Result<bool>.SuccessAsync(sessionIsValid);
        }
        catch (Exception ex)
        {
            // Token isn't valid and has likely expired
            return await Result<bool>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<bool>> IsUserRequiredToReAuthenticate(Guid userId)
    {
        try
        {
            var userSecurity = await _userRepository.GetSecurityAsync(userId);
            userSecurity.Result!.LastFullLogin ??= _dateTime.NowDatabaseTime;
            
            // If configured force login time has passed since last full login we want the user to login again
            if (userSecurity.Result!.LastFullLogin!.Value.AddMinutes(_securityConfig.ForceLoginIntervalMinutes) < _dateTime.NowDatabaseTime)
                return await Result<bool>.SuccessAsync(true);
            
            // If account auth state is set to force re-login we want the user to login again
            return await Result<bool>.SuccessAsync(userSecurity.Result!.AuthState == AuthState.LoginRequired);
        }
        catch (Exception ex)
        {
            return await Result<bool>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult> SetAuthState(Guid userId, AuthState authState)
    {
        var userSecurity = await _userRepository.GetSecurityAsync(userId);
        if (!userSecurity.Success || userSecurity.Result is null)
            return await Result.FailAsync(userSecurity.ErrorMessage);

        userSecurity.Result.AuthState = authState;
        // If we are enabling the account we'll reset bad password attempts
        if (authState == AuthState.Enabled)
            userSecurity.Result.BadPasswordAttempts = 0;

        var updateSecurity = await _userRepository.UpdateSecurityAsync(userSecurity.Result.ToUpdate());
        if (!updateSecurity.Success)
            return await Result.FailAsync(updateSecurity.ErrorMessage);

        return await Result.SuccessAsync($"User account successfully set: {userSecurity.Result.AuthState.ToString()}");
    }

    public async Task<IResult> GenerateUserApiToken(Guid userId, UserApiTokenTimeframe timeframe, string description)
    {
        var foundUserRequest = await _userRepository.GetByIdAsync(userId);
        if (!foundUserRequest.Success || foundUserRequest.Result is null)
            return await Result.FailAsync(ErrorMessageConstants.UserNotFoundError);

        var userApiToken = UrlHelpers.GenerateToken(_securityConfig.UserApiTokenSizeInBytes);
        var tokenExpiration = UserApiHelpers.GetUserApiTokenExpirationTime(_dateTime, timeframe);

        var newExtendedAttribute = new AppUserExtendedAttributeCreate()
        {
            OwnerId = foundUserRequest.Result.Id,
            Name = tokenExpiration.ToString(CultureInfo.CurrentCulture),
            Type = ExtendedAttributeType.UserApiToken,
            Value = userApiToken,
            Description = description
        };
        var addRequest = await _userRepository.AddExtendedAttributeAsync(newExtendedAttribute);
        if (!addRequest.Success)
            return await Result.FailAsync(addRequest.ErrorMessage);

        return await Result.SuccessAsync();
    }

    public async Task<IResult> DeleteUserApiToken(Guid userId, string value)
    {
        var foundUserRequest = await _userRepository.GetByIdAsync(userId);
        if (!foundUserRequest.Success || foundUserRequest.Result is null)
            return await Result.FailAsync(ErrorMessageConstants.UserNotFoundError);

        var apiTokenRequest = 
            await _userRepository.GetUserExtendedAttributesByTypeAndValueAsync(userId, ExtendedAttributeType.UserApiToken, value);
        if (!apiTokenRequest.Success || apiTokenRequest.Result is null || !apiTokenRequest.Result.Any())
            return await Result.FailAsync(ErrorMessageConstants.GenericNotFound);

        var apiToken = apiTokenRequest.Result.FirstOrDefault();
        if (apiToken is null)
            return await Result.FailAsync(ErrorMessageConstants.GenericNotFound);

        if (apiToken.OwnerId != foundUserRequest.Result.Id)
            return await Result.FailAsync(ErrorMessageConstants.GenericNotFound);
        
        var removeRequest = await _userRepository.RemoveExtendedAttributeAsync(apiToken.Id);
        if (!removeRequest.Success)
            return await Result.FailAsync(removeRequest.ErrorMessage);

        return await Result.SuccessAsync();
    }

    public async Task<IResult> DeleteAllUserApiTokens(Guid userId)
    {
        var foundUserRequest = await _userRepository.GetByIdAsync(userId);
        if (!foundUserRequest.Success || foundUserRequest.Result is null)
            return await Result.FailAsync(ErrorMessageConstants.UserNotFoundError);
        
        var existingTokenRequest =
            await _userRepository.GetUserExtendedAttributesByTypeAsync(foundUserRequest.Result.Id, ExtendedAttributeType.UserApiToken);
        if (!existingTokenRequest.Success)
            return await Result.FailAsync(existingTokenRequest.ErrorMessage);

        var messages = new List<string>();
        
        var existingTokens = (existingTokenRequest.Result ?? new List<AppUserExtendedAttributeDb>()).ToArray();
        if (existingTokens.Any())
        {
            foreach (var token in existingTokens)
            {
                var removeRequest = await _userRepository.RemoveExtendedAttributeAsync(token.Id);
                if (!removeRequest.Success)
                    messages.Add(removeRequest.ErrorMessage);
            }
        }

        if (messages.Any())
            return await Result.FailAsync(messages);

        return await Result.SuccessAsync();
    }
}
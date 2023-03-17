using System.Net.Http.Headers;
using System.Security.Claims;
using Application.Constants.Identity;
using Application.Constants.Web;
using Application.Models.Identity;
using Application.Services.System;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services.Identity;

public class AuthStateProvider : AuthenticationStateProvider
{
    private readonly HttpClient _httpClient;
    private readonly ISerializerService _serializer;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly ILocalStorageService _localStorage;
    private readonly ILogger _logger;

    public AuthStateProvider(HttpClient httpClient, ISerializerService serializer,
        IHttpContextAccessor contextAccessor, ILocalStorageService localStorage, ILogger logger)
    {
        _serializer = serializer;
        _contextAccessor = contextAccessor;
        _localStorage = localStorage;
        _logger = logger;
        _httpClient = httpClient;
    }

    private string _authToken = "";

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var currentPrincipal = GetPrincipalFromHttpContext();
            if (currentPrincipal.Identity?.Name != UserConstants.UnauthenticatedIdentity.Name)
                return new AuthenticationState(currentPrincipal);

            await GetSavedAuthToken();
            if (string.IsNullOrWhiteSpace(_authToken))
                return new AuthenticationState(UserConstants.UnauthenticatedPrincipal);
            
            return GenerateNewAuthenticationState(_authToken);
        }
        catch
        {
            return new AuthenticationState(UserConstants.UnauthenticatedPrincipal);
        }
    }

    public async Task<AuthenticationState> GetAuthenticationStateAsync(string providedToken)
    {
        _authToken = providedToken;
        return await GetAuthenticationStateAsync();
    }

    private AuthenticationState GenerateNewAuthenticationState(string savedToken)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", savedToken);
        
        var authorizedPrincipal = new ClaimsPrincipal(new ClaimsIdentity(GetClaimsFromJwt(savedToken), JwtBearerDefaults
            .AuthenticationScheme));
        _contextAccessor.HttpContext!.User = authorizedPrincipal;
        
        var state = new AuthenticationState(authorizedPrincipal);
        AuthenticationStateUser = state.User;
        return state;
    }

    private async Task GetSavedAuthToken()
    {
        _authToken = GetTokenFromHttpSession();
            
        if (string.IsNullOrWhiteSpace(_authToken))
            _authToken = await GetTokenFromLocalStorage();
            
        if (string.IsNullOrWhiteSpace(_authToken))
            _authToken = _httpClient.DefaultRequestHeaders.Authorization?.ToString() ?? "";
    }

    private ClaimsPrincipal GetPrincipalFromHttpContext()
    {
        try
        {
            return _contextAccessor.HttpContext?.User!;
        }
        catch
        {
            return UserConstants.UnauthenticatedPrincipal;
        }
    }

    private string GetTokenFromHttpSession()
    {
        try
        {
            return _contextAccessor.HttpContext!.Session.GetString(LocalStorageConstants.AuthToken)!;
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

    public ClaimsPrincipal AuthenticationStateUser { get; set; } = null!;

    public void IndicateUserAuthenticationSuccess(string userName)
    {
        var authenticatedUser = new ClaimsPrincipal(
            new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, userName)
            }, JwtBearerDefaults.AuthenticationScheme));

        var authState = Task.FromResult(new AuthenticationState(authenticatedUser));

        // NotifyAuthenticationStateChanged(authState);
    }

    public void IndicateUserAuthenticationSuccess(AuthenticationState authState)
    {
        var taskAuthState = Task.FromResult(authState);
        _contextAccessor.HttpContext!.User = new ClaimsPrincipal(authState.User.Identities);
        AuthenticationStateUser = authState.User;

        // NotifyAuthenticationStateChanged(taskAuthState);
    }

    public void DeauthenticateUser()
    {
        try
        {
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            var authState = Task.FromResult(new AuthenticationState(anonymousUser));
        }
        catch (Exception ex)
        {
            _logger.Warning("Error occurred attempting to deauthenticate user: {ErrorMessage}", ex.Message);
        }

        // NotifyAuthenticationStateChanged(authState);
    }

    public async Task<ClaimsPrincipal> GetAuthenticationStateProviderUserAsync()
    {
        var state = await GetAuthenticationStateAsync();
        return state.User;
    }

    public IEnumerable<Claim> GetClaimsFromJwt(string jwt)
    {
        var claims = new List<Claim>();
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = _serializer.Deserialize<Dictionary<string, object>>(jsonBytes);

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (keyValuePairs is null) return claims;
        keyValuePairs.TryGetValue(ClaimTypes.Role, out var roles);

        if (roles != null)
        {
            if (roles.ToString()!.Trim().StartsWith("["))
            {
                var parsedRoles = _serializer.Deserialize<string[]>(roles.ToString()!);

                claims.AddRange(parsedRoles.Select(role => new Claim(ClaimTypes.Role, role)));
            }
            else
            {
                claims.Add(new Claim(ClaimTypes.Role, roles.ToString()!));
            }

            keyValuePairs.Remove(ClaimTypes.Role);
        }

        keyValuePairs.TryGetValue(ApplicationClaimTypes.Permission, out var permissions);
        if (permissions != null)
        {
            if (permissions.ToString()!.Trim().StartsWith("["))
            {
                var parsedPermissions = _serializer.Deserialize<string[]>(permissions.ToString()!);
                claims.AddRange(parsedPermissions.Select(permission => new Claim(ApplicationClaimTypes.Permission, permission)));
            }
            else
            {
                claims.Add(new Claim(ApplicationClaimTypes.Permission, permissions.ToString()!));
            }
            keyValuePairs.Remove(ApplicationClaimTypes.Permission);
        }

        claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()!)));
        return claims;
    }

    private byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }

        return Convert.FromBase64String(base64);
    }
}
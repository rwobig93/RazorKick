﻿using Application.Models.Identity;
using Application.Models.Web;
using Domain.DatabaseEntities.Identity;
using Domain.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Shared.Requests.Identity;
using Shared.Responses.Identity;

namespace Application.Repositories.Identity;

public interface IAppUserRepository
{
    Task<IEnumerable<AppUserDb>> GetAllAsync();
    Task<int> GetCountAsync();
    Task<AppUserDb?> GetByIdAsync(Guid id);
    Task<AppUserFull?> GetByIdFullAsync(Guid id);
    Task<AppUserDb?> GetByUsernameAsync(string username);
    Task<AppUserDb?> GetByEmailAsync(string email);
    Task UpdateAsync(AppUserUpdate updateObject);
    Task<IdentityResult> UpdateAsync(AppUserDb user, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id);
    Task<IdentityResult> DeleteAsync(AppUserDb user, CancellationToken cancellationToken);
    Task<string> GetUserIdAsync(AppUserDb user, CancellationToken cancellationToken);
    Task<string> GetUserNameAsync(AppUserDb user, CancellationToken cancellationToken);
    Task SetUserNameAsync(AppUserDb user, string userName, CancellationToken cancellationToken);
    Task<string> GetNormalizedUserNameAsync(AppUserDb user, CancellationToken cancellationToken);
    Task SetNormalizedUserNameAsync(AppUserDb user, string normalizedName, CancellationToken cancellationToken);
    Task<Guid?> CreateAsync(AppUserCreate createObject);
    Task<IdentityResult> CreateAsync(AppUserDb user, CancellationToken cancellationToken);
    Task<AppUserDb> FindByIdAsync(string userId, CancellationToken cancellationToken);
    Task<AppUserDb> FindByNameAsync(string normalizedUsername, CancellationToken cancellationToken);
    Task SetEmailAsync(AppUserDb user, string email, CancellationToken cancellationToken);
    Task<string> GetEmailAsync(AppUserDb user, CancellationToken cancellationToken);
    Task<bool> GetEmailConfirmedAsync(AppUserDb user, CancellationToken cancellationToken);
    Task SetEmailConfirmedAsync(AppUserDb user, bool confirmed, CancellationToken cancellationToken);
    Task<AppUserDb> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken);
    Task<string> GetNormalizedEmailAsync(AppUserDb user, CancellationToken cancellationToken);
    Task SetNormalizedEmailAsync(AppUserDb user, string normalizedEmail, CancellationToken cancellationToken);
    Task SetPhoneNumberAsync(AppUserDb user, string phoneNumber, CancellationToken cancellationToken);
    Task<string> GetPhoneNumberAsync(AppUserDb user, CancellationToken cancellationToken);
    Task<bool> GetPhoneNumberConfirmedAsync(AppUserDb user, CancellationToken cancellationToken);
    Task SetPhoneNumberConfirmedAsync(AppUserDb user, bool confirmed, CancellationToken cancellationToken);
    Task SetTwoFactorEnabledAsync(AppUserDb user, bool enabled, CancellationToken cancellationToken);
    Task<bool> GetTwoFactorEnabledAsync(AppUserDb user, CancellationToken cancellationToken);
    Task SetPasswordHashAsync(AppUserDb user, string passwordHash, CancellationToken cancellationToken);
    Task<string> GetPasswordHashAsync(AppUserDb user, CancellationToken cancellationToken);
    Task<bool> HasPasswordAsync(AppUserDb user, CancellationToken cancellationToken);
    Task<IResult<UserLoginResponse>> LoginAsync(UserLoginRequest loginRequest);
    Task<IResult<List<IdentityResult>>> AddToRolesAsync(UserRoleRequest roleRequest);
    Task<IResult<List<IdentityResult>>> RemoveFromRolesAsync(UserRoleRequest roleRequest);
    Task<IResult<IdentityResult>> EnforceRolesAsync(UserRoleRequest roleRequest);
    Task<IResult> RegisterAsync(UserRegisterRequest registerRequest);
    Task<string> GetEmailConfirmationUrl(AppUserDb user);
    Task<IResult> ToggleUserStatusAsync(ChangeUserActiveStateRequest activeRequest);
    Task<IResult<List<RoleResponse>>> GetRolesAsync(Guid userId);
    Task<IResult<string>> ConfirmEmailAsync(Guid userId, string confirmationCode);
    Task<IResult> ForgotPasswordAsync(ForgotPasswordRequest forgotRequest);
    Task<IResult> ResetPasswordAsync(ResetPasswordRequest resetRequest);
    void Dispose();
    Task<IResult<UserLoginResponse>> GetRefreshTokenAsync(RefreshTokenRequest? refreshRequest);
}
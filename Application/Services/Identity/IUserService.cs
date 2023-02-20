using Application.Models.Web;
using Domain.DatabaseEntities.Identity;
using Microsoft.AspNetCore.Identity;
using Shared.Requests.Identity;
using Shared.Responses.Identity;

namespace Application.Services.Identity;

public interface IUserService
{
    Task<IEnumerable<AppUser>> GetAllAsync();
    Task<int> GetCountAsync();
    Task<AppUser?> GetByIdAsync(GetUserByIdRequest userRequest);
    Task<AppUser?> GetByUsernameAsync(GetUserByUsernameRequest userRequest);
    Task UpdateAsync(UpdateUserRequest userRequest);
    Task<IdentityResult> UpdateAsync(AppUser user, CancellationToken cancellationToken);
    Task DeleteAsync(DeleteUserRequest userRequest);
    Task<IdentityResult> DeleteAsync(AppUser user, CancellationToken cancellationToken);
    Task<string> GetUserIdAsync(AppUser user, CancellationToken cancellationToken);
    Task<string> GetUserNameAsync(AppUser user, CancellationToken cancellationToken);
    Task SetUserNameAsync(AppUser user, string userName, CancellationToken cancellationToken);
    Task<string> GetNormalizedUserNameAsync(AppUser user, CancellationToken cancellationToken);
    Task SetNormalizedUserNameAsync(AppUser user, string normalizedName, CancellationToken cancellationToken);
    Task<IdentityResult> CreateAsync(AppUser user, CancellationToken cancellationToken);
    Task<AppUser> FindByIdAsync(string userId, CancellationToken cancellationToken);
    Task<AppUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken);
    Task SetEmailAsync(AppUser user, string email, CancellationToken cancellationToken);
    Task<string> GetEmailAsync(AppUser user, CancellationToken cancellationToken);
    Task<bool> GetEmailConfirmedAsync(AppUser user, CancellationToken cancellationToken);
    Task SetEmailConfirmedAsync(AppUser user, bool confirmed, CancellationToken cancellationToken);
    Task<AppUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken);
    Task<string> GetNormalizedEmailAsync(AppUser user, CancellationToken cancellationToken);
    Task SetNormalizedEmailAsync(AppUser user, string normalizedEmail, CancellationToken cancellationToken);
    Task SetPhoneNumberAsync(AppUser user, string phoneNumber, CancellationToken cancellationToken);
    Task<string> GetPhoneNumberAsync(AppUser user, CancellationToken cancellationToken);
    Task<bool> GetPhoneNumberConfirmedAsync(AppUser user, CancellationToken cancellationToken);
    Task SetPhoneNumberConfirmedAsync(AppUser user, bool confirmed, CancellationToken cancellationToken);
    Task SetTwoFactorEnabledAsync(AppUser user, bool enabled, CancellationToken cancellationToken);
    Task<bool> GetTwoFactorEnabledAsync(AppUser user, CancellationToken cancellationToken);
    Task SetPasswordHashAsync(AppUser user, string passwordHash, CancellationToken cancellationToken);
    Task<string> GetPasswordHashAsync(AppUser user, CancellationToken cancellationToken);
    Task<bool> HasPasswordAsync(AppUser user, CancellationToken cancellationToken);
    Task<IResult<UserLoginResponse>> LoginAsync(UserLoginRequest loginRequest);
    Task<IResult<List<IdentityResult>>> AddToRolesAsync(UserRoleRequest roleRequest);
    Task<IResult<List<IdentityResult>>> RemoveFromRolesAsync(UserRoleRequest roleRequest);
    Task<IResult<IdentityResult>> EnforceRolesAsync(UserRoleRequest roleRequest);
    Task<IResult> RegisterAsync(UserRegisterRequest registerRequest);
    Task<string> GetEmailConfirmationUrl(AppUser user);
    Task<IResult> ToggleUserStatusAsync(ChangeUserActiveStateRequest activeRequest);
    Task<IResult<List<RoleResponse>>> GetRolesAsync(Guid userId);
    Task<IResult<string>> ConfirmEmailAsync(Guid userId, string confirmationCode);
    Task<IResult> ForgotPasswordAsync(ForgotPasswordRequest forgotRequest);
    Task<IResult> ResetPasswordAsync(ResetPasswordRequest resetRequest);
    void Dispose();
    Task<IResult<UserLoginResponse>> GetRefreshTokenAsync(RefreshTokenRequest? refreshRequest);
}
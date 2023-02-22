using Application.Models.Web;
using Domain.DatabaseEntities.Identity;
using Microsoft.AspNetCore.Identity;
using Shared.Requests.Identity;
using Shared.Requests.Identity.User;
using Shared.Responses.Identity;

namespace Application.Services.Identity;

public interface IAppIdentityService : IUserEmailStore<AppUserDb>, IUserPhoneNumberStore<AppUserDb>,
    IUserTwoFactorStore<AppUserDb>, IUserPasswordStore<AppUserDb>
{
    Task<IResult<UserLoginResponse>> LoginAsync(UserLoginRequest loginRequest);
    Task<IResult<List<IdentityResult>>> AddToRolesAsync(UserRoleRequest roleRequest);
    Task<IResult<List<IdentityResult>>> RemoveFromRolesAsync(UserRoleRequest roleRequest);
    Task<IResult> RegisterAsync(UserRegisterRequest registerRequest);
    Task<string> GetEmailConfirmationUrl(AppUserDb user);
    Task<IResult> ToggleUserStatusAsync(ChangeUserActiveStateRequest activeRequest);
    Task<IResult<List<AppRoleDb>>> GetRolesAsync(Guid userId);
    Task<IResult<string>> ConfirmEmailAsync(Guid userId, string confirmationCode);
    Task<IResult> ForgotPasswordAsync(ForgotPasswordRequest forgotRequest);
    Task<IResult> ResetPasswordAsync(ResetPasswordRequest resetRequest);
    Task<IResult<UserLoginResponse>> GetRefreshTokenAsync(RefreshTokenRequest? refreshRequest);
}
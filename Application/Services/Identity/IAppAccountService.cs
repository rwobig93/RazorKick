using Application.Models.Identity;
using Application.Models.Web;
using Domain.Models.Identity;
using Shared.Requests.Identity.User;
using Shared.Responses.Identity;

namespace Application.Services.Identity;

public interface IAppAccountService
{
    Task<IResult<UserLoginResponse>> LoginAsync(UserLoginRequest loginRequest);
    Task<IResult<UserLoginResponse>> LoginGuiAsync(UserLoginRequest loginRequest);
    Task<IResult> LogoutGuiAsync(Guid userId);
    Task<IResult<bool>> PasswordMeetsRequirements(string password);
    Task<IResult> RegisterAsync(UserRegisterRequest registerRequest);
    Task<IResult<string>> GetEmailConfirmationUrl(Guid userId);
    Task<IResult<string>> ConfirmEmailAsync(Guid userId, string confirmationCode);
    Task<IResult> SetUserPassword(Guid userId, string newPassword);
    Task<IResult<bool>> IsPasswordCorrect(Guid userId, string password);
    Task<IResult> ForgotPasswordAsync(ForgotPasswordRequest forgotRequest);
    Task<IResult> ForgotPasswordConfirmationAsync(Guid userId, string confirmationCode, string password, string confirmPassword);
    Task<IResult<UserLoginResponse>> GetRefreshTokenAsync(RefreshTokenRequest? refreshRequest);
    Task<IResult> UpdatePreferences(Guid userId, AppUserPreferenceUpdate preferenceUpdate);
    Task<IResult<AppUserPreferenceFull>> GetPreferences(Guid userId);
    Task<IResult> ChangeUserEnabledState(Guid userId, bool enabled);
    Task<IResult> ForceUserPasswordReset(Guid userId);
    Task<IResult> SetTwoFactorEnabled(Guid userId, bool enabled);
    Task<IResult> SetTwoFactorKey(Guid userId, string key);
}
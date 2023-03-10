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
    Task<IResult> LogoutGuiAsync();
    Task<IResult> RegisterAsync(UserRegisterRequest registerRequest);
    Task<string> GetEmailConfirmationUrl(Guid userId);
    Task<IResult<string>> ConfirmEmailAsync(Guid userId, string confirmationCode);
    Task<IResult> ForgotPasswordAsync(ForgotPasswordRequest forgotRequest);
    Task<IResult> ForgotPasswordConfirmationAsync(ResetPasswordRequest resetRequest);
    Task<IResult<UserLoginResponse>> GetRefreshTokenAsync(RefreshTokenRequest? refreshRequest);
    Task<IResult> UpdatePreferences(Guid userId, AppUserPreferenceUpdate preferenceUpdate);
    Task<IResult<AppUserPreferenceFull>> GetPreferences(Guid userId);
}
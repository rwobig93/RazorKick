using Application.Models.Identity.UserExtensions;
using Application.Models.Web;
using Application.Requests.Api;
using Application.Requests.Identity.User;
using Application.Responses.Api;
using Application.Responses.Identity;
using Domain.Enums.Identity;
using Domain.Models.Identity;

namespace Application.Services.Identity;

public interface IAppAccountService
{
    Task<IResult<UserLoginResponse>> LoginAsync(UserLoginRequest loginRequest);
    Task<IResult<UserLoginResponse>> LoginGuiAsync(UserLoginRequest loginRequest);
    Task<IResult<ApiTokenResponse>> GetApiAuthToken(ApiGetTokenRequest tokenRequest);
    Task<IResult> CacheAuthTokens(IResult<UserLoginResponse> loginResponse);
    Task<IResult> LogoutGuiAsync(Guid userId);
    Task<IResult<bool>> PasswordMeetsRequirements(string password);
    Task<IResult> RegisterAsync(UserRegisterRequest registerRequest);
    Task<IResult<string>> GetEmailConfirmationUrl(Guid userId);
    Task<IResult<string>> ConfirmEmailAsync(Guid userId, string confirmationCode);
    Task<IResult> SetUserPassword(Guid userId, string newPassword);
    Task<IResult<bool>> IsPasswordCorrect(Guid userId, string password);
    Task<IResult> ForgotPasswordAsync(ForgotPasswordRequest forgotRequest);
    Task<IResult> ForgotPasswordConfirmationAsync(Guid userId, string confirmationCode, string password, string confirmPassword);
    Task<IResult<UserLoginResponse>> ReAuthUsingRefreshTokenAsync(RefreshTokenRequest? refreshRequest);
    Task<IResult> UpdatePreferences(Guid userId, AppUserPreferenceUpdate preferenceUpdate);
    Task<IResult<AppUserPreferenceFull>> GetPreferences(Guid userId);
    Task<IResult> ForceUserLogin(Guid userId);
    Task<IResult> ForceUserPasswordReset(Guid userId);
    Task<IResult> SetTwoFactorEnabled(Guid userId, bool enabled);
    Task<IResult> SetTwoFactorKey(Guid userId, string key);
    Task<IResult<bool>> IsCurrentSessionValid();
    Task<IResult<bool>> IsUserRequiredToReAuthenticate(Guid userId);
    Task<IResult> SetAuthState(Guid userId, AuthState authState);
    Task<IResult> GenerateUserApiToken(Guid userId, UserApiTokenTimeframe timeframe, string description);
    Task<IResult> DeleteUserApiToken(Guid userId, string value);
    Task<IResult> DeleteAllUserApiTokens(Guid userId);
}
using Application.Models.Identity.External;
using Application.Models.Web;
using Domain.Enums.Integration;

namespace Application.Services.Integrations;

public interface IExternalAuthProviderService
{
    public bool ProviderEnabledGoogle { get; }
    public bool ProviderEnabledDiscord { get; }
    public bool ProviderEnabledSpotify { get; }
    public bool ProviderEnabledFacebook { get; }
    
    public Task<IResult<string>> GetLoginUri(ExternalAuthProvider provider);
    public Task<IResult<ExternalUserProfile>> GetUserProfile(ExternalAuthProvider provider, string oauthCode);
}
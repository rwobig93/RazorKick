using System.Security.Claims;
using Application.Services.Identity;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services.Identity;

public class CurrentUserService : ICurrentUserService
{
    public Guid? UserId { get; }

    public string? Username { get; }

    public string? Email { get; }

    public List<KeyValuePair<string, string>> Claims { get; }

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        var isGuid = Guid.TryParse(httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier), out var userId);
        UserId = isGuid ? userId : null;
        Username = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);
        Email = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);
        Claims = httpContextAccessor.HttpContext?.User?.Claims.AsEnumerable()
            .Select(item => new KeyValuePair<string, string>(item.Type, item.Value)).ToList() ?? new List<KeyValuePair<string, string>>();
    }
    
    // private readonly AuthStateProvider _authProvider;
    //
    // public CurrentUserService(AuthStateProvider authProvider)
    // {
    //     _authProvider = authProvider;
    // }
    //
    // public ClaimsPrincipal? AuthenticatedUser => _authProvider.AuthenticationStateUser;
    //
    // public Guid? UserId
    // {
    //     get
    //     {
    //         var isGuid = Guid.TryParse(AuthenticatedUser.FindFirstValue(ClaimTypes.NameIdentifier), out var userId);
    //         return isGuid ? userId : null;
    //     }
    // }
    //
    // public string? Username => AuthenticatedUser?.FindFirstValue(ClaimTypes.Name);
    //
    // public string? Email => AuthenticatedUser?.FindFirstValue(ClaimTypes.Email);
    //
    // public List<KeyValuePair<string, string>> Claims =>
    //     AuthenticatedUser?.Claims.Select(x => new KeyValuePair<string, string>(x.Type, x.Value)).ToList() ??
    //     new List<KeyValuePair<string, string>>();
}
using System.Security.Claims;

namespace Application.Services.Identity;

public interface ICurrentUserService
{
    // ClaimsPrincipal? AuthenticatedUser { get; }
    Guid? UserId { get; }
    string? Username { get; }
    string? Email { get; }
    public List<KeyValuePair<string, string>> Claims { get; }
}
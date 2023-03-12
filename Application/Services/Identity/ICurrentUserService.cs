using System.Security.Claims;
using Domain.DatabaseEntities.Identity;
using Domain.Models.Identity;
using Shared.Responses.Identity;

namespace Application.Services.Identity;

public interface ICurrentUserService
{
    Task<ClaimsPrincipal?> GetCurrentUserPrincipal();
    Task<Guid?> GetCurrentUserId();
    Guid GetIdFromPrincipal(ClaimsPrincipal principal);
    public Task<UserBasicResponse?> GetCurrentUserBasic();
    public Task<AppUserFull?> GetCurrentUserFull();
    public Task<AppUserDb?> GetCurrentUserDb();
}
using System.Security.Claims;
using Application.Models.Identity;
using Domain.DatabaseEntities.Identity;
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
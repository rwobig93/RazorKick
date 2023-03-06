using System.Security.Claims;
using Domain.DatabaseEntities.Identity;
using Shared.Responses.Identity;

namespace Application.Services.Identity;

public interface ICurrentUserService
{
    public Task<UserBasicResponse?> GetCurrentUserBasic();
    public Task<AppUserDb?> GetCurrentUserFull();
    ClaimsPrincipal? GetUserFromContext();
}
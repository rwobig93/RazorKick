using System.Security.Claims;
using Application.Repositories.Identity;
using Application.Services.Identity;
using Domain.DatabaseEntities.Identity;
using Shared.Responses.Identity;

namespace Infrastructure.Services.Identity;

public class CurrentUserService : ICurrentUserService
{
    private readonly AuthStateProvider _authProvider;
    private readonly IAppUserRepository _userRepository;
    
    public CurrentUserService(AuthStateProvider authProvider, IAppUserRepository userRepository)
    {
        _authProvider = authProvider;
        _userRepository = userRepository;
    }

    private async Task<Guid?> GetUserIdFromAuthProvider()
    {
        var userIdentity = await _authProvider.GetAuthenticationStateProviderUserAsync();
        var userIdClaim = userIdentity.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault();
        var isGuid = Guid.TryParse(userIdClaim?.Value, out var userId);
        
        if (!isGuid) return null;
        return userId;
    }
    
    public async Task<UserBasicResponse?> GetCurrentUserBasic()
    {
        var userId = await GetUserIdFromAuthProvider();
        
        if (userId is null) return null;

        var foundUser = await _userRepository.GetByIdAsync((Guid)userId);
        if (!foundUser.Success) return null;

        return foundUser.Result!.ToBasicResponse();
    }

    public async Task<AppUserDb?> GetCurrentUserFull()
    {
        var userId = await GetUserIdFromAuthProvider();
        
        if (userId is null) return null;

        var foundUser = await _userRepository.GetByIdAsync((Guid)userId);
        if (!foundUser.Success) return null;

        return foundUser.Result!;
    }
}
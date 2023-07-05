﻿using System.Security.Claims;
using Application.Mappers.Identity;
using Application.Models.Identity;
using Application.Repositories.Identity;
using Application.Responses.Identity;
using Application.Services.Identity;
using Domain.DatabaseEntities.Identity;
using Infrastructure.Services.Authentication;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services.Identity;

public class CurrentUserService : ICurrentUserService
{
    private readonly AuthStateProvider _authProvider;
    private readonly IAppUserRepository _userRepository;
    private readonly IHttpContextAccessor _contextAccessor;
    
    public CurrentUserService(AuthStateProvider authProvider, IAppUserRepository userRepository, IHttpContextAccessor contextAccessor)
    {
        _authProvider = authProvider;
        _userRepository = userRepository;
        _contextAccessor = contextAccessor;
    }

    public async Task<ClaimsPrincipal?> GetCurrentUserPrincipal()
    {
        return await _authProvider.GetAuthenticationStateProviderUserAsync();
    }

    public async Task<Guid?> GetCurrentUserId()
    {
        var userIdentity = await GetCurrentUserPrincipal();
        if (userIdentity is null)
            return null;
        
        var userId = GetIdFromPrincipal(userIdentity);
        
        if (userId == Guid.Empty) return null;
        return userId;
    }

    public Guid GetIdFromPrincipal(ClaimsPrincipal principal)
    {
        var userIdClaim = principal?.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault();
        var isGuid = Guid.TryParse(userIdClaim?.Value, out var userId);
        
        return !isGuid ? Guid.Empty : userId;
    }

    public async Task<UserBasicResponse?> GetCurrentUserBasic()
    {
        var userId = await GetCurrentUserId();
        
        if (userId is null) return null;

        var foundUser = await _userRepository.GetByIdAsync((Guid)userId);
        return !foundUser.Success ? null : foundUser.Result!.ToBasicResponse();
    }

    public async Task<AppUserFull?> GetCurrentUserFull()
    {
        var userId = await GetCurrentUserId();
        
        if (userId is null) return null;

        var foundUser = await _userRepository.GetByIdAsync((Guid)userId);
        return !foundUser.Success ? null : foundUser.Result!.ToFull();
    }

    public async Task<AppUserDb?> GetCurrentUserDb()
    {
        var userId = await GetCurrentUserId();
        
        if (userId is null) return null;

        var foundUser = await _userRepository.GetByIdAsync((Guid)userId);
        return !foundUser.Success ? null : foundUser.Result!;
    }
}
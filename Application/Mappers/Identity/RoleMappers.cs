﻿using Application.Helpers.Identity;
using Application.Models.Identity;
using Application.Requests.Identity.Role;
using Application.Responses.Identity;
using Domain.DatabaseEntities.Identity;

namespace Application.Mappers.Identity;

public static class RoleMappers
{
    public static AppRoleDb ToDb(this RoleResponse roleResponse)
    {
        return new AppRoleDb
        {
            Name = roleResponse.Name,
            Id = roleResponse.Id,
            Description = roleResponse.Description
        };
    }
    
    public static AppRoleCreate ToCreateObject(this AppRoleDb appRole)
    {
        return new AppRoleCreate
        {
            Name = appRole.Name,
            NormalizedName = appRole.NormalizedName,
            ConcurrencyStamp = appRole.ConcurrencyStamp,
            Description = appRole.Description,
            CreatedBy = appRole.CreatedBy,
            CreatedOn = appRole.CreatedOn,
            LastModifiedBy = appRole.LastModifiedBy,
            LastModifiedOn = appRole.LastModifiedOn
        };
    }

    public static AppRoleCreate ToCreateObject(this CreateRoleRequest createRole)
    {
        return new AppRoleCreate
        {
            Name = createRole.Name,
            NormalizedName = createRole.Name.NormalizeForDatabase(),
            ConcurrencyStamp = null,
            Description = createRole.Description,
            CreatedBy = default,
            CreatedOn = default,
            LastModifiedBy = null,
            LastModifiedOn = null
        };
    }
    
    public static AppRoleFull ToFull(this AppRoleDb roleDb)
    {
        return new AppRoleFull
        {
            Id = roleDb.Id,
            Name = roleDb.Name,
            Description = roleDb.Description,
            CreatedBy = roleDb.CreatedBy,
            CreatedOn = roleDb.CreatedOn,
            LastModifiedBy = roleDb.LastModifiedBy,
            LastModifiedOn = roleDb.LastModifiedOn
        };
    }

    public static AppRoleFull ToFull(this AppRoleSlim appRoleSlim)
    {
        return new AppRoleFull
        {
            Id = appRoleSlim.Id,
            Name = appRoleSlim.Name,
            Description = appRoleSlim.Description,
            CreatedBy = appRoleSlim.CreatedBy,
            CreatedOn = appRoleSlim.CreatedOn,
            LastModifiedBy = appRoleSlim.LastModifiedBy,
            LastModifiedOn = appRoleSlim.LastModifiedOn,
            Users = new List<AppUserSlim>(),
            Permissions = new List<AppPermissionSlim>()
        };
    }
    
    public static AppRoleSlim ToSlim(this AppRoleDb appRoleDb)
    {
        return new AppRoleSlim
        {
            Id = appRoleDb.Id,
            Name = appRoleDb.Name,
            Description = appRoleDb.Description,
            CreatedBy = appRoleDb.CreatedBy,
            CreatedOn = appRoleDb.CreatedOn,
            LastModifiedBy = appRoleDb.LastModifiedBy,
            LastModifiedOn = appRoleDb.LastModifiedOn
        };
    }

    public static IEnumerable<AppRoleSlim> ToSlims(this IEnumerable<AppRoleDb> appRoleDbs)
    {
        return appRoleDbs.Select(x => x.ToSlim());
    }
    
    public static RoleResponse ToResponse(this AppRoleSlim appRole)
    {
        return new RoleResponse
        {
            Id = appRole.Id,
            Name = appRole.Name ?? "",
            Description = appRole.Description ?? ""
        };
    }
    
    public static List<RoleResponse> ToResponses(this IEnumerable<AppRoleSlim> appRoles)
    {
        return appRoles.Select(x => x.ToResponse()).ToList();
    }
    
    public static RoleFullResponse ToFullResponse(this AppRoleSlim appRole)
    {
        return new RoleFullResponse
        {
            Id = appRole.Id,
            Name = appRole.Name ?? "",
            Description = appRole.Description ?? "",
            Permissions = new List<PermissionResponse>()
        };
    }
    
    public static List<RoleFullResponse> ToFullResponses(this IEnumerable<AppRoleSlim> appRoles)
    {
        return appRoles.Select(x => x.ToFullResponse()).ToList();
    }
    
    public static AppRoleUpdate ToObject(this AppRoleDb appRole)
    {
        return new AppRoleUpdate
        {
            Id = appRole.Id,
            Name = appRole.Name,
            NormalizedName = appRole.NormalizedName,
            ConcurrencyStamp = appRole.ConcurrencyStamp,
            Description = appRole.Description,
            CreatedBy = appRole.CreatedBy,
            CreatedOn = appRole.CreatedOn,
            LastModifiedBy = appRole.LastModifiedBy,
            LastModifiedOn = appRole.LastModifiedOn
        };
    }

    public static AppRoleUpdate ToUpdate(this UpdateRoleRequest roleRequest)
    {
        return new AppRoleUpdate
        {
            Id = roleRequest.Id,
            Name = roleRequest.Name,
            NormalizedName = roleRequest.Name!.NormalizeForDatabase(),
            ConcurrencyStamp = null,
            Description = roleRequest.Description,
            CreatedBy = default,
            CreatedOn = default,
            LastModifiedBy = null,
            LastModifiedOn = null
        };
    }

    public static AppRoleUpdate ToUpdate(this AppRoleFull roleFull)
    {
        return new AppRoleUpdate
        {
            Id = roleFull.Id,
            Name = roleFull.Name,
            NormalizedName = roleFull.Name!.NormalizeForDatabase(),
            ConcurrencyStamp = null,
            Description = roleFull.Description,
            CreatedBy = roleFull.CreatedBy,
            CreatedOn = roleFull.CreatedOn,
            LastModifiedBy = roleFull.LastModifiedBy,
            LastModifiedOn = roleFull.LastModifiedOn
        };
    }
}
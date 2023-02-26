﻿using Application.Helpers.Identity;
using Domain.DatabaseEntities.Identity;
using Shared.Requests.Identity;

namespace Application.Models.Identity;

public class AppRoleUpdate
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? NormalizedName { get; set; }
    public string? ConcurrencyStamp { get; set; }
    public string? Description { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid? LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }
}

public static class AppRoleUpdateExtensions
{
    public static AppRoleUpdate ToUpdateObject(this AppRoleDb appRole)
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
}
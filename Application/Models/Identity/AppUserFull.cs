﻿using Domain.Enums.Identity;

namespace Application.Models.Identity;

public class AppUserFull
{
    public Guid Id { get; set; }
    public string Username { get; set; } = null!;
    public string? EmailAddress { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public string? TwoFactorKey { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public Guid CreatedBy { get; set; }
    public string? ProfilePictureDataUrl { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid? LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedOn { get; set; }
    public AuthState AuthState { get; set; }
    public AccountType AccountType { get; set; } = AccountType.User;
    public List<AppRoleSlim> Roles { get; set; } = new();
    public List<AppUserExtendedAttributeSlim> ExtendedAttributes { get; set; } = new();
    public List<AppPermissionSlim> Permissions { get; set; } = new();
}
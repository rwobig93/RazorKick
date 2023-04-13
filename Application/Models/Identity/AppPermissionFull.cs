﻿namespace Application.Models.Identity;

public class AppPermissionFull
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public string ClaimType { get; set; } = "";
    public string ClaimValue { get; set; } = "";
    public string Name { get; set; } = "";
    public string Group { get; set; } = "";
    public string Access { get; set; } = "";
    public string Description { get; set; } = "";
    public Guid CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid? LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public List<AppUserSlim> Users { get; set; } = new();
    public List<AppRoleSlim> Roles { get; set; } = new();
}
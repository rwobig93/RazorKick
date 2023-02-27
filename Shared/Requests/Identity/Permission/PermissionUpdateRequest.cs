﻿namespace Shared.Requests.Identity.Permission;

public class PermissionUpdateRequest
{
    public new Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Group { get; set; }
    public string? Access { get; set; }
    public string? Description { get; set; }
}
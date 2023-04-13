using Domain.Contracts;
using Microsoft.AspNetCore.Identity;
using Shared.Responses.Identity;

namespace Domain.DatabaseEntities.Identity;

public class AppRoleDb : IdentityRole<Guid>, IAuditableEntity<Guid>
{
    public override Guid Id { get; set; }
    public string? Description { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid? LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }

    public override string ToString()
    {
        return Name;
    }
}

public static class AppRoleDbExtensions
{
    public static RoleResponse ToResponse(this AppRoleDb appRole)
    {
        return new RoleResponse
        {
            Id = appRole.Id,
            Name = appRole.Name,
            Description = appRole.Description ?? ""
        };
    }
    
    public static List<RoleResponse> ToResponses(this IEnumerable<AppRoleDb> appRoles)
    {
        return appRoles.Select(x => x.ToResponse()).ToList();
    }
    
    public static RoleFullResponse ToFullResponse(this AppRoleDb appRole)
    {
        return new RoleFullResponse
        {
            Id = appRole.Id,
            Name = appRole.Name,
            Description = appRole.Description ?? "",
            Permissions = new List<PermissionResponse>()
        };
    }
    
    public static List<RoleFullResponse> ToFullResponses(this IEnumerable<AppRoleDb> appRoles)
    {
        return appRoles.Select(x => x.ToFullResponse()).ToList();
    }

    public static AppRoleDb ToDb(this RoleResponse roleResponse)
    {
        return new AppRoleDb
        {
            Name = roleResponse.Name,
            Id = roleResponse.Id,
            Description = roleResponse.Description
        };
    }
}
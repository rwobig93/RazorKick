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
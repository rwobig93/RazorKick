using Application.Helpers.Identity;
using Domain.DatabaseEntities.Identity;
using Shared.Requests.Identity;

namespace Application.Models.Identity;

public class AppRoleCreate
{
    public string Name { get; set; } = null!;
    public string NormalizedName { get; set; } = null!;
    public string? ConcurrencyStamp { get; set; }
    public string? Description { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid? LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }
}

public static class AppRoleCreateExtensions
{
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
            Name = createRole.Name!,
            NormalizedName = createRole.Name!.NormalizeForDatabase(),
            ConcurrencyStamp = null,
            Description = createRole.Description,
            CreatedBy = default,
            CreatedOn = default,
            LastModifiedBy = null,
            LastModifiedOn = null
        };
    }
}
using Domain.Enums.Identity;

namespace Application.Models.Identity;

public class AppUserExtendedAttributeCreate
{
    public Guid OwnerId { get; set; }
    public string Name { get; set; } = "";
    public string Value { get; set; } = "";
    public ExtendedAttributeType Type { get; set; }
}
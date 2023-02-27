using Shared.Enums.Identity;

namespace Shared.Requests.Identity.User;

public class GetUserExtendedAttributesByOwnerIdAndType
{
    public Guid Id { get; set; }
    public AttributeType Type { get; set; }
}
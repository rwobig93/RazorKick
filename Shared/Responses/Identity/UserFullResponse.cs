using Shared.Enums.Identity;

namespace Shared.Responses.Identity;

public class UserFullResponse
{
    public Guid Id { get; set; }
    public string Username { get; set; } = null!;
    public DateTime CreatedOn { get; set; }
    public bool IsActive { get; set; }
    public AccountType AccountType { get; init; } = AccountType.User;
    public List<ExtendedAttributeResponse> ExtendedAttributes { get; set; } = new();
    public List<PermissionResponse> Permissions { get; set; } = new();
}
using Domain.Enums.Identity;

namespace Application.Responses.Identity;

public class UserFullResponse
{
    public Guid Id { get; set; }
    public string Username { get; set; } = null!;
    public DateTime CreatedOn { get; set; }
    public AuthState AuthState { get; set; }
    public AccountType AccountType { get; init; } = AccountType.User;
    public List<ExtendedAttributeResponse> ExtendedAttributes { get; set; } = new();
    public List<PermissionResponse> Permissions { get; set; } = new();
}
using Shared.Enums.Identity;

namespace Shared.Responses.Identity;

public class UserBasicResponse
{
    public Guid Id { get; set; }
    public string Username { get; set; } = null!;
    public DateTime CreatedOn { get; set; }
    public bool IsEnabled { get; set; }
    public AccountType AccountType { get; init; } = AccountType.User;
}
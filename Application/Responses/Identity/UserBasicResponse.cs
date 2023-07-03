using Domain.Enums.Identity;

namespace Application.Responses.Identity;

public class UserBasicResponse
{
    public Guid Id { get; set; }
    public string Username { get; set; } = null!;
    public DateTime CreatedOn { get; set; }
    public AuthState AuthState { get; set; }
    public AccountType AccountType { get; init; } = AccountType.User;
}
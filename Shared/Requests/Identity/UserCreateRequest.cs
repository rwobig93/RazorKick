using Shared.Enums.Identity;

namespace Shared.Requests.Identity;

public class UserCreateRequest
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName {get;set;} = string.Empty;
    public string LastName {get;set;} = string.Empty;
    public string UserName {get;set;} = string.Empty;
    public bool EmailConfirmed {get;set;}
    public string PhoneNumber { get; set; } = string.Empty;
    public bool PhoneNumberConfirmed {get;set;}
    public bool IsActive {get;set;}
    public AccountType AccountType {get;set;} = AccountType.User;
}
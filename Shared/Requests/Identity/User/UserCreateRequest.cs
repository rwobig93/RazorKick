﻿using Shared.Enums.Identity;

namespace Shared.Requests.Identity.User;

public class UserCreateRequest
{
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string FirstName {get;set;} = null!;
    public string LastName {get;set;} = null!;
    public bool EmailConfirmed {get;set;}
    public string PhoneNumber { get; set; } = null!;
    public bool PhoneNumberConfirmed {get;set;}
    public bool IsActive {get;set;}
    public AccountType AccountType {get;set;} = AccountType.User;
}
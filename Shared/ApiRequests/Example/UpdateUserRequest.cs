﻿namespace Shared.ApiRequests.Example;

public class UpdateUserRequest
{
    public int Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}
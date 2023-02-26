using Shared.Requests.Example;

namespace Application.Models.Example;

public class ExamplePermissionUpdate
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Value { get; set; }
}

public static class ExamplePermissionUpdateExtensions
{
    public static ExamplePermissionUpdateRequest ToUpdateRequest(this ExamplePermissionUpdate updateRequest)
    {
        return new ExamplePermissionUpdateRequest
        {
            Id = updateRequest.Id,
            Name = updateRequest.Name,
            Value = updateRequest.Value
        };
    }
    
    public static ExamplePermissionUpdate ToUpdate(this ExamplePermissionUpdateRequest updateRequest)
    {
        return new ExamplePermissionUpdate
        {
            Id = updateRequest.Id,
            Name = updateRequest.Name,
            Value = updateRequest.Value
        };
    }
}
using Shared.Requests.Example;

namespace Application.Models.Example;

public class ExamplePermissionCreate
{
    public string Name { get; set; } = null!;
    public string Value { get; set; } = null!;
}

public static class ExamplePermissionCreateExtensions
{
    public static ExamplePermissionCreateRequest ToCreateRequest(this ExamplePermissionCreate createRequest)
    {
        return new ExamplePermissionCreateRequest
        {
            Name = createRequest.Name,
            Value = createRequest.Value
        };
    }
    
    public static ExamplePermissionCreate ToCreate(this ExamplePermissionCreateRequest createRequest)
    {
        return new ExamplePermissionCreate
        {
            Name = createRequest.Name,
            Value = createRequest.Value
        };
    }
}
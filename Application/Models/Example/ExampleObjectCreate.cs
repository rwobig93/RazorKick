using Shared.Requests.Example;

namespace Application.Models.Example;

public class ExampleObjectCreate
{
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
}

public static class ExampleObjectsCreateExtensions
{
    public static ExampleObjectCreate ToObjectCreate(this ExampleObjectCreateRequest createRequest)
    {
        return new ExampleObjectCreate
        {
            FirstName = createRequest.FirstName,
            LastName = createRequest.LastName
        };
    }
}
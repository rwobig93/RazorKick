using Shared.Requests.Example;

namespace Application.Models.Example;

public class ExampleObjectUpdate
{
    public Guid Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}

public static class ExampleObjectsUpdateExtensions
{
    public static ExampleObjectUpdate ToObjectUpdate(this ExampleObjectUpdateRequest updateRequest)
    {
        return new ExampleObjectUpdate
        {
            Id = updateRequest.Id,
            FirstName = updateRequest.FirstName,
            LastName = updateRequest.LastName
        };
    }
}
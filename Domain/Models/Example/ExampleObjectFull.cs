using Domain.DatabaseEntities.Example;
using Shared.Responses.Example;

namespace Domain.Models.Example;

public class ExampleObjectFull
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public List<ExampleExtendedAttributeDb> ExtendedAttributes { get; set; } = new();
    public List<ExamplePermissionDb> Permissions { get; set; } = new();
}

public static class ExampleObjectFullExtensions
{
    public static ExampleObjectFullResponse ToFullResponse(this ExampleObjectFull fullObject)
    {
        return new ExampleObjectFullResponse
        {
            Id = fullObject.Id,
            FirstName = fullObject.FirstName,
            LastName = fullObject.LastName,
            ExtendedAttributes = fullObject.ExtendedAttributes.ToResponses(),
            Permissions = fullObject.Permissions.ToResponses()
        };
    }
}
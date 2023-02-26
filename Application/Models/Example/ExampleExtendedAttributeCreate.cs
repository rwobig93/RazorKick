using Domain.DatabaseEntities.Example;
using Domain.Enums.Example;
using Shared.Requests.Example;

namespace Application.Models.Example;

public class ExampleExtendedAttributeCreate
{
    public Guid AssignedTo { get; set; }
    public string Name { get; set; } = "";
    public string Value { get; set; } = "";
    public ExampleExtendedAttributeType Type { get; set; }
}

public static class ExampleExtendedAttributeCreateExtensions
{
    public static ExampleExtendedAttributeCreateRequest ToCreateRequest(this ExampleExtendedAttributeCreate createRequest)
    {
        return new ExampleExtendedAttributeCreateRequest
        {
            UserId = createRequest.AssignedTo,
            Name = createRequest.Name,
            Value = createRequest.Value,
            Type = (Shared.Enums.Example.ExampleExtendedAttributeType)createRequest.Type
        };
    }
    
    public static ExampleExtendedAttributeCreate ToCreate(this ExampleExtendedAttributeCreateRequest createRequest)
    {
        return new ExampleExtendedAttributeCreate
        {
            AssignedTo = createRequest.UserId,
            Name = createRequest.Name,
            Value = createRequest.Value,
            Type = (ExampleExtendedAttributeType)createRequest.Type
        };
    }
}
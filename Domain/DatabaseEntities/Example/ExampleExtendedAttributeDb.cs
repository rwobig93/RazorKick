using Domain.Enums.Example;
using Shared.Requests.Example;
using Shared.Responses.Example;

namespace Domain.DatabaseEntities.Example;

public class ExampleExtendedAttributeDb
{
    public Guid Id { get; set; }
    public Guid AssignedTo { get; set; }
    public string Name { get; set; } = "";
    public string Value { get; set; } = "";
    public ExampleExtendedAttributeType Type { get; set; }
}

public static class ExampleExtendedAttributeDbExtensions
{
    public static ExampleExtendedAttributeResponse ToResponse(this ExampleExtendedAttributeDb exampleAttribute)
    {
        return new ExampleExtendedAttributeResponse
        {
            Id = exampleAttribute.Id,
            Name = exampleAttribute.Name,
            Value = exampleAttribute.Value
        };
    }
    
    public static List<ExampleExtendedAttributeResponse> ToResponses(this IEnumerable<ExampleExtendedAttributeDb> exampleAttributes)
    {
        return exampleAttributes.Select(x => x.ToResponse()).ToList();
    }
    
    public static ExampleExtendedAttributeDb ToDb(this ExampleExtendedAttributeResponse exampleAttribute)
    {
        return new ExampleExtendedAttributeDb
        {
            Id = exampleAttribute.Id,
            Name = exampleAttribute.Name,
            Value = exampleAttribute.Value
        };
    }
}
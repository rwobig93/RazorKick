using Domain.Models.Example;
using Shared.Responses.Example;

namespace Domain.DatabaseEntities.Example;

public class ExampleObjectDb
{
    public Guid Id { get; init; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
}

public static class ExampleObjectDbExtensions
{
    public static ExampleObjectFull ToFullObject(this ExampleObjectDb exampleObject)
    {
        return new ExampleObjectFull
        {
            Id = exampleObject.Id,
            FirstName = exampleObject.FirstName,
            LastName = exampleObject.LastName,
            ExtendedAttributes = new List<ExampleExtendedAttributeDb>(),
            Permissions = new List<ExamplePermissionDb>()
        };
    }
    
    public static ExampleObjectResponse ToResponse(this ExampleObjectDb exampleObject)
    {
        return new ExampleObjectResponse
        {
            Id = exampleObject.Id,
            FirstName = exampleObject.FirstName,
            LastName = exampleObject.LastName
        };
    }

    public static List<ExampleObjectResponse> ToResponses(this IEnumerable<ExampleObjectDb> objectList)
    {
        return objectList.Select(dbObject => dbObject.ToResponse()).ToList();
    }
}
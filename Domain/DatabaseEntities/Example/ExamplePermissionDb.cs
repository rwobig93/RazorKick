using Shared.Responses.Example;

namespace Domain.DatabaseEntities.Example;

public class ExamplePermissionDb
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string Value { get; set; } = "";
}

public static class ExamplePermissionDbExtensions
{
    public static ExamplePermissionResponse ToResponse(this ExamplePermissionDb examplePermission)
    {
        return new ExamplePermissionResponse
        {
            Id = examplePermission.Id,
            Name = examplePermission.Name,
            Value = examplePermission.Value
        };
    }

    public static List<ExamplePermissionResponse> ToResponses(this IEnumerable<ExamplePermissionDb> objectList)
    {
        return objectList.Select(dbObject => dbObject.ToResponse()).ToList();
    }
}
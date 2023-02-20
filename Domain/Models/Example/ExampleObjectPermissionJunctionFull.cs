using Domain.DatabaseEntities.Example;

namespace Domain.Models.Example;

public class ExampleObjectPermissionJunctionFull
{
    public ExampleObjectDb ExampleObject { get; set; } = null!;
    public ExamplePermissionDb ExamplePermission { get; set; } = null!;
}
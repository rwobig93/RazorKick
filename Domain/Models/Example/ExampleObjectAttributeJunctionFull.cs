using Domain.DatabaseEntities.Example;
using Domain.Entities.Example;

namespace Domain.Models.Example;

public class ExampleObjectAttributeJunctionFull
{
    public ExampleObjectDb ExampleObject { get; set; } = null!;
    public ExampleExtendedAttributeDb ExampleAttribute { get; set; } = null!;
}
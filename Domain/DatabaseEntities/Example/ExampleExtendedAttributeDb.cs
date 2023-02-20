using Domain.Enums.Example;

namespace Domain.DatabaseEntities.Example;

public class ExampleExtendedAttributeDb
{
    public Guid Id { get; set; }
    public Guid AssignedTo { get; set; }
    public string Name { get; set; } = "";
    public string Value { get; set; } = "";
    public ExampleExtendedAttributeType Type { get; set; }
}
using Domain.Enums.Example;

namespace Application.Models.Example;

public class ExampleExtendedAttributeCreate
{
    public Guid AssignedTo { get; set; }
    public string Name { get; set; } = "";
    public string Value { get; set; } = "";
    public ExampleExtendedAttributeType Type { get; set; }
}
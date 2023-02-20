using Domain.DatabaseEntities.Example;

namespace Domain.Models.Example;

public class ExampleObjectFull
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public List<ExampleExtendedAttributeDb> ExtendedAttributes { get; set; } = new();
    public List<ExamplePermissionDb> Permissions { get; set; } = new();
}
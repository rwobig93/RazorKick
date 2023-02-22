namespace Application.Models.Example;

public class ExamplePermissionCreate
{
    public Guid Id { get; set; }
    public Guid AssignedTo { get; set; }
    public string Name { get; set; } = null!;
    public string Value { get; set; } = null!;
}
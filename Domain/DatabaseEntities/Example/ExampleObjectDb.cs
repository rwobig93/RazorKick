namespace Domain.DatabaseEntities.Example;

public class ExampleObjectDb
{
    public Guid Id { get; init; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
}
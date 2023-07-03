namespace Domain.DatabaseEntities.Example;

public class BookDb
{
    public Guid Id { get; init; }
    public string Name { get; set; } = null!;
    public string Author { get; set; } = null!;
    public int Pages { get; set; }
}
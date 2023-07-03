namespace Application.Requests.Example;

public class BookCreateRequest
{
    public string Name { get; set; } = null!;
    public string Author { get; set; } = null!;
    public int Pages { get; set; }
}
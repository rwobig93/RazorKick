namespace Shared.Requests.Example;

public class BookUpdateRequest
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Author { get; set; }
    public int? Pages { get; set; }
}
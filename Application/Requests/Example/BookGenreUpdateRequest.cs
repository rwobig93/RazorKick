namespace Application.Requests.Example;

public class BookGenreUpdateRequest
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Value { get; set; }
}
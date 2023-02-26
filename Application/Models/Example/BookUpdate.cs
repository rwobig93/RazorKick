using Shared.Requests.Example;

namespace Application.Models.Example;

public class BookUpdate
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Author { get; set; }
    public int? Pages { get; set; }
}

public static class ExampleObjectsUpdateExtensions
{
    public static BookUpdate ToRequest(this BookUpdateRequest bookUpdate)
    {
        return new BookUpdate
        {
            Id = bookUpdate.Id,
            Name = bookUpdate.Name,
            Author = bookUpdate.Author,
            Pages = bookUpdate.Pages
        };
    }
}
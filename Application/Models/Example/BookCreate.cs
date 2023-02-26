using Shared.Requests.Example;

namespace Application.Models.Example;

public class BookCreate
{
    public string Name { get; set; } = null!;
    public string Author { get; set; } = null!;
    public int Pages { get; set; }
}

public static class ExampleObjectsCreateExtensions
{
    public static BookCreate ToCreate(this BookCreateRequest createRequest)
    {
        return new BookCreate
        {
            Name = createRequest.Name,
            Author = createRequest.Author,
            Pages = createRequest.Pages
        };
    }
}
using Domain.DatabaseEntities.Example;
using Shared.Responses.Example;

namespace Domain.Models.Example;

public class BookFullDb
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Author { get; set; } = null!;
    public int Pages { get; set; }
    public List<BookReviewDb> Reviews { get; set; } = new();
    public List<BookGenreDb> Genres { get; set; } = new();
}

public static class ExampleObjectFullExtensions
{
    public static BookFullResponse ToFullResponse(this BookFullDb bookFull)
    {
        return new BookFullResponse
        {
            Id = bookFull.Id,
            Name = bookFull.Name,
            Author = bookFull.Author,
            Pages = bookFull.Pages,
            Reviews = bookFull.Reviews.ToResponses(),
            Genres = bookFull.Genres.ToResponses()
        };
    }
}
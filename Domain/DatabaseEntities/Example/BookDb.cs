using Domain.Models.Example;
using Shared.Responses.Example;

namespace Domain.DatabaseEntities.Example;

public class BookDb
{
    public Guid Id { get; init; }
    public string Name { get; set; } = null!;
    public string Author { get; set; } = null!;
    public int Pages { get; set; }
}

public static class BookDbExtensions
{
    public static BookFullDb ToFullObject(this BookDb book)
    {
        return new BookFullDb
        {
            Id = book.Id,
            Name = book.Name,
            Author = book.Author,
            Pages = book.Pages,
            Reviews = new List<BookReviewDb>(),
            Genres = new List<BookGenreDb>()
        };
    }
    
    public static BookResponse ToResponse(this BookDb book)
    {
        return new BookResponse
        {
            Id = book.Id,
            Name = book.Name,
            Author = book.Author,
            Pages = book.Pages
        };
    }

    public static List<BookResponse> ToResponses(this IEnumerable<BookDb> bookList)
    {
        return bookList.Select(dbObject => dbObject.ToResponse()).ToList();
    }
}
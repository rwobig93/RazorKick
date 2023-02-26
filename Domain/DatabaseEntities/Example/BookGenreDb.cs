using Shared.Responses.Example;

namespace Domain.DatabaseEntities.Example;

public class BookGenreDb
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string Value { get; set; } = "";
}

public static class BookGenreDbExtensions
{
    public static BookGenreResponse ToResponse(this BookGenreDb bookGenre)
    {
        return new BookGenreResponse
        {
            Id = bookGenre.Id,
            Name = bookGenre.Name,
            Value = bookGenre.Value
        };
    }

    public static List<BookGenreResponse> ToResponses(this IEnumerable<BookGenreDb> genreList)
    {
        return genreList.Select(dbObject => dbObject.ToResponse()).ToList();
    }
}
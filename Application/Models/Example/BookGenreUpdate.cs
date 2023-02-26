using Shared.Requests.Example;

namespace Application.Models.Example;

public class BookGenreUpdate
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Value { get; set; }
}

public static class BookGenreUpdateExtensions
{
    public static BookGenreUpdateRequest ToUpdateRequest(this BookGenreUpdate genreUpdate)
    {
        return new BookGenreUpdateRequest
        {
            Id = genreUpdate.Id,
            Name = genreUpdate.Name,
            Value = genreUpdate.Value
        };
    }
    
    public static BookGenreUpdate ToUpdate(this BookGenreUpdateRequest genreRequest)
    {
        return new BookGenreUpdate
        {
            Id = genreRequest.Id,
            Name = genreRequest.Name,
            Value = genreRequest.Value
        };
    }
}
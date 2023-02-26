using Shared.Requests.Example;

namespace Application.Models.Example;

public class BookGenreCreate
{
    public string Name { get; set; } = null!;
    public string Value { get; set; } = null!;
}

public static class ExamplePermissionCreateExtensions
{
    public static BookGenreCreateRequest ToCreateRequest(this BookGenreCreate createRequest)
    {
        return new BookGenreCreateRequest
        {
            Name = createRequest.Name,
            Value = createRequest.Value
        };
    }
    
    public static BookGenreCreate ToCreate(this BookGenreCreateRequest createRequest)
    {
        return new BookGenreCreate
        {
            Name = createRequest.Name,
            Value = createRequest.Value
        };
    }
}
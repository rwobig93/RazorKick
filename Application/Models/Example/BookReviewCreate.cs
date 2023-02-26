using Shared.Requests.Example;

namespace Application.Models.Example;

public class BookReviewCreate
{
    public Guid BookId { get; set; }
    public string Author { get; set; } = null!;
    public string Content { get; set; } = null!;
}

public static class BookReviewCreateExtensions
{
    public static BookReviewCreateRequest ToCreateRequest(this BookReviewCreate reviewCreate)
    {
        return new BookReviewCreateRequest
        {
            BookId = reviewCreate.BookId,
            Author = reviewCreate.Author,
            Content = reviewCreate.Content
        };
    }
    
    public static BookReviewCreate ToCreate(this BookReviewCreateRequest reviewRequest)
    {
        return new BookReviewCreate
        {
            BookId = reviewRequest.BookId,
            Author = reviewRequest.Author,
            Content = reviewRequest.Content
        };
    }
}
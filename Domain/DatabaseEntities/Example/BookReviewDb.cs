using Shared.Responses.Example;

namespace Domain.DatabaseEntities.Example;

public class BookReviewDb
{
    public Guid Id { get; set; }
    public Guid BookId { get; set; }
    public string Author { get; set; } = "";
    public string Content { get; set; } = "";
    public bool IsDeleted { get; set; }
}

public static class BookReviewDbExtensions
{
    public static BookReviewResponse ToResponse(this BookReviewDb bookReview)
    {
        return new BookReviewResponse
        {
            Id = bookReview.Id,
            BookId = bookReview.BookId,
            Author = bookReview.Author,
            Content = bookReview.Content
        };
    }
    
    public static List<BookReviewResponse> ToResponses(this IEnumerable<BookReviewDb> bookReviews)
    {
        return bookReviews.Select(x => x.ToResponse()).ToList();
    }
    
    public static BookReviewDb ToDb(this BookReviewResponse bookReviewResponse)
    {
        return new BookReviewDb
        {
            Id = bookReviewResponse.Id,
            BookId = bookReviewResponse.BookId,
            Author = bookReviewResponse.Author,
            Content = bookReviewResponse.Content
        };
    }
}
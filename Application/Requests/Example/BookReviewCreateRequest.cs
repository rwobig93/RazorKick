namespace Application.Requests.Example;

public class BookReviewCreateRequest
{
    public Guid BookId { get; set; }
    public string Author { get; set; } = null!;
    public string Content { get; set; } = null!;
}
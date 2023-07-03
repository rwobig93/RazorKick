namespace Domain.DatabaseEntities.Example;

public class BookReviewDb
{
    public Guid Id { get; set; }
    public Guid BookId { get; set; }
    public string Author { get; set; } = "";
    public string Content { get; set; } = "";
    public bool IsDeleted { get; set; }
}
using Domain.DatabaseEntities.Example;

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
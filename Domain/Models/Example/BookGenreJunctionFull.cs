using Domain.DatabaseEntities.Example;

namespace Domain.Models.Example;

public class BookGenreJunctionFull
{
    public BookDb Book { get; set; } = null!;
    public BookGenreDb Genre { get; set; } = null!;
}
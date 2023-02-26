using Application.Models.Example;
using Domain.DatabaseEntities.Example;
using Domain.Models.Example;

namespace Application.Repositories.Example;

public interface IBookGenreRepository
{
    Task<List<BookGenreDb>> GetAll();
    Task<Guid?> Create(BookGenreCreate genreCreate);
    Task<BookGenreDb?> GetById(Guid id);
    Task<BookGenreDb?> GetByName(string genreName);
    Task<BookGenreDb?> GetByValue(string genreValue);
    Task Update(BookGenreUpdate genreUpdate);
    Task Delete(Guid id);
    Task AddToBook(Guid bookId, Guid genreId);
    Task RemoveFromBook(Guid bookId, Guid genreId);
    Task<List<BookGenreDb>> GetGenresForBook(Guid bookId);
    Task<List<BookDb>> GetBooksWithGenre(Guid genreId);
    Task<List<BookGenreJunctionFull>> GetAllBookGenreMappings();
}
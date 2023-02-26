using Application.Models.Example;
using Domain.DatabaseEntities.Example;
using Domain.Models.Database;
using Domain.Models.Example;

namespace Application.Repositories.Example;

public interface IBookGenreRepository
{
    Task<DatabaseActionResult<List<BookGenreDb>>> GetAllAsync();
    Task<DatabaseActionResult<Guid?>> CreateAsync(BookGenreCreate genreCreate);
    Task<DatabaseActionResult<BookGenreDb?>> GetByIdAsync(Guid id);
    Task<DatabaseActionResult<BookGenreDb?>> GetByNameAsync(string genreName);
    Task<DatabaseActionResult<BookGenreDb?>> GetByValueAsync(string genreValue);
    Task<DatabaseActionResult> UpdateAsync(BookGenreUpdate genreUpdate);
    Task<DatabaseActionResult> DeleteAsync(Guid id);
    Task<DatabaseActionResult> AddToBookAsync(Guid bookId, Guid genreId);
    Task<DatabaseActionResult> RemoveFromBookAsync(Guid bookId, Guid genreId);
    Task<DatabaseActionResult<List<BookGenreDb>>> GetGenresForBookAsync(Guid bookId);
    Task<DatabaseActionResult<List<BookDb>>> GetBooksWithGenreAsync(Guid genreId);
    Task<DatabaseActionResult<List<BookGenreJunctionFull>>> GetAllBookGenreMappingsAsync();
}
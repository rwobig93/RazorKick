using Application.Models.Example;
using Domain.DatabaseEntities.Example;
using Domain.Models.Database;
using Domain.Models.Example;

namespace Application.Repositories.Example;

public interface IBookRepository
{
    Task<DatabaseActionResult<List<BookDb>>> GetAllAsync();
    Task<DatabaseActionResult<Guid?>> CreateAsync(BookCreate createBook);
    Task<DatabaseActionResult<BookDb?>> GetByIdAsync(Guid id);
    Task<DatabaseActionResult<BookFull?>> GetFullByIdAsync(Guid id);
    Task<DatabaseActionResult> UpdateAsync(BookUpdate updateBook);
    Task<DatabaseActionResult> DeleteAsync(Guid id);
}
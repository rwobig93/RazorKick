using Application.Models.Example;
using Domain.DatabaseEntities.Example;
using Domain.Models.Database;

namespace Application.Repositories.Example;

public interface IBookReviewRepository
{
    Task<DatabaseActionResult<List<BookReviewDb>>> GetAllAsync();
    Task<DatabaseActionResult<Guid?>> CreateAsync(BookReviewCreate createObject);
    Task<DatabaseActionResult<BookReviewDb?>> GetByIdAsync(Guid id);
    Task<DatabaseActionResult> UpdateAsync(BookReviewUpdate objectUpdate);
    Task<DatabaseActionResult> DeleteAsync(Guid id);
    Task<DatabaseActionResult<List<BookReviewDb>>> GetReviewsForBookAsync(Guid bookId);
    Task<DatabaseActionResult<List<BookReviewDb>>> GetReviewsFromAuthorAsync(string author);
}
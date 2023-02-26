using Application.Models.Example;
using Domain.DatabaseEntities.Example;

namespace Application.Repositories.Example;

public interface IBookReviewRepository
{
    Task<List<BookReviewDb>> GetAll();
    Task<Guid?> Create(BookReviewCreate createObject);
    Task<BookReviewDb?> Get(Guid id);
    Task Update(BookReviewUpdate objectUpdate);
    Task Delete(Guid id);
    Task<List<BookReviewDb>> GetReviewsForBook(Guid bookId);
    Task<List<BookReviewDb>> GetReviewsFromAuthor(string author);
}
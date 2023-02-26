using Application.Models.Example;
using Domain.DatabaseEntities.Example;
using Domain.Models.Example;

namespace Application.Repositories.Example;

public interface IBookRepository
{
    Task<List<BookDb>> GetAll();
    Task<Guid?> Create(BookCreate createBook);
    Task<BookDb?> Get(Guid id);
    Task<BookFull?> GetFull(Guid id);
    Task Update(BookUpdate updateBook);
    Task Delete(Guid id);
}
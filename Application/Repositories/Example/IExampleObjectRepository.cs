using Application.Models.Example;
using Domain.DatabaseEntities.Example;
using Domain.Models.Example;

namespace Application.Repositories.Example;

public interface IExampleObjectRepository
{
    Task<List<ExampleObjectDb>> GetAll();
    Task CreateRandom();
    Task<Guid?> Create(ExampleObjectCreate createObject);
    Task<ExampleObjectDb?> Get(Guid id);
    Task<ExampleObjectFull?> GetFull(Guid id);
    Task Update(ExampleObjectUpdate objectUpdate);
    Task Delete(Guid id);
}
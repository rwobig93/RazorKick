using Shared.Requests.Example;
using Shared.Responses.Example;

namespace Application.Repositories.Example;

public interface IExampleObjectRepository
{
    Task<List<ExampleObjectResponse>> GetAll();
    Task CreateRandom();
    Task<Guid?> Create(string firstName, string lastName);
    Task<ExampleObjectResponse> Get(Guid id);
    Task<ExampleObjectFullResponse> GetFull(Guid id);
    Task Update(ExampleObjectUpdateRequest objectUpdate);
    Task Delete(Guid id);
}
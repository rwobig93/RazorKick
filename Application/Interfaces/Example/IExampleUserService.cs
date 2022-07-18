using Domain.Entities.Example;
using Shared.Requests.Example;

namespace Application.Interfaces.Example;

public interface IExampleUserService
{
    Task<IEnumerable<ExampleUser>> GetAllUsers();
    Task<ExampleUser?> GetUser(GetExampleUserRequest userRequest);
    Task CreateUser(CreateExampleUserRequest userRequest);
    Task UpdateUser(UpdateExampleUserRequest userRequest);
    Task DeleteUser(DeleteExampleUserRequest userRequest);
}
using Domain.Entities.Example;
using Shared.ApiRequests.Example;

namespace Application.Interfaces.Example;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllUsers();
    Task<User?> GetUser(GetUserRequest userRequest);
    Task CreateUser(CreateUserRequest userRequest);
    Task UpdateUser(UpdateUserRequest userRequest);
    Task DeleteUser(DeleteUserRequest userRequest);
}
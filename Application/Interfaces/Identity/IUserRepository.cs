using Domain.Entities.Identity;
using Shared.Requests.Identity;

namespace Application.Interfaces.Identity;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllUsers();
    Task<User?> GetUser(GetUserRequest userRequest);
    Task RegisterUser(UserRegisterRequest userRequest);
    Task UpdateUser(UpdateUserRequest userRequest);
    Task DeleteUser(DeleteUserRequest userRequest);
}
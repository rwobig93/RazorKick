using Application.Extensibility.Extensions;
using Application.Interfaces.Database;
using Application.Interfaces.Identity;
using Domain.Entities.Identity;
using Shared.Requests.Identity;
using static Application.Constants.Database.MsSqlConstants.StoredProcedures;

namespace Infrastructure.Features.Identity;

public class UserRepository : IUserRepository
{
    private readonly ISqlDataAccess _database;

    public UserRepository(ISqlDataAccess database)
    {
        _database = database;
    }

    public Task<IEnumerable<User>> GetAllUsers() =>
        _database.LoadData<User, dynamic>(UserGetAll, new { });

    public async Task<User?> GetUser(GetUserRequest userRequest) =>
        (await _database.LoadData<User, dynamic>(UserGet.DboFullName(), userRequest)).FirstOrDefault();

    public Task RegisterUser(UserRegisterRequest userRequest) =>
        _database.SaveData(UserCreate.DboFullName(), userRequest);

    public Task UpdateUser(UpdateUserRequest userRequest) =>
        _database.SaveData(UserUpdate.DboFullName(), userRequest);

    public Task DeleteUser(DeleteUserRequest userRequest) =>
        _database.SaveData(UserDelete.DboFullName(), userRequest);
}
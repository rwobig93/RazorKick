using Application.Extensibility.Extensions;
using Application.Interfaces.Database;
using Application.Interfaces.Example;
using Domain.Entities.Example;
using Shared.ApiRequests.Example;
using static Application.Constants.Database.MsSqlConstants.StoredProcedures;

namespace Infrastructure.Services.Example;

public class UserRepository : IUserRepository
{
    private readonly ISqlDataAccess _database;

    public UserRepository(ISqlDataAccess database)
    {
        _database = database;
    }

    public Task<IEnumerable<User>> GetAllUsers() =>
        _database.LoadData<User, dynamic>(UserGetAll.DboFullName(), new { });

    public async Task<User?> GetUser(GetUserRequest userRequest) =>
        (await _database.LoadData<User, dynamic>(UserGet.DboFullName(), userRequest)).FirstOrDefault();

    public Task CreateUser(CreateUserRequest userRequest) =>
        _database.SaveData(UserCreate.DboFullName(), userRequest);

    public Task UpdateUser(UpdateUserRequest userRequest) =>
        _database.SaveData(UserUpdate.DboFullName(), userRequest);

    public Task DeleteUser(DeleteUserRequest userRequest) =>
        _database.SaveData(UserDelete.DboFullName(), userRequest);
}
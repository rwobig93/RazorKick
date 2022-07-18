using Application.Extensibility.Extensions;
using Application.Interfaces.Database;
using Application.Interfaces.Example;
using Domain.Entities.Example;
using Shared.Requests.Example;
using static Application.Constants.Database.MsSqlConstants.StoredProcedures;

namespace Infrastructure.Services.Example;

public class ExampleUserService : IExampleUserService
{
    private readonly ISqlDataService _database;

    public ExampleUserService(ISqlDataService database)
    {
        _database = database;
    }

    public Task<IEnumerable<ExampleUser>> GetAllUsers() =>
        _database.LoadData<ExampleUser, dynamic>(ExampleUserGetAll.ToDboName(), new { });

    public async Task<ExampleUser?> GetUser(GetExampleUserRequest userRequest) =>
        (await _database.LoadData<ExampleUser, dynamic>(ExampleUserGet.ToDboName(), userRequest)).FirstOrDefault();

    public Task CreateUser(CreateExampleUserRequest userRequest) =>
        _database.SaveData(ExampleUserCreate.ToDboName(), userRequest);

    public Task UpdateUser(UpdateExampleUserRequest userRequest) =>
        _database.SaveData(ExampleUserUpdate.ToDboName(), userRequest);

    public Task DeleteUser(DeleteExampleUserRequest userRequest) =>
        _database.SaveData(ExampleUserDelete.ToDboName(), userRequest);
}
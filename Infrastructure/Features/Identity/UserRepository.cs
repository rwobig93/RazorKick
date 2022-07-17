using Application.Extensibility.Extensions;
using Application.Extensibility.Utilities;
using Application.Interfaces.Database;
using Application.Interfaces.Identity;
using AutoMapper;
using Domain.Entities.Identity;
using Shared.Requests.Identity;
using static Application.Constants.Database.MsSqlConstants.StoredProcedures;

namespace Infrastructure.Features.Identity;

public class UserRepository : IUserRepository
{
    private readonly ISqlDataAccess _database;
    private readonly IMapper _mapper;

    public UserRepository(ISqlDataAccess database, IMapper mapper)
    {
        _database = database;
        _mapper = mapper;
    }

    public Task<IEnumerable<User>> GetAllUsers() =>
        _database.LoadData<User, dynamic>(UserGetAll, new { });

    public async Task<User?> GetUserById(GetUserByIdRequest userRequest) =>
        (await _database.LoadData<User, dynamic>(UserGetById.ToDboName(), userRequest)).FirstOrDefault();

    public async Task<User?> GetUserByUsername(GetUserByUsernameRequest userRequest) =>
        (await _database.LoadData<User, dynamic>(UserGetByUsername.ToDboName(), userRequest)).FirstOrDefault();

    public Task UpdateUser(UpdateUserRequest userRequest) =>
        _database.SaveData(UserUpdate.ToDboName(), userRequest);

    public Task DeleteUser(DeleteUserRequest userRequest) =>
        _database.SaveData(UserDelete.ToDboName(), userRequest);

    public Task RegisterUser(UserRegisterRequest userRequest)
    {
        // TODO: Add user validation, existing, overlapping email, ect
        IdentityUtilities.GetPasswordHash(userRequest.Password, out var passwordHash, out var passwordSalt);
        
        // TODO: Add auditing field data once authorization is in place
        var newUser = _mapper.Map<User>(userRequest);
        newUser.NormalizedUserName = userRequest.Username.ToUpper();
        newUser.NormalizedEmail = userRequest.Email.ToUpper();
        newUser.PasswordHash = passwordHash;
        newUser.PasswordSalt = passwordSalt;
        
        return _database.SaveData(UserCreate.ToDboName(), newUser);
    }
}
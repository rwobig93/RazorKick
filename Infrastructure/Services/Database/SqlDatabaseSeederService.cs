using Application.Constants.Identity;
using Application.Helpers.Identity;
using Application.Models.Identity;
using Application.Repositories.Identity;
using Domain.DatabaseEntities.Identity;
using Domain.Models.Database;
using Microsoft.Extensions.Hosting;
using Shared.Enums.Identity;

namespace Infrastructure.Services.Database;

public class SqlDatabaseSeederService : IHostedService
{
    private readonly ILogger _logger;
    private readonly IAppUserRepository _userRepository;
    private readonly IAppRoleRepository _roleRepository;
    private readonly IAppPermissionRepository _permissionRepository;

    public SqlDatabaseSeederService(ILogger logger, IAppUserRepository userRepository, IAppRoleRepository roleRepository,
        IAppPermissionRepository permissionRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.Debug("Starting to seed database");
        await SeedDatabaseRoles();
        await SeedDatabaseUsers();
        _logger.Information("Finished seeding database");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        // We don't have any cleanup required so we'll just return a completed task
        await Task.CompletedTask;
    }

    private async Task SeedDatabaseRoles()
    {
        var adminRole = await CreateOrGetSeedRole(RoleConstants.AdminRoleName, RoleConstants.AdminRoleDescription);
        if (adminRole.Success)
            await EnforcePermissionsForRole(adminRole.Result!.Id, PermissionConstants.GetAllPermissions());

        var defaultRole = await CreateOrGetSeedRole(RoleConstants.DefaultRoleName, RoleConstants.DefaultRoleDescription);
        if (defaultRole.Success)
            await EnforcePermissionsForRole(defaultRole.Result!.Id, PermissionConstants.GetDefaultRolePermissions());
    }

    private async Task<DatabaseActionResult<AppRoleDb>> CreateOrGetSeedRole(string roleName, string roleDescription)
    {
        var existingRole = await _roleRepository.GetByNameAsync(roleName);
        if (existingRole.Result is not null || !existingRole.Success)
            return existingRole;
        
        var createdRole = await _roleRepository.CreateAsync(new AppRoleCreate()
        {
            Name = roleName,
            NormalizedName = roleName.NormalizeForDatabase(),
            Description = roleDescription,
            CreatedOn = DateTime.Now,
            CreatedBy = Guid.Empty,
            LastModifiedBy = Guid.Empty
        });
        _logger.Information("Created missing {RoleName} role with id: {RoleId}", RoleConstants.AdminRoleName, createdRole.Result);

        return await _roleRepository.GetByIdAsync(createdRole.Result);
    }

    private async Task EnforcePermissionsForRole(Guid roleId, List<string> desiredPermissions)
    {
        var currentPermissions = await _permissionRepository.GetAllForRoleAsync(roleId);
        foreach (var permission in desiredPermissions)
        {
            if (currentPermissions.Result!.Any(x => x.ClaimValue == permission))
                continue;

            var convertedPermission = permission.ToAppPermissionCreate();
            convertedPermission.RoleId = roleId;
            var addedPermission = await _permissionRepository.CreateAsync(convertedPermission);
            if (!addedPermission.Success)
            {
                _logger.Error("Failed to enforce permission {PermissionValue} on role {RoleId}: {ErrorMessage}",
                    permission, roleId, addedPermission.ErrorMessage);
                continue;
            }
            
            _logger.Debug("Added missing {PermissionValue} to role {RoleId} with id {PermissionId}",
                permission, roleId, addedPermission.Result);
        }
        _logger.Debug("Finished enforcing seeded permissions for role {roleId}", roleId);
    }

    private async Task SeedDatabaseUsers()
    {
        var adminUser = await CreateOrGetSeedUser(
            UserConstants.DefaultAdminUsername, "Admini", "Strator", UserConstants.DefaultAdminPassword);
        if (adminUser.Success)
            await EnforceRolesForUser(adminUser.Result!.Id, new List<string>() {RoleConstants.AdminRoleName});
    }

    private async Task<DatabaseActionResult<AppUserDb>> CreateOrGetSeedUser(string userName, string firstName, string lastName, string userPassword)
    {
        var existingUser = await _userRepository.GetByUsernameAsync(userName);
        if (!existingUser.Success)
            _logger.Error("Failed to seed user in database: {UserName} => {ErrorMessage}", userName, existingUser.ErrorMessage);
        if (existingUser.Result is not null || !existingUser.Success)
            return existingUser;
        
        AccountHelpers.GetPasswordHash(userPassword, out var salt, out var hash);
        var createdUser = await _userRepository.CreateAsync(new AppUserCreate
        {
            Username = userName,
            NormalizedUserName = userName.NormalizeForDatabase(),
            Email = $"{userName}@localhost.local",
            NormalizedEmail = $"{userName}@localhost.local".NormalizeForDatabase(),
            EmailConfirmed = true,
            PasswordHash = hash.ToString()!,
            PasswordSalt = salt,
            PhoneNumber = "",
            PhoneNumberConfirmed = false,
            TwoFactorEnabled = false,
            FirstName = firstName,
            LastName = lastName,
            ProfilePictureDataUrl = null,
            CreatedBy = Guid.Empty,
            CreatedOn = DateTime.Now,
            LastModifiedBy = null,
            LastModifiedOn = null,
            IsDeleted = false,
            DeletedOn = null,
            IsActive = true,
            RefreshToken = null,
            RefreshTokenExpiryTime = null,
            AccountType = AccountType.User
        });
        _logger.Information("Created missing {UserName} user with id: {UserId}", userName, createdUser.Result);

        return await _userRepository.GetByIdAsync(createdUser.Result);
    }

    private async Task EnforceRolesForUser(Guid userId, List<string> roleNames)
    {
        var currentRoles = await _roleRepository.GetRolesForUser(userId);
        foreach (var role in roleNames)
        {
            if (currentRoles.Result!.Any(x => x.Name == role))
                continue;
            
            var foundRole = await _roleRepository.GetByNameAsync(role);
            await _roleRepository.AddUserToRoleAsync(userId, foundRole.Result!.Id);
            
            _logger.Debug("Added missing role {RoleId} to user {UserId}", foundRole.Result.Id, userId);
        }
    }
}
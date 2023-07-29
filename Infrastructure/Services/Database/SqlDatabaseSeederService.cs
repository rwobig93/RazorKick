using Application.Constants.Identity;
using Application.Helpers.Identity;
using Application.Helpers.Web;
using Application.Mappers.Identity;
using Application.Models.Identity.Role;
using Application.Models.Identity.User;
using Application.Models.Identity.UserExtensions;
using Application.Models.Lifecycle;
using Application.Repositories.Identity;
using Application.Repositories.Lifecycle;
using Application.Services.Lifecycle;
using Application.Settings.AppSettings;
using Domain.DatabaseEntities.Identity;
using Domain.Enums.Identity;
using Domain.Models.Database;
using Domain.Models.Identity;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services.Database;

public class SqlDatabaseSeederService : IHostedService
{
    private readonly ILogger _logger;
    private readonly IAppUserRepository _userRepository;
    private readonly IAppRoleRepository _roleRepository;
    private readonly IAppPermissionRepository _permissionRepository;
    private readonly LifecycleConfiguration _lifecycleConfig;
    private readonly IRunningServerState _serverState;
    private readonly SecurityConfiguration _securityConfig;
    private readonly IServerStateRecordsRepository _serverStateRepository;

    private AppUserSecurityDb _systemUser = new() { Id = Guid.Empty };

    public SqlDatabaseSeederService(ILogger logger, IAppUserRepository userRepository, IAppRoleRepository roleRepository,
        IAppPermissionRepository permissionRepository, IOptions<LifecycleConfiguration> lifecycleConfig, IRunningServerState serverState,
        IOptions<SecurityConfiguration> securityConfig, IServerStateRecordsRepository serverStateRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
        _lifecycleConfig = lifecycleConfig.Value;
        _serverState = serverState;
        _serverStateRepository = serverStateRepository;
        _securityConfig = securityConfig.Value;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.Debug("Starting to seed database");
        await SeedSystemUser();
        await SeedDatabaseRoles();
        await SeedDatabaseUsers();
        await SeedServerStateRecords();
        _logger.Information("Finished seeding database");
    }

    private async Task SeedSystemUser()
    {
        var systemUser = await CreateOrGetSeedUser(
            UserConstants.DefaultUsers.SystemUsername, UserConstants.DefaultUsers.SystemFirstName, UserConstants.DefaultUsers.SystemLastName,
            UserConstants.DefaultUsers.SystemEmail, UrlHelpers.GenerateToken(64));
        _systemUser = systemUser.Result!;

        _serverState.UpdateSystemUserId(_systemUser.Id);
    }

    private async Task SeedDatabaseRoles()
    {
        var adminRole = await CreateOrGetSeedRole(
            RoleConstants.DefaultRoles.AdminName, RoleConstants.DefaultRoles.AdminDescription);
        if (adminRole.Succeeded)
            await EnforcePermissionsForRole(adminRole.Result!.Id, PermissionHelpers.GetAllBuiltInPermissions());
        
        var moderatorRole = await CreateOrGetSeedRole(
            RoleConstants.DefaultRoles.ModeratorName, RoleConstants.DefaultRoles.ModeratorDescription);
        if (moderatorRole.Succeeded && _lifecycleConfig.EnforceDefaultRolePermissions)
            await EnforcePermissionsForRole(moderatorRole.Result!.Id, PermissionHelpers.GetModeratorRolePermissions());
        
        var serviceAccountRole = await CreateOrGetSeedRole(
            RoleConstants.DefaultRoles.ServiceAccountName, RoleConstants.DefaultRoles.ServiceAccountDescription);
        if (serviceAccountRole.Succeeded && _lifecycleConfig.EnforceDefaultRolePermissions)
            await EnforcePermissionsForRole(serviceAccountRole.Result!.Id, PermissionHelpers.GetServiceAccountRolePermissions());

        var defaultRole = await CreateOrGetSeedRole(
            RoleConstants.DefaultRoles.DefaultName, RoleConstants.DefaultRoles.DefaultDescription);
        if (defaultRole.Succeeded && _lifecycleConfig.EnforceDefaultRolePermissions)
            await EnforcePermissionsForRole(defaultRole.Result!.Id, PermissionHelpers.GetDefaultRolePermissions());
    }

    private async Task SeedDatabaseUsers()
    {
        // Seed system user permissions
        await EnforceRolesForUser(_systemUser.Id, RoleConstants.GetAdminRoleNames());
        
        var adminUser = await CreateOrGetSeedUser(
            UserConstants.DefaultUsers.AdminUsername, UserConstants.DefaultUsers.AdminFirstName, UserConstants.DefaultUsers.AdminLastName,
            UserConstants.DefaultUsers.AdminEmail, UserConstants.DefaultUsers.AdminPassword);
        if (adminUser.Succeeded)
            await EnforceRolesForUser(adminUser.Result!.Id, RoleConstants.GetAdminRoleNames());

        if (_lifecycleConfig.EnforceTestAccounts)
        {
            var moderatorUser = await CreateOrGetSeedUser(
                UserConstants.DefaultUsers.ModeratorUsername, UserConstants.DefaultUsers.ModeratorFirstName,
                UserConstants.DefaultUsers.ModeratorLastName, UserConstants.DefaultUsers.ModeratorEmail, UserConstants.DefaultUsers.ModeratorPassword);
            if (moderatorUser.Succeeded)
                await EnforceRolesForUser(moderatorUser.Result!.Id, RoleConstants.GetModeratorRoleNames());
            
            var basicUser = await CreateOrGetSeedUser(
                UserConstants.DefaultUsers.BasicUsername, UserConstants.DefaultUsers.BasicFirstName, UserConstants.DefaultUsers.BasicLastName,
                UserConstants.DefaultUsers.BasicEmail, UserConstants.DefaultUsers.BasicPassword);
            if (basicUser.Succeeded)
                await EnforceRolesForUser(basicUser.Result!.Id, RoleConstants.GetDefaultRoleNames());
        }
        
        var anonymousUser = await CreateOrGetSeedUser(
            UserConstants.DefaultUsers.AnonymousUsername, UserConstants.DefaultUsers.AnonymousFirstName,
            UserConstants.DefaultUsers.AnonymousLastName, UserConstants.DefaultUsers.AnonymousEmail, UrlHelpers.GenerateToken(64));
        if (anonymousUser.Succeeded)
            await EnforceAnonUserIdToEmptyGuid(anonymousUser.Result!.Id);
    }

    private async Task SeedServerStateRecords()
    {
        var existingStateRecord = await _serverStateRepository.GetByVersion(_serverState.ApplicationVersion);
        if (!existingStateRecord.Succeeded)
        {
            _logger.Error("Failed to retrieve existing server state record: {Error}", existingStateRecord.ErrorMessage);
            return;
        }

        var existingRecord = existingStateRecord.Result?.FirstOrDefault();
        if (existingRecord is not null)
        {
            _logger.Debug("Existing record for current application version exists => [{Id}]{Version} :: {Timestamp}",
                existingRecord.Id, existingRecord.AppVersion, existingRecord.Timestamp);
            return;
        }

        var createRecordRequest = await _serverStateRepository.CreateAsync(new ServerStateRecordCreate
        {
            AppVersion = _serverState.ApplicationVersion.ToString()
        });
        if (!createRecordRequest.Succeeded)
        {
            _logger.Error("Failed to create server state record: {Error}", createRecordRequest.ErrorMessage);
            return;
        }
        
        _logger.Information("Created server state record: {Id} {Version}", createRecordRequest.Result, _serverState.ApplicationVersion);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        // We don't have any cleanup required so we'll just return a completed task
        await Task.CompletedTask;
    }

    private async Task<DatabaseActionResult<AppRoleDb>> CreateOrGetSeedRole(string roleName, string roleDescription)
    {
        var existingRole = await _roleRepository.GetByNameAsync(roleName);
        if (!existingRole.Succeeded)
            return existingRole;
        if (existingRole.Result is not null)
            return existingRole;
        
        var createdRole = await _roleRepository.CreateAsync(new AppRoleCreate()
        {
            Name = roleName,
            Description = roleDescription,
            CreatedOn = DateTime.Now,
            CreatedBy = _systemUser.Id
        });
        _logger.Information("Created missing {RoleName} role with id: {RoleId}", roleName, createdRole.Result);

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
            convertedPermission.CreatedBy = _systemUser.Id;
            var addedPermission = await _permissionRepository.CreateAsync(convertedPermission);
            if (!addedPermission.Succeeded)
            {
                _logger.Error("Failed to enforce permission {PermissionValue} on role {RoleId}: {ErrorMessage}",
                    permission, roleId, addedPermission.ErrorMessage);
                continue;
            }
            
            _logger.Debug("Added missing {PermissionValue} to role {RoleId} with id {PermissionId}",
                permission, roleId, addedPermission.Result);
        }
    }

    private async Task<DatabaseActionResult<AppUserSecurityDb>> CreateOrGetSeedUser(string userName, string firstName, string lastName, string email,
        string userPassword)
    {
        var existingUser = await _userRepository.GetByUsernameAsync(userName);
        if (!existingUser.Succeeded)
        {
            _logger.Error("Failed to seed user in database: {UserName} => {ErrorMessage}", userName, existingUser.ErrorMessage);
            return existingUser;
        }
        if (existingUser.Result is not null)
            return existingUser;
        
        var createdUser = await _userRepository.CreateAsync(new AppUserCreate
        {
            Username = userName,
            Email = email,
            EmailConfirmed = true,
            PhoneNumber = "",
            PhoneNumberConfirmed = false,
            FirstName = firstName,
            LastName = lastName,
            ProfilePictureDataUrl = null,
            CreatedBy = _systemUser.Id,
            CreatedOn = DateTime.Now,
            LastModifiedBy = null,
            LastModifiedOn = null,
            IsDeleted = false,
            DeletedOn = null,
            AccountType = AccountType.User
        });
        
        AccountHelpers.GenerateHashAndSalt(userPassword, _securityConfig.PasswordPepper, out var salt, out var hash);
        await _userRepository.UpdateSecurityAsync(new AppUserSecurityAttributeUpdate
        {
            OwnerId = createdUser.Result,
            PasswordHash = hash,
            PasswordSalt = salt,
            TwoFactorEnabled = false,
            TwoFactorKey = null,
            AuthState = AuthState.Enabled,
            AuthStateTimestamp = null,
            BadPasswordAttempts = 0,
            LastBadPassword = null
        });
        
        _logger.Information("Created missing {UserName} user with id: {UserId}", userName, createdUser.Result);
        
        return await _userRepository.GetByIdAsync(createdUser.Result);
    }

    private async Task EnforceRolesForUser(Guid userId, IEnumerable<string> roleNames)
    {
        var currentRoles = await _roleRepository.GetRolesForUser(userId);
        foreach (var role in roleNames.Where(role => !currentRoles.Result!.Any(x => x.Name == role)))
        {
            var foundRole = await _roleRepository.GetByNameAsync(role);
            await _roleRepository.AddUserToRoleAsync(userId, foundRole.Result!.Id, _systemUser.Id);
            
            _logger.Debug("Added missing role {RoleId} to user {UserId}", foundRole.Result.Id, userId);
        }
    }

    private async Task EnforceAnonUserIdToEmptyGuid(Guid currentId)
    {
        var desiredAnonUser = await _userRepository.GetByIdAsync(Guid.Empty);
        if (desiredAnonUser.Result is not null)
            return;
        
        var updatedId = await _userRepository.SetUserId(currentId, Guid.Empty);
        if (!updatedId.Succeeded)
        {
            _logger.Error("Failed to set Anonymous UserId to Empty Guid: {ErrorMessage}", updatedId.ErrorMessage);
            return;
        }
        
        var anonUserValidation = await _userRepository.GetByIdAsync(Guid.Empty);
        if (anonUserValidation.Result is null)
        {
            _logger.Error("Failed to get Anonymous UserId after update: {ErrorMessage}", anonUserValidation.ErrorMessage);
            return;
        }
        
        _logger.Information("Anon user ID was updated and validated correct: {UserId}", anonUserValidation.Result!.Id);
    }
}
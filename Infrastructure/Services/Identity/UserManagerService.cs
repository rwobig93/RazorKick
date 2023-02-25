using Domain.DatabaseEntities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services.Identity;

public class UserManagerService : UserManager<AppUserDb>
{
    public UserManagerService(IUserStore<AppUserDb> store,
        IOptions<IdentityOptions> optionsAccessor,
        IPasswordHasher<AppUserDb> passwordHasher,
        IEnumerable<IUserValidator<AppUserDb>> userValidators,
        IEnumerable<IPasswordValidator<AppUserDb>> passwordValidators,
        ILookupNormalizer keyNormalizer,
        IdentityErrorDescriber errors,
        IServiceProvider services,
        ILogger<UserManagerService> logger) :
        base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
    {
    }
}
using Domain.DatabaseEntities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Application.Services.Identity;

public interface IAppIdentityService : IUserEmailStore<AppUserDb>, IUserPhoneNumberStore<AppUserDb>,
    IUserTwoFactorStore<AppUserDb>, IUserPasswordStore<AppUserDb>
{
    
}
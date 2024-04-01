using Capstone.UniFarm.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Capstone.UniFarm.API.Configurations;

public class CustomUserManager : UserManager<Account>
{
    public CustomUserManager(IUserStore<Account> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<Account> passwordHasher, IEnumerable<IUserValidator<Account>> userValidators, IEnumerable<IPasswordValidator<Account>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<Account>> logger)
        : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
    {
    }
}
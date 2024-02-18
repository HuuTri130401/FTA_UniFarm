using AutoMapper;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Repositories.UnitOfWork;
using Capstone.UniFarm.Services.Commons;
using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.Account.Request;
using Capstone.UniFarm.Services.ViewModels.Account.Response;
using Capstone.UniFarm.Services.ViewModels.Authen.Request;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Capstone.UniFarm.Domain.Enum;

namespace Capstone.UniFarm.Services.CustomServices
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<Account> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AccountService(
            UserManager<Account> userManager,
            IUnitOfWork unitOfWork,
            IMapper mapper
        )
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<OperationResult<RegisterVM>> CreateAccount(RegisterVM registerVM)
        {
            var result = new OperationResult<RegisterVM>();
            try
            {
                var newAccount = _mapper.Map<Account>(registerVM);
                newAccount.CreatedAt = DateTime.UtcNow;
                newAccount.PasswordHash = _userManager.PasswordHasher.HashPassword(newAccount, registerVM.Password);
                var response = await _userManager.CreateAsync(newAccount);
                if (response.Succeeded)
                {
                    await _userManager.AddToRoleAsync(newAccount, registerVM.Role.ToString());
                    result.Payload = registerVM;
                }
                else
                {
                    result.Payload = null;
                    result.AddError(ErrorCode.BadRequest, response.Errors.AsEnumerable().ToList().ToString());
                }
            }
            catch (Exception ex)
            {
                result.AddUnknownError(ex.Message);
            }
            return result;
        }

        public Task<OperationResult<bool>> DeleteAccount(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<AccountResponse>> GetAccountById(Guid accountId)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<Pagination<AccountResponse>>> GetAccountPaginationAsync(int pageIndex = 0, int pageSize = 10)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<IEnumerable<AccountResponse>>> GetAllAccounts()
        {
            throw new NotImplementedException();
        }

        public async Task<OperationResult<Account>> HandleLoginGoogle(IEnumerable<Claim> claims)
        {
            var result = new OperationResult<Account>();
            try
            {
                var emailClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                if (emailClaim is null)
                {
                    result.AddError(ErrorCode.UnAuthorize, "Email claim is missing");
                    return result;
                }
                var user = await _unitOfWork.AccountRepository.FindSingleAsync(x => x.Email == emailClaim!.Value);
                if (user is null)
                {
                    var userName = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
                    var givenName = claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName);
                    var surName = claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname);
                    var identifier = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                    var newUser = new Account
                    {
                        Id = Guid.NewGuid(),
                        Email = emailClaim.Value,
                        UserName = userName != null ? userName.Value : emailClaim.Value,
                        FirstName = givenName != null ? givenName.Value : null,
                        LastName = surName != null ? surName.Value : null,
                    };

                    var creationResult = await _userManager.CreateAsync(newUser);
                    if (creationResult.Succeeded)
                    {
                        var userLogin = new UserLoginInfo(
                            loginProvider: "Google",
                            providerKey: identifier != null ? identifier.Value : null,
                            displayName: userName != null ? userName.Value : emailClaim.Value
                        );
                        await _userManager.AddLoginAsync(newUser, userLogin);
                        await _userManager.AddToRoleAsync(newUser, "Customer");
                        result.Payload = newUser;
                    }
                    else
                    {
                        result.IsError = true;
                        result.AddError(ErrorCode.UnAuthorize, creationResult.Errors.FirstOrDefault().Description);
                    }
                }
                else
                {
                    result.IsError = false;
                    result.Payload = user;
                }
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }
            return result;
        }

        public Task<OperationResult<AccountResponse>> UpdateAccount(Guid Id, AccountRequest accountRequest)
        {
            throw new NotImplementedException();
        }


        public string GenerateJwtToken(Account user, byte[] key)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.Name, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public Task<OperationResult<IEnumerable<AccountResponse>>> GetAllAccounts(Expression<Func<Account, bool>>? predicate)
        {
            throw new NotImplementedException();
        }

        public async Task<OperationResult<AccountResponse>> GetAccountByPredicate(Expression<Func<Account, bool>>? predicate)
        {
            var result = new OperationResult<AccountResponse>();
            try
            {
                var user = await _unitOfWork.AccountRepository.FindSingleAsync(predicate);
                if (user == null)
                {
                    result.AddError(ErrorCode.NotFound, "User not found");
                    return result;
                }
                var accountReponse = _mapper.Map<AccountResponse>(user);
                result.Payload = accountReponse;
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }
            return result;
        }
    }
}

using AutoMapper;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Repositories.UnitOfWork;
using Capstone.UniFarm.Services.Commons;
using Capstone.UniFarm.Services.ICustomServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using Capstone.UniFarm.Domain.Enum;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Capstone.UniFarm.Services.ViewModels.ModelResponses;
using Microsoft.EntityFrameworkCore;

namespace Capstone.UniFarm.Services.CustomServices
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<Account> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWalletService _walletService;
        private readonly IAccountRoleService _accountRoleService;
        private readonly ICollectedHubService _collectedHubService;
        private readonly IStationService _stationService;
        private readonly IMapper _mapper;

        public AccountService(
            UserManager<Account> userManager,
            IUnitOfWork unitOfWork,
            IAccountRoleService accountRoleService,
            IWalletService walletService,
            ICollectedHubService collectedHubService,
            IStationService stationService,
            IMapper mapper
        )
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _walletService = walletService;
            _accountRoleService = accountRoleService;
            _collectedHubService = collectedHubService;
            _stationService = stationService;
            _mapper = mapper;
        }
        
        public string GenerateJwtToken(Account user, byte[] key, string userRole)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, userRole)
                }),
                Audience = "FTAAudience",
                Issuer = "FTAIssuer",
                Expires = DateTime.UtcNow.AddHours(24),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        
        public async Task<OperationResult<AccountRequestCreate>> CreateAccount(AccountRequestCreate accountRequestCreate)
        {
            var result = new OperationResult<AccountRequestCreate>();
            try
            {
                var checkEmail = await _userManager.FindByEmailAsync(accountRequestCreate.Email);
                if (checkEmail != null)
                {
                    result.AddError(StatusCode.BadRequest, "Email already exists");
                    result.IsError = true;
                    return result;
                }

                var checkPhone =
                    await _unitOfWork.AccountRepository.FindSingleAsync(x => x.Phone == accountRequestCreate.PhoneNumber);
                if (checkPhone != null)
                {
                    result.AddError(StatusCode.BadRequest, "Phone already exists");
                    result.IsError = true;
                    return result;
                }

                var newAccount = _mapper.Map<Account>(accountRequestCreate);
                newAccount.CreatedAt = DateTime.UtcNow;
                newAccount.PasswordHash =
                    _userManager.PasswordHasher.HashPassword(newAccount, accountRequestCreate.Password);
                newAccount.Status = EnumConstants.ActiveInactiveEnum.ACTIVE;
                newAccount.RoleName = EnumConstants.RoleEnumString.CUSTOMER;
                var accountRole = new AccountRole
                {
                    Id = Guid.NewGuid(),
                    AccountId = newAccount.Id,
                    Account = newAccount,
                    Status = EnumConstants.ActiveInactiveEnum.ACTIVE
                };
                newAccount.AccountRoles = accountRole;

                var response = await _userManager.CreateAsync(newAccount);
                if (response.Succeeded)
                {
                    var wallet = new WalletRequest
                    {
                        AccountId = newAccount.Id,
                    };

                    var walletResult = await _walletService.Create(wallet);
                    if (walletResult.IsError)
                    {
                        result.AddError(StatusCode.BadRequest,
                            "Create wallet error " + walletResult.Errors.FirstOrDefault()?.Message);
                        result.IsError = true;
                        return result;
                    }

                    await _userManager.AddToRoleAsync(newAccount, accountRequestCreate.Role);
                    result.Payload = accountRequestCreate;
                    result.IsError = false;
                }
                else
                {
                    result.Payload = null;
                    result.AddError(StatusCode.BadRequest, response.Errors.AsEnumerable().ToList().ToString()!);
                    result.IsError = true;
                }
            }
            catch (Exception ex)
            {
                result.AddUnknownError(ex.Message);
                throw;
            }

            return result;
        }

        public async Task<OperationResult<AccountResponse>> GetAccountById(Guid accountId)
        {
            var result = new OperationResult<AccountResponse>();
            try
            {
                var account = await _unitOfWork.AccountRepository.GetByIdAsync(accountId);
                if (account == null)
                {
                    result.AddError(StatusCode.NotFound, "Account not found");
                    result.Message = "Account not found";
                    result.IsError = true;
                    return result;
                }

                var accountResponse = _mapper.Map<AccountResponse>(account);
                result.Payload = accountResponse;
                result.Message = "Get account by id successfully";
                result.IsError = false;
            }
            catch (Exception e)
            {
                result.AddUnknownError("Get account by id error" + e.Message);
                result.IsError = true;
                result.Message = "Get account by id error";
                throw;
            }

            return result;
        }
        
        public async Task<OperationResult<Account>> HandleLoginGoogle(IEnumerable<Claim> claims)
        {
            var result = new OperationResult<Account>();
            try
            {
                var enumerable = claims.ToList();
                var emailClaim = enumerable.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                if (emailClaim is null)
                {
                    result.AddError(StatusCode.UnAuthorize, "Email claim is missing");
                    return result;
                }

                var user = await _unitOfWork.AccountRepository.FindSingleAsync(x => x.Email == emailClaim!.Value);
                if (user is null)
                {
                    var userName = enumerable.FirstOrDefault(c => c.Type == ClaimTypes.Name);
                    var givenName = enumerable.FirstOrDefault(c => c.Type == ClaimTypes.GivenName);
                    var surName = enumerable.FirstOrDefault(c => c.Type == ClaimTypes.Surname);
                    var identifier = enumerable.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                    var newUser = new Account
                    {
                        Id = Guid.NewGuid(),
                        Email = emailClaim.Value,
                        UserName = userName != null ? userName.Value : emailClaim.Value,
                        FirstName = givenName?.Value,
                        LastName = surName?.Value,
                        RoleName = "Customer",
                        CreatedAt = DateTime.UtcNow
                    };

                    var accountRole = new AccountRole
                    {
                        Id = Guid.NewGuid(),
                        AccountId = newUser.Id,
                        Account = newUser,
                        Status = "Active"
                    };
                    newUser.AccountRoles = accountRole;

                    // Tạo tài khoản mới
                    var creationResult = await _userManager.CreateAsync(newUser);
                    if (creationResult.Succeeded)
                    {
                        var wallet = new WalletRequest
                        {
                            AccountId = newUser.Id,
                        };

                        var walletResult = await _walletService.Create(wallet);
                        if (walletResult.IsError)
                        {
                            result.AddError(StatusCode.BadRequest,
                                "Create wallet error " + walletResult.Errors.FirstOrDefault()?.Message);
                            result.IsError = true;
                            return result;
                        }

                        var userLogin = new UserLoginInfo(
                            loginProvider: "Google",
                            providerKey: identifier?.Value,
                            displayName: userName != null ? userName.Value : emailClaim.Value
                        );
                        await _userManager.AddLoginAsync(newUser, userLogin);
                        await _userManager.AddToRoleAsync(newUser, EnumConstants.RoleEnumString.CUSTOMER);
                        result.Payload = newUser;
                        result.IsError = false;
                    }
                    else
                    {
                        
                        result.IsError = true;
                        var description = creationResult.Errors.FirstOrDefault()?.Description;
                        if (description != null)
                            result.AddError(StatusCode.UnAuthorize,
                                description);
                    }
                }
                else
                {
                    if(user.Status == EnumConstants.ActiveInactiveEnum.INACTIVE)
                    {
                        result.AddError(StatusCode.UnAuthorize, "Account has been blocked! Please contact admin for more information fta@gmail.com");
                        result.IsError = true;
                        return result;
                    }
                    result.IsError = false;
                    result.Payload = user;
                }
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
                result.IsError = true;
                throw;
            }

            return result;
        }

        public Task<OperationResult<AccountResponse>> GetAccountByExpression(Expression<Func<Account, bool>> predicate,
            string[]? includeProperties = null)
        {
            var result = new OperationResult<AccountResponse>();
            try
            {
                var account = _unitOfWork.AccountRepository.FilterByExpression(predicate, includeProperties);
                if (!account.Any())
                {
                    result.AddError(StatusCode.NotFound, "Account not found");
                    result.Message = "Account not found";
                    result.IsError = true;
                    result.StatusCode = StatusCode.NotFound;
                    return Task.FromResult(result);
                }

                var accountResponse = _mapper.Map<AccountResponse>(account);
                result.Payload = accountResponse;
                result.Message = "Get account successfully";
                result.IsError = false;
                result.StatusCode = StatusCode.Ok;
            }
            catch (Exception e)
            {
                result.AddUnknownError("Get account by expression error " + e.Message);
                result.IsError = true;
                result.Message = "Get account by expression error";
                result.StatusCode = StatusCode.ServerError;
                throw;
            }

            return Task.FromResult(result);
        }

        public async Task<OperationResult<AccountResponse>> UpdateAccount(Guid id,
            AccountRequestUpdate accountRequestUpdate)
        {
            var result = new OperationResult<AccountResponse>();
            try
            {
                var account = await _unitOfWork.AccountRepository.GetByIdAsync(id);
                if (account == null)
                {
                    result.AddError(StatusCode.NotFound, "Account not found");
                    result.Message = "Account not found";
                    result.IsError = true;
                    result.StatusCode = StatusCode.NotFound;
                    return result;
                }

                var accountUpdate = _mapper.Map<Account>(accountRequestUpdate);
                accountUpdate.Id = id;
                var userList = _unitOfWork.AccountRepository.FilterByExpression(x =>
                    x.Email == accountUpdate.Email || x.Phone == accountUpdate.Phone);
                if (userList.Count() > 1)
                {
                    result.AddError(StatusCode.BadRequest, "Email or phone number already exists");
                    result.Message = "Email or phone number already exists";
                    result.IsError = true;
                    result.StatusCode = StatusCode.BadRequest;
                    return result;
                }
                _unitOfWork.AccountRepository.Update(accountUpdate);
                await _unitOfWork.SaveChangesAsync();
                result.Payload = _mapper.Map<AccountResponse>(accountUpdate);
                result.Message = "Update account successfully";
                result.IsError = false;
            }
            catch (Exception e)
            {
                result.AddUnknownError("Update account error " + e.Message);
                result.IsError = true;
                result.Message = "Update account error";
                result.StatusCode = StatusCode.ServerError;
                throw;
            }

            return result;
        }


        public OperationResult<AboutMeResponse.AboutMeRoleAndID> GetIdAndRoleFromToken(string token)
        {
            var result = new OperationResult<AboutMeResponse.AboutMeRoleAndID>();
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var nameClaim = jwtToken.Claims.First(claim => claim.Type == "name");
            var roleClaim = jwtToken.Claims.First(claim => claim.Type == "role");
            if (roleClaim == null || nameClaim == null)
            {
                result.AddError(StatusCode.UnAuthorize, "Token is invalid");
                result.IsError = true;
                return result;
            }

            result.Payload = new AboutMeResponse.AboutMeRoleAndID
            {
                Id = Guid.Parse(nameClaim.Value),
                Role = roleClaim.Value
            };
            return result;
        }


        public async Task<OperationResult<AboutMeResponse.AboutCustomerResponse>> GetAboutCustomer(Guid accountId)
        {
            var result = new OperationResult<AboutMeResponse.AboutCustomerResponse>();
            try
            {
                var account = await _unitOfWork.AccountRepository
                    .FilterByExpression(x => x.Id == accountId
                                             && x.RoleName ==
                                             EnumConstants.RoleEnumString.CUSTOMER
                                             && x.Status ==
                                             EnumConstants.ActiveInactiveEnum.ACTIVE)
                    .FirstOrDefaultAsync();
                if (account == null)
                {
                    result.AddError(StatusCode.NotFound, "Account not found");
                    result.Message = "Account not found";
                    result.IsError = true;
                    result.StatusCode = StatusCode.NotFound;
                    return result;
                }

                var accountResponse = _mapper.Map<AboutMeResponse.AboutCustomerResponse>(account);
                
                var myWallet = await _walletService.FindByExpression(x =>
                    x.AccountId == accountId && x.Status == EnumConstants.ActiveInactiveEnum.ACTIVE);
                if (myWallet.Payload == null || myWallet.IsError)
                {
                    result.AddError(StatusCode.NotFound, "Wallet not found");
                    result.Message = "Wallet not found";
                    result.IsError = true;
                    result.StatusCode = StatusCode.NotFound;
                    return result;
                }

                accountResponse.Wallet = myWallet.Payload;
                result.Payload = accountResponse;
                result.Message = "Get account successfully";
                result.IsError = false;
                result.StatusCode = StatusCode.Ok;
            }
            catch (Exception e)
            {
                result.AddUnknownError("Get account by expression error " + e.Message);
                result.IsError = true;
                result.Message = "Get account by expression error";
                result.StatusCode = StatusCode.ServerError;
                throw;
            }

            return result;
        }


        /// <summary>
        ///  Get about farm hub
        ///  Check FarmHubId in AccountRole
        ///  Get FarmHub by FarmHubId if exist
        ///  Get Wallet by AccountId
        /// </summary>
        /// <param name="accountId"></param>
        public async Task<OperationResult<AboutMeResponse.AboutFarmHubResponse>> GetAboutFarmHub(Guid accountId)
        {
            var result = new OperationResult<AboutMeResponse.AboutFarmHubResponse>();
            try
            {
                var account = await _unitOfWork.AccountRepository
                    .FilterByExpression(x => x.Id == accountId
                                             && x.RoleName == EnumConstants.RoleEnumString.FARMHUB
                                             && x.Status == EnumConstants.ActiveInactiveEnum.ACTIVE).FirstOrDefaultAsync();
                if (account == null)
                {
                    result.AddError(StatusCode.NotFound, "Account not found");
                    result.Message = "Account not found";
                    result.IsError = true;
                    result.StatusCode = StatusCode.NotFound;
                    return result;
                }

                var accountResponse = _mapper.Map<AboutMeResponse.AboutFarmHubResponse>(account);
                
                var accountRole = await _accountRoleService.GetAccountRoleByExpression(x =>
                    x.AccountId == accountId && x.Status == EnumConstants.ActiveInactiveEnum.ACTIVE);
                if (accountRole.Payload!.FarmHubId != null)
                {
                    var farmHub = await _unitOfWork.FarmHubRepository.GetByIdAsync(accountRole.Payload.FarmHubId.Value);
                    if (farmHub != null)
                    {
                        _mapper.Map(farmHub, accountResponse.FarmHub);
                    }
                }

                var myWallet = await _walletService.FindByExpression(x =>
                    x.AccountId == accountId && x.Status == EnumConstants.ActiveInactiveEnum.ACTIVE);

                if (myWallet.Payload != null)
                {
                    accountResponse.Wallet = myWallet.Payload;
                }

                result.Payload = accountResponse;
                result.Message = "Get account successfully";
                result.IsError = false;
                result.StatusCode = StatusCode.Ok;
            }
            catch (Exception e)
            {
                result.AddUnknownError("Get account by expression error " + e.Message);
                result.IsError = true;
                result.Message = "Get account by expression error";
                result.StatusCode = StatusCode.ServerError;
                throw;
            }

            return result;
        }

        public async Task<OperationResult<AboutMeResponse.AboutCollectedStaffResponse>> GetAboutCollectedStaff(
            Guid accountId)
        {
            var result = new OperationResult<AboutMeResponse.AboutCollectedStaffResponse>();
            try
            {
                var account = await _unitOfWork.AccountRepository
                    .FilterByExpression(x => x.Id == accountId
                                             && x.RoleName == EnumConstants.RoleEnumString.COLLECTEDSTAFF
                                             && x.Status == EnumConstants.ActiveInactiveEnum.ACTIVE)
                    .FirstOrDefaultAsync();
                if (account == null)
                {
                    result.AddError(StatusCode.NotFound, "Account not found");
                    result.Message = "Account not found";
                    result.IsError = true;
                    result.StatusCode = StatusCode.NotFound;
                    return result;
                }

                var accountResponse = _mapper.Map<AboutMeResponse.AboutCollectedStaffResponse>(account);
                
                var accountRole = await _accountRoleService.GetAccountRoleByExpression(x =>
                    x.AccountId == accountId && x.Status == EnumConstants.ActiveInactiveEnum.ACTIVE);
                if (accountRole.Payload!.CollectedHubId != null)
                {
                    var collectedStaff =
                        await _collectedHubService.GetFilterByExpression(x =>
                            x.Id == accountRole.Payload.CollectedHubId.Value);
                    if (collectedStaff.Payload != null)
                    {
                        _mapper.Map(collectedStaff, accountResponse.CollectedHub);
                    }
                }

                result.Payload = accountResponse;
                result.Message = "Get account successfully";
                result.IsError = false;
                result.StatusCode = StatusCode.Ok;
            }
            catch (Exception e)
            {
                result.AddUnknownError("Get account by expression error " + e.Message);
                result.IsError = true;
                result.Message = "Get account by expression error";
                result.StatusCode = StatusCode.ServerError;
                throw;
            }

            return result;
        }

        public async Task<OperationResult<AboutMeResponse.AboutStationStaffResponse>> GetAboutStationStaff(
            Guid accountId)
        {
            var result = new OperationResult<AboutMeResponse.AboutStationStaffResponse>();
            try
            {
                var account = await _unitOfWork.AccountRepository
                    .FilterByExpression(x => x.Id == accountId
                                             && x.RoleName == EnumConstants.RoleEnumString.STATIONSTAFF
                                             && x.Status == EnumConstants.ActiveInactiveEnum.ACTIVE).FirstOrDefaultAsync();
                if (account == null)
                {
                    result.AddError(StatusCode.NotFound, "Account not found");
                    result.Message = "Account not found";
                    result.IsError = true;
                    result.StatusCode = StatusCode.NotFound;
                    return result;
                }

                var accountResponse = _mapper.Map<AboutMeResponse.AboutStationStaffResponse>(account);
                var accountRole = await _accountRoleService.GetAccountRoleByExpression(x =>
                    x.AccountId == accountId && x.Status == EnumConstants.ActiveInactiveEnum.ACTIVE);
                if (accountRole.Payload!.StationId != null)
                {
                    var station =
                        await _stationService.GetFilterByExpression(x => x.Id == accountRole.Payload.StationId.Value);
                    if (station.Payload != null)
                    {
                        _mapper.Map(station, accountResponse.Station);
                    }
                }

                result.Payload = accountResponse;
                result.Message = "Get account successfully";
                result.IsError = false;
                result.StatusCode = StatusCode.Ok;
            }
            catch (Exception e)
            {
                result.AddUnknownError("Get account by expression error " + e.Message);
                result.IsError = true;
                result.Message = "Get account by expression error";
                result.StatusCode = StatusCode.ServerError;
                throw;
            }

            return result;
        }

        public async Task<OperationResult<AboutMeResponse.AboutAdminResponse>> GetAboutAdmin(Guid accountId)
        {
            var result = new OperationResult<AboutMeResponse.AboutAdminResponse>();
            try
            {
                var account = await _unitOfWork.AccountRepository
                    .FilterByExpression(x => x.Id == accountId
                                             && x.RoleName == EnumConstants.RoleEnumString.ADMIN
                                             && x.Status == EnumConstants.ActiveInactiveEnum.ACTIVE).FirstOrDefaultAsync();
                if (account == null)
                {
                    result.AddError(StatusCode.NotFound, "Account not found");
                    result.Message = "Account not found";
                    result.IsError = true;
                    result.StatusCode = StatusCode.NotFound;
                    return result;
                }

                var myWallet = await _walletService.FindByExpression(x =>
                    x.AccountId == accountId && x.Status == EnumConstants.ActiveInactiveEnum.ACTIVE);
                var accountResponse = _mapper.Map<AboutMeResponse.AboutAdminResponse>(account);

                if (myWallet.Payload != null)
                {
                    accountResponse.Wallet = myWallet.Payload;
                }

                result.Payload = accountResponse;
                result.Message = "Get account successfully";
                result.IsError = false;
                result.StatusCode = StatusCode.Ok;
            }
            catch (Exception e)
            {
                result.AddUnknownError("Get account by expression error " + e.Message);
                result.IsError = true;
                result.Message = "Get account by expression error";
                result.StatusCode = StatusCode.ServerError;
                throw;
            }

            return result;
        }
    }
}
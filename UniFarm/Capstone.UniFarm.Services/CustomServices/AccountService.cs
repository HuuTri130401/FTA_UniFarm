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
            var accountRole = _accountRoleService.GetAccountRoleByExpression(x => x.AccountId == user.Id);
            var workPlaceId = "CUSTOMER";
            if (userRole == EnumConstants.RoleEnumString.FARMHUB && accountRole.Result.Payload!.FarmHubId != null)
            {
                workPlaceId = accountRole.Result.Payload.FarmHubId.ToString();
            }
            else if (userRole == EnumConstants.RoleEnumString.COLLECTEDSTAFF &&
                     accountRole.Result.Payload!.CollectedHubId != null)
            {
                workPlaceId = accountRole.Result.Payload.CollectedHubId.ToString();
            }
            else if (userRole == EnumConstants.RoleEnumString.STATIONSTAFF &&
                     accountRole.Result.Payload!.StationId != null)
            {
                workPlaceId = accountRole.Result.Payload.StationId.ToString();
            }
            else if (userRole == EnumConstants.RoleEnumString.ADMIN)
            {
                workPlaceId = "ADMIN";
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, userRole),
                    new Claim("AuthorizationDecision", workPlaceId)
                }),
                Audience = "FTAAudience",
                Issuer = "FTAIssuer",
                Expires = DateTime.UtcNow.AddMonths(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


        public async Task<OperationResult<AccountRequestCreate>> CreateAccount(
            AccountRequestCreate accountRequestCreate)
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
                    await _unitOfWork.AccountRepository.FindSingleAsync(
                        x => x.Phone == accountRequestCreate.PhoneNumber);
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
                    if (user.Status == EnumConstants.ActiveInactiveEnum.INACTIVE)
                    {
                        result.AddError(StatusCode.UnAuthorize,
                            "Account has been blocked! Please contact admin for more information fta@gmail.com");
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
                    Error error = new Error()
                    {
                        Code = StatusCode.NotFound,
                        Message = "Account not found"
                    };
                    result.Errors.Add(error);
                    result.Message = "Account not found";
                    result.IsError = true;
                    result.StatusCode = StatusCode.NotFound;
                    return result;
                }


                var checkEmail = _unitOfWork.AccountRepository.FilterByExpression(x =>
                    x.Email == accountRequestUpdate.Email);

                if (checkEmail.Count() > 1)
                {
                    Error error = new Error()
                    {
                        Code = StatusCode.BadRequest,
                        Message = "Email is already exists"
                    };
                    result.Errors.Add(error);
                    result.IsError = true;
                    result.StatusCode = StatusCode.BadRequest;
                    return result;
                }

                var checkPhone = _unitOfWork.AccountRepository.FilterByExpression(x =>
                    x.Phone == accountRequestUpdate.PhoneNumber);
                if (checkPhone.Count() > 1)
                {
                    Error error = new Error()
                    {
                        Code = StatusCode.BadRequest,
                        Message = "Phone number is already exists"
                    };
                    result.Errors.Add(error);
                    result.IsError = true;
                    result.StatusCode = StatusCode.BadRequest;
                    return result;
                }

                var checkEmailAndPhone = _unitOfWork.AccountRepository.FilterByExpression(x =>
                    x.Email == accountRequestUpdate.Email && x.Phone == accountRequestUpdate.PhoneNumber);

                if (checkEmailAndPhone.Count() > 1)
                {
                    Error error = new Error()
                    {
                        Code = StatusCode.BadRequest,
                        Message = "Email or phone number already exists"
                    };
                    result.Errors.Add(error);
                    result.IsError = true;
                    result.StatusCode = StatusCode.BadRequest;
                    return result;
                }

                if (checkEmail.Count() == 1 && checkPhone.Count() == 1)
                {
                    if (checkEmail.First().Id != checkPhone.First().Id)
                    {
                        Error error = new Error()
                        {
                            Code = StatusCode.BadRequest,
                            Message = "Email or phone number already exists"
                        };
                        result.Errors.Add(error);
                        result.IsError = true;
                        result.StatusCode = StatusCode.BadRequest;
                        return result;
                    }
                }

                account.Email = string.IsNullOrEmpty(accountRequestUpdate.Email) ? account.Email : accountRequestUpdate.Email;
                account.Phone = string.IsNullOrEmpty(accountRequestUpdate.PhoneNumber) ? account.Phone : accountRequestUpdate.PhoneNumber;
                account.FirstName = string.IsNullOrEmpty(accountRequestUpdate.FirstName) ? account.FirstName : accountRequestUpdate.FirstName;
                account.LastName = string.IsNullOrEmpty(accountRequestUpdate.LastName) ? account.LastName : accountRequestUpdate.LastName;
                account.UserName = string.IsNullOrEmpty(accountRequestUpdate.UserName) ? account.UserName : accountRequestUpdate.UserName;
                account.Avatar = string.IsNullOrEmpty(accountRequestUpdate.Avatar) ? account.Avatar : accountRequestUpdate.Avatar;
                account.Code = string.IsNullOrEmpty(accountRequestUpdate.Code) ? account.Code : accountRequestUpdate.Code;
                account.Address = string.IsNullOrEmpty(accountRequestUpdate.Address) ? account.Address : accountRequestUpdate.Address;
                account.Status = string.IsNullOrEmpty(accountRequestUpdate.Status) ? account.Status : accountRequestUpdate.Status;
                account.AccountRoles = null;
                account.NormalizedEmail = account.Email.ToUpper();
                account.UpdatedAt = DateTime.UtcNow;

                if (!string.IsNullOrEmpty(accountRequestUpdate.Password))
                {
                    account.PasswordHash =_userManager.PasswordHasher.HashPassword(account, accountRequestUpdate.Password);
                }

                var response = await _userManager.UpdateAsync(account);
                if (response.Succeeded == false)
                {
                    Error error = new Error()
                    {
                        Code = StatusCode.BadRequest,
                        Message = "Update account error"
                    };
                    result.Errors.Add(error);
                    result.IsError = true;
                    result.StatusCode = StatusCode.BadRequest;
                    return result;
                }
                
                result.Payload = _mapper.Map<AccountResponse>(account);
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
            var nameClaim = jwtToken.Claims.First(claim => claim.Type.ToUpper() == "NAME");
            var roleClaim = jwtToken.Claims.First(claim => claim.Type.ToUpper() == "ROLE");
            var authorizationDecision = jwtToken.Claims.First(claim => claim.Type.ToUpper() == "AUTHORIZATIONDECISION");
            if (roleClaim == null || nameClaim == null)
            {
                result.AddError(StatusCode.UnAuthorize, "Token is invalid");
                result.IsError = true;
                return result;
            }

            result.Payload = new AboutMeResponse.AboutMeRoleAndID
            {
                Id = Guid.Parse(nameClaim.Value),
                Role = roleClaim.Value,
                AuthorizationDecision = authorizationDecision.Value
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

                var accountResponse = _mapper.Map<AboutMeResponse.AboutFarmHubResponse>(account);

                var accountRole = await _accountRoleService.GetAccountRoleByExpression(x =>
                    x.AccountId == accountId && x.Status == EnumConstants.ActiveInactiveEnum.ACTIVE, null);
                if (accountRole.Payload!.FarmHubId != null)
                {
                    var farmHub = await _unitOfWork.FarmHubRepository.GetByIdAsync(accountRole.Payload.FarmHubId.Value);
                    if (farmHub != null)
                    {
                        accountResponse.FarmHub = _mapper.Map<FarmHubResponse>(farmHub);
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
                        accountResponse.CollectedHub = collectedStaff.Payload;
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

                var accountResponse = _mapper.Map<AboutMeResponse.AboutStationStaffResponse>(account);
                var accountRole = await _accountRoleService.GetAccountRoleByExpression(x =>
                    x.AccountId == accountId && x.Status == EnumConstants.ActiveInactiveEnum.ACTIVE);
                if (accountRole.Payload!.StationId != null)
                {
                    var station =
                        await _stationService.GetFilterByExpression(x => x.Id == accountRole.Payload.StationId, null);
                    if (station.Payload != null)
                    {
                        accountResponse.Station = station.Payload;
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

        public async Task<OperationResult<IEnumerable<AccountResponse>>> GetAllWithoutPaging(bool? isAscending,
            string? orderBy = null, Expression<Func<Account, bool>>? filter = null, string[]? includeProperties = null)
        {
            var result = new OperationResult<IEnumerable<AccountResponse>>();
            try
            {
                var accounts = _unitOfWork.AccountRepository
                    .GetAllWithoutPaging(isAscending, orderBy, filter, includeProperties);
                if (!accounts.Any())
                {
                    result.AddError(StatusCode.NotFound, "Account not found");
                    result.Message = "Account not found";
                    result.IsError = true;
                    result.StatusCode = StatusCode.NotFound;
                    return result;
                }

                var accountResponse = _mapper.Map<IEnumerable<AccountResponse>>(accounts);
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

        public async Task<OperationResult<FarmHubRegisterRequest>> CreateFarmhubAccount(
            FarmHubRegisterRequest farmHubRegisterRequest)
        {
            var result = new OperationResult<FarmHubRegisterRequest>();
            try
            {
                var checkEmail = await _userManager.FindByEmailAsync(farmHubRegisterRequest.Email);
                if (checkEmail != null)
                {
                    result.AddError(StatusCode.BadRequest, "Email already exists");
                    result.IsError = true;
                    return result;
                }

                var existingUserName = await _userManager.FindByNameAsync(farmHubRegisterRequest.UserName);
                if (existingUserName != null)
                {
                    result.AddError(StatusCode.BadRequest, "FarmHub Name must be unique!");
                    result.IsError = true;
                    return result;
                }

                var newAccount = _mapper.Map<Account>(farmHubRegisterRequest);
                newAccount.CreatedAt = DateTime.UtcNow;
                newAccount.PasswordHash =
                    _userManager.PasswordHasher.HashPassword(newAccount, farmHubRegisterRequest.Password);
                newAccount.Status = EnumConstants.ActiveInactiveEnum.ACTIVE;
                newAccount.RoleName = EnumConstants.RoleEnumString.FARMHUB;
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

                    await _userManager.AddToRoleAsync(newAccount, farmHubRegisterRequest.Role);
                    result.Payload = farmHubRegisterRequest;
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
    }
}
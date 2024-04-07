using System.Linq.Expressions;
using AutoMapper;
using Capstone.UniFarm.Domain.Enum;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Repositories.UnitOfWork;
using Capstone.UniFarm.Services.Commons;
using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Capstone.UniFarm.Services.ViewModels.ModelResponses;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Capstone.UniFarm.Services.CustomServices;

public class ManageUsersService : IManageUsersService
{
    private readonly UserManager<Account> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWalletService _walletService;
    private readonly IAccountRoleService _accountRoleService;
    private readonly ICollectedHubService _collectedHubService;
    private readonly IStationService _stationService;
    private readonly IMapper _mapper;

    public ManageUsersService(UserManager<Account> userManager, IUnitOfWork unitOfWork, IWalletService walletService,
        IAccountRoleService accountRoleService, ICollectedHubService collectedHubService,
        IStationService stationService, IMapper mapper)
    {
        _userManager = userManager;
        _unitOfWork = unitOfWork;
        _walletService = walletService;
        _accountRoleService = accountRoleService;
        _collectedHubService = collectedHubService;
        _stationService = stationService;
        _mapper = mapper;
    }


    /// <summary>
    ///   Create account for admin
    ///  - Check email and phone number already exists
    ///  - Create account
    ///  - Create wallet
    ///  - Add role for account
    ///  - Return account request create
    /// </summary>
    /// <param name="accountRequestCreate"></param>
    public async Task<OperationResult<AccountRequestCreate>> CreateAccountForAdmin(
        AccountRequestCreate accountRequestCreate)
    {
        var result = new OperationResult<AccountRequestCreate>();
        try
        {
            var checkEmail = await _userManager.FindByEmailAsync(accountRequestCreate.Email);
            if (checkEmail != null)
            {
                result.Message = "Email already exists";
                result.StatusCode = StatusCode.BadRequest;
                Error error = new Error
                {
                    Code = StatusCode.BadRequest,
                    Message = "Email already exists"
                };
                result.Errors.Add(error);
                result.IsError = true;
                return result;
            }

            var checkPhone =
                await _unitOfWork.AccountRepository.FindSingleAsync(x => x.Phone == accountRequestCreate.PhoneNumber);
            if (checkPhone != null)
            {
                result.Message = "Phone number already exists";
                result.StatusCode = StatusCode.BadRequest;
                Error error = new Error
                {
                    Code = StatusCode.BadRequest,
                    Message = "Phone number already exists"
                };
                result.Errors.Add(error);
                result.IsError = true;
                return result;
            }

            var newAccount = _mapper.Map<Account>(accountRequestCreate);
            newAccount.CreatedAt = DateTime.UtcNow;
            newAccount.PasswordHash =
                _userManager.PasswordHasher.HashPassword(newAccount, accountRequestCreate.Password);
            newAccount.Status = EnumConstants.ActiveInactiveEnum.ACTIVE;
            var accountRole = new AccountRole
            {
                Id = Guid.NewGuid(),
                AccountId = newAccount.Id,
                Account = newAccount,
                Status = EnumConstants.ActiveInactiveEnum.ACTIVE
            };
            newAccount.AccountRoles = accountRole;
            newAccount.RoleName = accountRequestCreate.Role;

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
                    Error error = new Error
                    {
                        Code = StatusCode.BadRequest,
                        Message = "Create wallet error "
                    };
                    result.Errors.Add(error);
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
                Error error = new Error
                {
                    Code = StatusCode.BadRequest,
                    Message = response.Errors.FirstOrDefault()!.Description ?? "Create account error!"
                };
                result.Errors.Add(error);
                result.IsError = true;
            }
        }
        catch (Exception ex)
        {
            result.AddUnknownError(ex.Message);
            result.IsError = true;
            result.StatusCode = StatusCode.ServerError;
            throw;
        }

        return result;
    }


    /// <summary>
    /// Get all accounts for admin
    /// - Get all accounts
    /// - Map account to account response
    /// - Return account response
    /// </summary>
    public Task<OperationResult<IEnumerable<AccountResponse>>> GetAllAccountsForAdmin(bool? isAscending,
        string? orderBy = null, Expression<Func<Account, bool>>? filter = null,
        string[]? includeProperties = null, int pageIndex = 0, int pageSize = 10)
    {
        var result = new OperationResult<IEnumerable<AccountResponse>>();
        try
        {
            if (orderBy == null)
            {
                orderBy = "CreatedAt";
                isAscending = false;
            }

            var accounts = _unitOfWork.AccountRepository.FilterAll(isAscending, orderBy, filter, includeProperties,
                pageIndex, pageSize);
            var accountResponses = _mapper.Map<IEnumerable<AccountResponse>>(accounts);
            result.Payload = accountResponses;
            result.Message = "Get all accounts successfully";
            result.IsError = false;
        }
        catch (Exception e)
        {
            result.AddUnknownError("Get all accounts error" + e.Message);
            result.IsError = true;
            throw;
        }

        return Task.FromResult(result);
    }

    /// <summary>
    ///  Get all customers for admin
    ///  - Get all customers
    ///  - Map account to account response
    ///  - Return account response
    /// </summary>
    public async Task<OperationResult<IEnumerable<AccountResponse>>> GetAllCustomersForAdmin(bool? isAscending,
        string? orderBy = null, Expression<Func<Account, bool>>? filter = null,
        string[]? includeProperties = null, int pageIndex = 0, int pageSize = 10)
    {
        var result = new OperationResult<IEnumerable<AccountResponse>>();
        try
        {
            if (orderBy == null)
            {
                orderBy = "CreatedAt";
                isAscending = false;
            }

            var customers = await _userManager.GetUsersInRoleAsync(EnumConstants.RoleEnumString.CUSTOMER);
            IQueryable<Account> accounts = customers.AsQueryable();
            accounts = accounts.Where(filter!)
                .AsQueryable()
                .Skip(pageIndex * pageSize)
                .Take(pageSize);

            if (orderBy == null)
            {
                orderBy = "CreatedAt";
                isAscending = false;
            }

            var parameter = Expression.Parameter(typeof(Account), "x");
            var property = Expression.Property(parameter, orderBy);
            var lambda =
                Expression.Lambda<Func<Account, object>>(Expression.Convert(property, typeof(object)), parameter);

            accounts = isAscending == true ? accounts.OrderBy(lambda) : accounts.OrderByDescending(lambda);

            var accountResponses = _mapper.Map<IEnumerable<AccountResponse>>(accounts);
            result.Payload = accountResponses;
            result.Message = "Get all customers successfully";
            result.IsError = false;
        }
        catch (Exception e)
        {
            result.AddUnknownError("Get all customers error" + e.Message);
            result.IsError = true;
            throw;
        }

        return result;
    }


    /// <summary>
    ///  Get all farm hubs for admin
    ///  - Get all farm hubs
    ///  - Map account to account response
    ///  - Return account response
    /// </summary>
    public async Task<OperationResult<IEnumerable<AccountResponse>>> GetAllFarmHubsForAdmin(bool? isAscending,
        string? orderBy = null, Expression<Func<Account, bool>>? filter = null,
        string[]? includeProperties = null, int pageIndex = 0, int pageSize = 10)
    {
        var result = new OperationResult<IEnumerable<AccountResponse>>();
        try
        {
            if (orderBy == null)
            {
                orderBy = "CreatedAt";
                isAscending = false;
            }

            IQueryable<Account> accounts = _userManager.GetUsersInRoleAsync(EnumConstants.RoleEnumString.FARMHUB)
                .Result.AsQueryable();
            
            accounts = accounts.Where(filter!)
                .AsQueryable()
                .Skip(pageIndex * pageSize)
                .Take(pageSize);
            
            if (orderBy == null)
            {
                orderBy = "CreatedAt";
                isAscending = false;
            }
            
            var parameter = Expression.Parameter(typeof(Account), "x");
            var property = Expression.Property(parameter, orderBy);
            var lambda =
                Expression.Lambda<Func<Account, object>>(Expression.Convert(property, typeof(object)), parameter);
             
            accounts = isAscending == true ? accounts.OrderBy(lambda) : accounts.OrderByDescending(lambda);
            var accountResponses = _mapper.Map<IEnumerable<AccountResponse>>(accounts);
            result.Payload = accountResponses;
            result.Message = "Get all farm hubs successfully";
            result.IsError = false;
        }
        catch (Exception e)
        {
            result.AddUnknownError("Get all farm hubs error" + e.Message);
            result.IsError = true;
            throw;
        }

        return result;
    }

    /// <summary>
    ///  Get all collected staffs for admin
    ///  - Get all collected staffs
    ///  - Map account to account response
    ///  - Return account response
    ///  - If order by null, order by created at descending
    /// </summary>
    public Task<OperationResult<IEnumerable<AccountResponse>>> GetAllCollectedStaffsForAdmin(bool? isAscending,
        string? orderBy = null, Expression<Func<Account, bool>>? filter = null,
        string[]? includeProperties = null, int pageIndex = 0, int pageSize = 10)
    {
        var result = new OperationResult<IEnumerable<AccountResponse>>();
        try
        {
            if (orderBy == null)
            {
                orderBy = "CreatedAt";
                isAscending = false;
            }

            IQueryable<Account> accounts = FilterAllByRole(EnumConstants.RoleEnumString.COLLECTEDSTAFF, isAscending, orderBy, filter, includeProperties, pageIndex, pageSize);
            var accountResponses = _mapper.Map<IEnumerable<AccountResponse>>(accounts);
            result.Payload = accountResponses;
            result.Message = "Get all collected staffs successfully";
            result.IsError = false;
        }
        catch (Exception e)
        {
            result.AddUnknownError("Get all collected staffs error" + e.Message);
            result.IsError = true;
            throw;
        }

        return Task.FromResult(result);
    }

    /// <summary>
    ///  Get all station staffs for admin
    ///  - Get all station staffs
    ///  - Map account to account response
    ///  - Return account response
    /// </summary>
    public Task<OperationResult<IEnumerable<AccountResponse>>> GetAllStationStaffsForAdmin(bool? isAscending,
        string? orderBy = null, Expression<Func<Account, bool>>? filter = null,
        string[]? includeProperties = null, int pageIndex = 0, int pageSize = 10)
    {
        var result = new OperationResult<IEnumerable<AccountResponse>>();
        try
        {
            if (orderBy == null)
            {
                orderBy = "CreatedAt";
                isAscending = false;
            }

            IQueryable<Account> accounts = FilterAllByRole(EnumConstants.RoleEnumString.STATIONSTAFF, isAscending, orderBy, filter, includeProperties, pageIndex, pageSize);
            var accountResponses = _mapper.Map<IEnumerable<AccountResponse>>(accounts);
            result.Payload = accountResponses;
            result.Message = "Get all station staffs successfully";
            result.IsError = false;
        }
        catch (Exception e)
        {
            result.AddUnknownError("Get all station staffs error" + e.Message);
            result.IsError = true;
            throw;
        }

        return Task.FromResult(result);
    }


    public async Task<OperationResult<AccountResponse>> GetAccountByExpression(
        Expression<Func<Account, bool>> predicate, string[]? includeProperties = null)
    {
        var result = new OperationResult<AccountResponse>();
        try
        {
            var account = await _unitOfWork.AccountRepository.FilterByExpression(predicate, includeProperties)
                .FirstOrDefaultAsync();
            if (account == null)
            {
                result.AddError(StatusCode.NotFound, "Account not found");
                result.Message = "Account not found";
                result.IsError = true;
                return result;
            }

            var accountResponse = _mapper.Map<AccountResponse>(account);
            result.Payload = accountResponse;
            result.Message = "Get account successfully";
            result.IsError = false;
        }
        catch (Exception e)
        {
            result.AddUnknownError("Get account error " + e.Message);
            result.IsError = true;
            result.Message = "Get account error";
            throw;
        }

        return result;
    }


    /// <summary>
    ///  Delete account
    ///  - Check account exists
    ///  - Check role admin
    ///  - Soft remove account
    ///  - Save changes
    /// </summary>
    public async Task<OperationResult<bool>> DeleteAccount(Guid id)
    {
        var result = new OperationResult<bool>();
        try
        {
            var account = await _unitOfWork.AccountRepository.GetByIdAsync(id);
            if (account == null)
            {
                result.AddError(StatusCode.NotFound, "Account not found");
                result.Message = "Account not found";
                result.IsError = true;
                return result;
            }

            var role = _userManager.GetRolesAsync(account).Result.Contains(EnumConstants.RoleEnumString.ADMIN);
            if (role)
            {
                result.AddError(StatusCode.BadRequest, "Can not delete admin account");
                result.Message = "Can not delete admin account";
                result.IsError = true;
                return result;
            }

            _unitOfWork.AccountRepository.SoftRemove(account);
            await _unitOfWork.SaveChangesAsync();
            result.Payload = true;
            result.Message = "Delete account successfully";
            result.IsError = false;
        }
        catch (Exception e)
        {
            result.AddUnknownError("Delete account error " + e.Message);
            result.IsError = true;
            result.Message = "Delete account error";
            throw;
        }

        return result;
    }
    
    
    private IQueryable<Account> FilterAllByRole(string roleEnum, bool? isAscending, string? orderBy = null, Expression<Func<Account, bool>>? filter = null, string[]? includeProperties = null, int pageIndex = 0, int pageSize = 10)
    {
        IQueryable<Account> accounts = _userManager.GetUsersInRoleAsync(roleEnum)
            .Result.AsQueryable();
        accounts = accounts.Where(filter!)
            .AsQueryable()
            .Skip(pageIndex * pageSize)
            .Take(pageSize);
            
        if (orderBy == null)
        {
            orderBy = "CreatedAt";
            isAscending = false;
        }
            
        var parameter = Expression.Parameter(typeof(Account), "x");
        var property = Expression.Property(parameter, orderBy);
        var lambda =
            Expression.Lambda<Func<Account, object>>(Expression.Convert(property, typeof(object)), parameter);
        accounts = isAscending == true ? accounts.OrderBy(lambda) : accounts.OrderByDescending(lambda);

        return accounts;
    }
}
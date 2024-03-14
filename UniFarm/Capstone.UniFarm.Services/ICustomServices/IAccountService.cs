using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Services.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Capstone.UniFarm.Services.ViewModels.ModelResponses;

namespace Capstone.UniFarm.Services.ICustomServices
{
    public interface IAccountService
    {
        string GenerateJwtToken(Account user, byte[] key, string userRole);
        OperationResult<AboutMeResponse.AboutMeRoleAndID> GetIdAndRoleFromToken(string token);
        
        Task<OperationResult<AccountRequestCreate>> CreateAccount(AccountRequestCreate accountRequestCreate);
        Task<OperationResult<FarmHubRegisterRequest>> CreateFarmhubAccount(FarmHubRegisterRequest farmHubRegisterRequest);
        Task<OperationResult<Account>> HandleLoginGoogle(IEnumerable<Claim> claims);
        Task<OperationResult<AccountResponse>> GetAccountByExpression(Expression<Func<Account, bool>> predicate, string[]? includeProperties = null);
        Task<OperationResult<AccountResponse>> UpdateAccount(Guid id, AccountRequestUpdate accountRequestUpdate);
        
        Task<OperationResult<AboutMeResponse.AboutCustomerResponse>> GetAboutCustomer(Guid accountId);

        Task<OperationResult<AboutMeResponse.AboutFarmHubResponse>> GetAboutFarmHub(Guid accountId);
        Task<OperationResult<AboutMeResponse.AboutCollectedStaffResponse>> GetAboutCollectedStaff(Guid accountId);
        Task<OperationResult<AboutMeResponse.AboutStationStaffResponse>> GetAboutStationStaff(Guid accountId);
        Task<OperationResult<AboutMeResponse.AboutAdminResponse>> GetAboutAdmin(Guid accountId);
        
        Task<OperationResult<IEnumerable<AccountResponse>>> GetAllWithoutPaging(bool? isAscending, string? orderBy = null, Expression<Func<Account, bool>>? filter = null, string[]? includeProperties = null);

        Task<OperationResult<AccountResponse>> GetAccountById(Guid accountId);

    }
}

﻿using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Services.Commons;
using Capstone.UniFarm.Services.ViewModels.Account.Request;
using Capstone.UniFarm.Services.ViewModels.Account.Response;
using Capstone.UniFarm.Services.ViewModels.Authen.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Services.ICustomServices
{
    public interface IAccountService
    {
        string GenerateJwtToken(Account user, byte[] key);
        Task<OperationResult<IEnumerable<AccountResponse>>> GetAllAccounts(Expression<Func<Account, bool>>? predicate);
        Task<OperationResult<AccountResponse>> GetAccountById(Guid accountId);
        Task<OperationResult<AccountResponse>> GetAccountByPredicate(Expression<Func<Account, bool>>? predicate);
        Task<OperationResult<RegisterVM>> CreateAccount(RegisterVM registerVM);
        Task<OperationResult<bool>> DeleteAccount(Guid id);
        Task<OperationResult<AccountResponse>> UpdateAccount(Guid Id, AccountRequest accountRequest);
        Task<OperationResult<Pagination<AccountResponse>>> GetAccountPaginationAsync(int pageIndex = 0, int pageSize = 10);
        Task<OperationResult<Account>> HandleLoginGoogle(IEnumerable<Claim> claims);
    }
}

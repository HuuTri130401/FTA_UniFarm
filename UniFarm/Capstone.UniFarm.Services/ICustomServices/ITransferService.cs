using System.Linq.Expressions;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Services.Commons;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Capstone.UniFarm.Services.ViewModels.ModelResponses;

namespace Capstone.UniFarm.Services.ICustomServices;

public interface ITransferService
{
    Task<OperationResult<TransferResponse>> Create(Guid createdBy, TransferRequestCreate objectRequestCreate);

    Task<OperationResult<IEnumerable<TransferResponse>?>?> GetAll(bool? isAscending, string? orderBy = null,
        Expression<Func<Transfer, bool>>? filter = null, string[]? includeProperties = null, int pageIndex = 0,
        int pageSize = 10);

    Task<OperationResult<TransferResponse?>> UpdateStatus(AboutMeResponse.AboutMeRoleAndID defineUser, TransferRequestUpdate request);
    Task<OperationResult<TransferResponse>>  ResendTransfer(Guid createdBy, Guid transferId);
}
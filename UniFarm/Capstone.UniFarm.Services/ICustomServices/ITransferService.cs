using System.Linq.Expressions;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Services.Commons;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Capstone.UniFarm.Services.ViewModels.ModelResponses;

namespace Capstone.UniFarm.Services.ICustomServices;

public interface ITransferService
{
    Task<OperationResult<TransferResponse>> Create(Guid createdBy, TransferRequestCreate objectRequestCreate);

    Task<OperationResult<IEnumerable<TransferResponse>>> GetAll(
        string? keyword,
        Guid? collectedId,
        Guid? stationId,
        string? status,
        string? code,
        Guid? createdBy,
        Guid? updatedBy,
        DateTime? fromDate,
        DateTime? toDate,
        bool? isAscending,
        string? orderBy,
        int page = 1,
        int pageSize = 10
    );
}
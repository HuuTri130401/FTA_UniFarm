using System.Linq.Expressions;
using AutoMapper;
using Capstone.UniFarm.Domain.Enum;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Repositories.UnitOfWork;
using Capstone.UniFarm.Services.Commons;
using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Capstone.UniFarm.Services.ViewModels.ModelResponses;
using Microsoft.EntityFrameworkCore;

namespace Capstone.UniFarm.Services.CustomServices;

public class TransferService : ITransferService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public TransferService(
        IUnitOfWork unitOfWork,
        IMapper mapper
    )
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<OperationResult<TransferResponse>> Create(Guid createdBy,
        TransferRequestCreate objectRequestCreate)
    {
        var result = new OperationResult<TransferResponse>();
        var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            foreach (var orderId in objectRequestCreate.OrderIds)
            {
                var order = await _unitOfWork.OrderRepository.FilterByExpression(
                    x => x.Id == orderId
                    /*&& x.IsPaid == true*/
                ).FirstOrDefaultAsync();
                if (order == null)
                {
                    return new OperationResult<TransferResponse>()
                    {
                        StatusCode = StatusCode.NotFound,
                        Message = EnumConstants.OrderMessage.ORDER_NOT_FOUND + orderId,
                        IsError = true
                    };
                }
            }

            var checkCollected = await _unitOfWork.CollectedHubRepository
                .FilterByExpression(x => x.Id == objectRequestCreate.CollectedId).FirstOrDefaultAsync();
            if (checkCollected == null)
            {
                return new OperationResult<TransferResponse>()
                {
                    StatusCode = StatusCode.NotFound,
                    Message = EnumConstants.CollectedHubMessage.COLLECTED_NOT_FOUND + objectRequestCreate.CollectedId,
                    IsError = true
                };
            }

            var checkStation = await _unitOfWork.StationRepository
                .FilterByExpression(x => x.Id == objectRequestCreate.StationId).FirstOrDefaultAsync();
            if (checkStation == null)
            {
                return new OperationResult<TransferResponse>()
                {
                    StatusCode = StatusCode.NotFound,
                    Message = EnumConstants.StationMessage.STATION_NOT_FOUND + objectRequestCreate.StationId,
                    IsError = true
                };
            }

            var transfer = _mapper.Map<Transfer>(objectRequestCreate);
            transfer.CreatedBy = createdBy;
            transfer.Code = "TRANSFER_BY" + createdBy + "_FROM_" + objectRequestCreate.CollectedId + "_" +
                            DateTime.Now.ToString("yyyyMMddHHmmss");
            await _unitOfWork.TransferRepository.AddAsync(transfer);
            var count = await _unitOfWork.SaveChangesAsync();
            if (count == 0)
            {
                await transaction.RollbackAsync();
                result.AddError(StatusCode.UnknownError, "Create transfer failed");
                result.IsError = true;
                result.StatusCode = StatusCode.ServerError;
                return result;
            }

            foreach (var orderId in objectRequestCreate.OrderIds)
            {
                var order = await _unitOfWork.OrderRepository.FilterByExpression(
                    x => x.Id == orderId
                    /*&& x.IsPaid == true*/
                ).FirstOrDefaultAsync();
                if (order != null)
                {
                    order.TransferId = transfer.Id;
                    await _unitOfWork.OrderRepository.UpdateAsync(order);
                    var countUpdate = await _unitOfWork.SaveChangesAsync();
                    if (countUpdate == 0)
                    {
                        await transaction.RollbackAsync();
                        result.AddError(StatusCode.UnknownError, "Update TransferId for order " + orderId + " failed");
                        result.Message = "Update TransferId for order " + orderId + " failed";
                        result.IsError = true;
                        result.StatusCode = StatusCode.ServerError;
                        return result;
                    }
                }
            }

            transfer.Status = EnumConstants.TransferStatusEnum.PENDING;
            var orderResponses = new List<OrderResponse.OrderResponseForCustomer>();

            var transferResponse = new TransferResponse(
                transfer.Id,
                transfer.CollectedId,
                transfer.StationId,
                transfer.CreatedAt,
                transfer.UpdatedAt,
                transfer.ExpectedReceiveDate,
                transfer.ReceivedDate,
                transfer.CreatedBy,
                transfer.UpdatedBy,
                transfer.NoteSend,
                transfer.NoteReceived,
                transfer.Code,
                transfer.Status,
                _mapper.Map<CollectedHubResponse>(checkCollected),
                null,
                null);


            transferResponse.Orders = new List<OrderResponse.OrderResponseForCustomer>();
            foreach (var orderId in objectRequestCreate.OrderIds)
            {
                var order = await _unitOfWork.OrderRepository.FilterByExpression(
                    x => x.Id == orderId
                    /*&& x.IsPaid == true*/
                ).FirstOrDefaultAsync();
                if (order != null)
                {
                    var orderResponse = new OrderResponse.OrderResponseForCustomer(
                        order.Id,
                        order.FarmHubId,
                        order.CustomerId,
                        order.StationId,
                        order.BusinessDayId,
                        order.CreatedAt,
                        order.Code,
                        order.ShipAddress,
                        order.TotalAmount,
                        order.IsPaid,
                        _mapper.Map<FarmHubResponse>(order.FarmHub),
                        _mapper.Map<BusinessDayResponse>(order.BusinessDay),
                        _mapper.Map<StationResponse.StationResponseSimple>(checkStation),
                        null
                    );

                    var station = await _unitOfWork.StationRepository.FilterByExpression(x => x.Id == order.StationId)
                        .FirstOrDefaultAsync();
                    orderResponse.StationResponse = _mapper.Map<StationResponse.StationResponseSimple>(station);
                    orderResponse.FarmHubResponse = _mapper.Map<FarmHubResponse>(order.FarmHub);
                    orderResponse.BusinessDayResponse = _mapper.Map<BusinessDayResponse>(order.BusinessDay);
                    transferResponse.Orders.Add(orderResponse);
                }
            }

            await transaction.CommitAsync();
            result.Payload = transferResponse;
            result.StatusCode = StatusCode.Created;
            result.Message = EnumConstants.TransferMessage.CREATE_TRANSFER_SUCCESS;
            result.IsError = false;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            result.AddUnknownError(ex.Message);
            result.IsError = true;
            result.StatusCode = StatusCode.ServerError;
            throw;
        }

        return result;
    }

    public async Task<OperationResult<IEnumerable<TransferResponse>?>?> GetAll(
        bool? isAscending,
        string? orderBy = null,
        Expression<Func<Transfer, bool>>? filter = null,
        string[]? includeProperties = null,
        int pageIndex = 0,
        int pageSize = 10)
    {
        var result = new OperationResult<IEnumerable<TransferResponse>?>();
        try
        {
            var getTransfers = await _unitOfWork.TransferRepository.FilterAll(
                isAscending: isAscending,
                orderBy: orderBy,
                predicate: filter,
                includeProperties: includeProperties,
                pageIndex: pageIndex,
                pageSize: pageSize
            ).ToListAsync();

            if (getTransfers.Count == 0)
            {
                result.StatusCode = StatusCode.Ok;
                result.Message = "There is no transfer found";
                result.IsError = false;
                result.Payload = null;
                return result;
            }

            var transferResponses = new List<TransferResponse>();
            foreach (var transfer in getTransfers)
            {
                var transferResponse = new TransferResponse(
                    transfer.Id,
                    transfer.CollectedId,
                    transfer.StationId,
                    transfer.CreatedAt,
                    transfer.UpdatedAt,
                    transfer.ExpectedReceiveDate,
                    transfer.ReceivedDate,
                    transfer.CreatedBy,
                    transfer.UpdatedBy,
                    transfer.NoteSend,
                    transfer.NoteReceived,
                    transfer.Code,
                    transfer.Status,
                    null,
                    null,
                    null);

                var collected = await _unitOfWork.CollectedHubRepository
                    .FilterByExpression(x => x.Id == transfer.CollectedId).FirstOrDefaultAsync();
                transferResponse.Collected = _mapper.Map<CollectedHubResponse>(collected);
                var station = await _unitOfWork.StationRepository.FilterByExpression(x => x.Id == transfer.StationId)
                    .FirstOrDefaultAsync();
                transferResponse.Station = _mapper.Map<StationResponse.StationResponseSimple>(station);

                var orders = await _unitOfWork.OrderRepository.FilterByExpression(x => x.TransferId == transfer.Id)
                    .ToListAsync();

                transferResponses.Add(transferResponse);
                result.Payload = transferResponses;
                result.StatusCode = StatusCode.Ok;
                result.Message = EnumConstants.TransferMessage.GET_ALL_TRANSFER_SUCCESS;
                result.IsError = false;
            }

            result.Payload = transferResponses;
            result.StatusCode = StatusCode.Ok;
            result.Message = EnumConstants.TransferMessage.GET_ALL_TRANSFER_SUCCESS;
            result.IsError = false;
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

    public async Task<OperationResult<TransferResponse?>> UpdateStatus(AboutMeResponse.AboutMeRoleAndID defineUser,
        TransferRequestUpdate request)
    {
        var result = new OperationResult<TransferResponse?>();
        var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var transfer = await _unitOfWork.TransferRepository.FilterByExpression(x => x.Id == request.Id)
                .FirstOrDefaultAsync();

            if (transfer == null)
            {
                result.StatusCode = StatusCode.NotFound;
                result.Message = EnumConstants.TransferMessage.TRANSFER_NOT_FOUND + request.Id;
                result.IsError = false;
                result.Payload = null;
                return result;
            }

            if (defineUser.AuthorizationDecision != transfer!.StationId.ToString())
            {
                result.StatusCode = StatusCode.UnAuthorize;
                result.Message = "Only staffs in this station have permission to update this transfer";
                result.IsError = true;
                result.Payload = null;
                return result;
            }

            if (transfer.Status == request.Status)
            {
                result.StatusCode = StatusCode.BadRequest;
                result.Message = "Status of transfer is already " + request.Status;
                result.IsError = true;
                result.Payload = null;
                return result;
            }

            transfer.UpdatedAt = DateTime.Now;
            transfer.UpdatedBy = defineUser.Id;
            transfer.Status = request.Status;
            if (request.Status == EnumConstants.TransferStatusEnum.RECEIVED)
            {
                transfer.ReceivedDate = DateTime.Now;
                transfer.NoteReceived = request.NoteReceived;
            }

            await _unitOfWork.TransferRepository.UpdateAsync(transfer);
            var count = await _unitOfWork.SaveChangesAsync();

            if (count == 0)
            {
                await transaction.RollbackAsync();
                result.StatusCode = StatusCode.UnknownError;
                result.Message = "Update transfer failed";
                result.IsError = true;
                result.Payload = null;
                return result;
            }

            var orders = await _unitOfWork.OrderRepository.FilterByExpression(x => x.TransferId == request.Id)
                .ToListAsync();
            foreach (var order in orders)
            {
                if (request.Status == EnumConstants.TransferStatusEnum.RECEIVED)
                {
                    order.CustomerStatus = EnumConstants.OrderCustomerStatus.CHO_NHAN_HANG;
                }
                else
                {
                    order.CustomerStatus = EnumConstants.OrderCustomerStatus.DANG_VAN_CHUYEN;
                }

                await _unitOfWork.OrderRepository.UpdateAsync(order);

                var countUpdate = await _unitOfWork.SaveChangesAsync();
                if (countUpdate == 0)
                {
                    await transaction.RollbackAsync();
                    result.StatusCode = StatusCode.UnknownError;
                    result.Message = "Update status for order " + order.Id + " failed";
                    result.IsError = true;
                    result.Payload = null;
                    return result;
                }
            }

            var collected = await _unitOfWork.CollectedHubRepository
                .FilterByExpression(x => x.Id == transfer.CollectedId).FirstOrDefaultAsync();
            var station = _unitOfWork.StationRepository.FilterByExpression(x => x.Id == transfer.StationId).FirstOrDefaultAsync();
            var collectedResponse = _mapper.Map<CollectedHubResponse>(collected);
            var orderResponses = new List<OrderResponse.OrderResponseForCustomer>();

            foreach (var order in orders)
            {
                var orderResponse = new OrderResponse.OrderResponseForCustomer(
                    order.Id,
                    order.FarmHubId,
                    order.CustomerId,
                    order.StationId,
                    order.BusinessDayId,
                    order.CreatedAt,
                    order.Code,
                    order.ShipAddress,
                    order.TotalAmount,
                    order.IsPaid,
                    null,
                    null,
                    null,
                    null
                );
                
                orderResponses.Add(orderResponse);
            }


            var transferResponse = new TransferResponse(
                transfer.Id,
                transfer.CollectedId,
                transfer.StationId,
                transfer.CreatedAt,
                transfer.UpdatedAt,
                transfer.ExpectedReceiveDate,
                transfer.ReceivedDate,
                transfer.CreatedBy,
                transfer.UpdatedBy,
                transfer.NoteSend,
                transfer.NoteReceived,
                transfer.Code,
                transfer.Status,
                collectedResponse,
                null,
                orderResponses);
            result.Payload = transferResponse;
            result.StatusCode = StatusCode.Ok;
            result.Message = EnumConstants.TransferMessage.UPDATE_TRANSFER_STATUS_SUCCESS;
            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            result.AddUnknownError(e.Message);
            result.IsError = true;
            result.StatusCode = StatusCode.ServerError;
            throw;
        }
        return result;
    }
}
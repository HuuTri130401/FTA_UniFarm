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
                    x => x.Id == Guid.Parse(orderId)
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
                    x => x.Id == Guid.Parse(orderId)
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
                transfer.ReceivedDate,
                transfer.CreatedBy,
                transfer.UpdatedBy,
                transfer.NoteSend,
                transfer.NoteReceived,
                transfer.Code,
                transfer.Status,
                _mapper.Map<CollectedHubResponse>(checkCollected),
                _mapper.Map<StationResponse>(checkStation),
                null);


            transferResponse.Orders = new List<OrderResponse.OrderResponseForCustomer>();
            foreach (var orderId in objectRequestCreate.OrderIds)
            {
                var order = await _unitOfWork.OrderRepository.FilterByExpression(
                    x => x.Id == Guid.Parse(orderId)
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
                        _mapper.Map<StationResponse>(checkStation),
                        null
                    );

                    var station = await _unitOfWork.StationRepository.FilterByExpression(x => x.Id == order.StationId)
                        .FirstOrDefaultAsync();
                    orderResponse.StationResponse = _mapper.Map<StationResponse>(station);
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

    public async Task<OperationResult<IEnumerable<TransferResponse>>> GetAll(
        string? keyword, Guid? collectedId, Guid? stationId,
        string? status, string? code,
        Guid? createdBy, Guid? updatedBy,
        DateTime? fromDate, DateTime? toDate,
        bool? isAscending,
        string? orderBy,
        int pageIndex = 1,
        int pageSize = 10)
    {
        var result = new OperationResult<IEnumerable<TransferResponse>>();
        try
        {
            var getTransfers = await _unitOfWork.TransferRepository.GetAllAsync();

            var transferResponses = new List<TransferResponse>();
            foreach (var transfer in getTransfers)
            {
                var transferResponse = new TransferResponse(
                    transfer.Id,
                    transfer.CollectedId,
                    transfer.StationId,
                    transfer.CreatedAt,
                    transfer.UpdatedAt,
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

                var collected =  await _unitOfWork.CollectedHubRepository.FilterByExpression(x => x.Id == transfer.CollectedId).FirstOrDefaultAsync();
                transferResponse.Collected = _mapper.Map<CollectedHubResponse>(collected);
                var station = await _unitOfWork.StationRepository.FilterByExpression(x => x.Id == transfer.StationId)
                    .FirstOrDefaultAsync();
                transferResponse.Station = _mapper.Map<StationResponse>(station);

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
}
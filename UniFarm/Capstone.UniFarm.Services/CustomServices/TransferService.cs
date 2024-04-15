using System.Linq.Expressions;
using AutoMapper;
using Capstone.UniFarm.Domain.Enum;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Repositories.UnitOfWork;
using Capstone.UniFarm.Services.Commons;
using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ThirdPartyService;
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
            if (objectRequestCreate.OrderIds.Length == 0)
            {
                result.Errors.Add(new Error()
                {
                    Code = StatusCode.BadRequest,
                    Message = "OrderIds is required"
                });
                result.IsError = true;
                result.StatusCode = StatusCode.BadRequest;
                return result;
            }

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
            transfer.Code = "TRA" + Utils.RandomString(5);
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
                    order.DeliveryStatus = EnumConstants.DeliveryStatus.OnTheWayToStation.ToString();
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

            transfer.Status = EnumConstants.StationUpdateTransfer.Pending.ToString();
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
                    var station = await _unitOfWork.StationRepository.FilterByExpression(x => x.Id == order.StationId)
                        .FirstOrDefaultAsync();

                    var orderDetail = await _unitOfWork.OrderDetailRepository
                        .FilterByExpression(x => x.OrderId == order.Id)
                        .ToListAsync();
                    var orderDetailResponses = new List<OrderDetailResponseForCustomer>();
                    foreach (var detail in orderDetail)
                    {
                        var product = await _unitOfWork.ProductItemRepository
                            .FilterByExpression(x => x.Id == detail.ProductItemId)
                            .FirstOrDefaultAsync();
                        var productItemResponse = new ProductItemResponseForCustomer();
                        if (product != null)
                        {
                            productItemResponse = _mapper.Map<ProductItemResponseForCustomer>(product);
                        }

                        var orderDetailResponse = new OrderDetailResponseForCustomer
                        {
                            OrderId = detail.OrderId,
                            ProductItemId = detail.ProductItemId,
                            Quantity = detail.Quantity,
                            UnitPrice = detail.UnitPrice,
                            Unit = detail.Unit,
                            TotalPrice = detail.TotalPrice,
                            ProductItemResponse = productItemResponse
                        };
                        orderDetailResponses.Add(orderDetailResponse);
                    }

                    var orderResponse = new OrderResponse.OrderResponseForCustomer()
                    {
                        Id = order.Id,
                        FarmHubId = order.FarmHubId,
                        CustomerId = order.CustomerId,
                        StationId = order.StationId,
                        BusinessDayId = order.BusinessDayId,
                        CreatedAt = order.CreatedAt,
                        Code = order.Code,
                        ShipAddress = order.ShipAddress,
                        TotalAmount = order.TotalAmount,
                        IsPaid = order.IsPaid,
                        FullName = order.FullName,
                        PhoneNumber = order.PhoneNumber,
                        FarmHubResponse = _mapper.Map<FarmHubResponse>(order.FarmHub),
                        BusinessDayResponse = _mapper.Map<BusinessDayResponse>(order.BusinessDay),
                        StationResponse = _mapper.Map<StationResponse.StationResponseSimple>(station),
                        OrderDetailResponse = orderDetailResponses
                    };
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

            if (transfer.Status == EnumConstants.StationUpdateTransfer.Resend.ToString())
            {
                result.Errors.Add(new Error()
                {
                    Code = StatusCode.BadRequest,
                    Message = "This transfer is already sent to station"
                });
                result.IsError = true;
                result.StatusCode = StatusCode.BadRequest;
                return result;
            }

            if (transfer.Status == EnumConstants.StationUpdateTransfer.Received.ToString() ||
                transfer.Status == EnumConstants.StationUpdateTransfer.NotReceived.ToString())
            {
                result.Errors.Add(new Error()
                {
                    Code = StatusCode.BadRequest,
                    Message = "Status of transfer is already updated!"
                });
                result.IsError = true;
                result.Payload = null;
                return result;
            }

            transfer.UpdatedAt = DateTime.Now;
            transfer.UpdatedBy = defineUser.Id;
            transfer.Status = request.Status.ToString();
            if (request.Status.ToString() == EnumConstants.StationUpdateTransfer.Received.ToString())
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

            var orders = await _unitOfWork.OrderRepository.FilterByExpression(x => x.TransferId == request.Id
                    && x.DeliveryStatus == EnumConstants.DeliveryStatus.OnTheWayToStation.ToString()
                    && x.CollectedHubId == transfer.CollectedId
                    && x.IsPaid == true
                )
                .ToListAsync();

            var collected = await _unitOfWork.CollectedHubRepository
                .FilterByExpression(x => x.Id == transfer.CollectedId).FirstOrDefaultAsync();
            var station = await _unitOfWork.StationRepository.FilterByExpression(x => x.Id == transfer.StationId)
                .FirstOrDefaultAsync();
            var stationResponse = _mapper.Map<StationResponse.StationResponseSimple>(station);
            var collectedResponse = _mapper.Map<CollectedHubResponse>(collected);
            var orderResponses = new List<OrderResponse.OrderResponseForCustomer>();

            foreach (var order in orders)
            {
                var orderDetail = await _unitOfWork.OrderDetailRepository.FilterByExpression(x => x.OrderId == order.Id)
                    .ToListAsync();
                var orderDetailResponses = new List<OrderDetailResponseForCustomer>();
                foreach (var detail in orderDetail)
                {
                    var product = await _unitOfWork.ProductItemRepository
                        .FilterByExpression(x => x.Id == detail.ProductItemId)
                        .FirstOrDefaultAsync();
                    var productItemResponse = new ProductItemResponseForCustomer();
                    if (product != null)
                    {
                        productItemResponse = _mapper.Map<ProductItemResponseForCustomer>(product);
                    }

                    var orderDetailResponse = new OrderDetailResponseForCustomer
                    {
                        Id = detail.Id,
                        OrderId = detail.OrderId,
                        ProductItemId = detail.ProductItemId,
                        Quantity = detail.Quantity,
                        UnitPrice = detail.UnitPrice,
                        Unit = detail.Unit,
                        TotalPrice = detail.TotalPrice,
                        ProductItemResponse = productItemResponse
                    };
                    orderDetailResponses.Add(orderDetailResponse);
                }

                var orderResponse = new OrderResponse.OrderResponseForCustomer()
                {
                    Id = order.Id,
                    FarmHubId = order.FarmHubId,
                    CustomerId = order.CustomerId,
                    StationId = order.StationId,
                    BusinessDayId = order.BusinessDayId,
                    CreatedAt = order.CreatedAt,
                    Code = order.Code,
                    ShipAddress = order.ShipAddress,
                    TotalAmount = order.TotalAmount,
                    IsPaid = order.IsPaid,
                    FullName = order.FullName,
                    PhoneNumber = order.PhoneNumber,
                    FarmHubResponse = _mapper.Map<FarmHubResponse>(order.FarmHub),
                    BusinessDayResponse = _mapper.Map<BusinessDayResponse>(order.BusinessDay),
                    StationResponse = stationResponse,
                    OrderDetailResponse = orderDetailResponses
                };
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
                stationResponse,
                orderResponses);
            await transaction.CommitAsync();
            result.Payload = transferResponse;
            result.StatusCode = StatusCode.Ok;
            result.Message = EnumConstants.TransferMessage.UPDATE_TRANSFER_STATUS_SUCCESS;
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

    public async Task<OperationResult<TransferResponse>> ResendTransfer(Guid createdBy, Guid transferId)
    {
        var result = new OperationResult<TransferResponse>();
        try
        {
            var transfer = await _unitOfWork.TransferRepository.FilterByExpression(x => x.Id == transferId)
                .FirstOrDefaultAsync();
            if (transfer == null)
            {
                result.Errors.Add(new Error()
                {
                    Code = StatusCode.NotFound,
                    Message = EnumConstants.TransferMessage.TRANSFER_NOT_FOUND + transferId
                });
                result.IsError = true;
                result.StatusCode = StatusCode.NotFound;
                return result;
            }

            if (transfer.Status != EnumConstants.StationUpdateTransfer.NotReceived.ToString())
            {
                result.Errors.Add(new Error()
                {
                    Code = StatusCode.BadRequest,
                    Message = "This transfer is already sent to station"
                });
                result.IsError = true;
                result.StatusCode = StatusCode.BadRequest;
                return result;
            }

            transfer.Status = EnumConstants.StationUpdateTransfer.Resend.ToString();
            transfer.UpdatedAt = DateTime.Now;
            transfer.UpdatedBy = createdBy;
            await _unitOfWork.TransferRepository.UpdateAsync(transfer);
            var count = await _unitOfWork.SaveChangesAsync();
            if (count == 0)
            {
                result.Errors.Add(new Error()
                {
                    Code = StatusCode.BadRequest,
                    Message = "Resend transfer failed"
                });
                result.IsError = true;
                result.StatusCode = StatusCode.BadRequest;
                return result;
            }

            var collected = await _unitOfWork.CollectedHubRepository
                .FilterByExpression(x => x.Id == transfer.CollectedId).FirstOrDefaultAsync();
            var station = await _unitOfWork.StationRepository.FilterByExpression(x => x.Id == transfer.StationId)
                .FirstOrDefaultAsync();
            var stationResponse = _mapper.Map<StationResponse.StationResponseSimple>(station);
            var collectedResponse = _mapper.Map<CollectedHubResponse>(collected);
            var orders = await _unitOfWork.OrderRepository.FilterByExpression(x => x.TransferId == transfer.Id)
                .ToListAsync();
            var orderResponses = new List<OrderResponse.OrderResponseForCustomer>();

            foreach (var order in orders)
            {
                var orderDetail = await _unitOfWork.OrderDetailRepository.FilterByExpression(x => x.OrderId == order.Id)
                    .ToListAsync();
                var orderDetailResponses = new List<OrderDetailResponseForCustomer>();
                foreach (var detail in orderDetail)
                {
                    var product = await _unitOfWork.ProductItemRepository
                        .FilterByExpression(x => x.Id == detail.ProductItemId)
                        .FirstOrDefaultAsync();
                    var productItemResponse = new ProductItemResponseForCustomer();
                    if (product != null)
                    {
                        productItemResponse = _mapper.Map<ProductItemResponseForCustomer>(product);
                    }

                    var orderDetailResponse = new OrderDetailResponseForCustomer
                    {
                        Id = detail.Id,
                        OrderId = detail.OrderId,
                        ProductItemId = detail.ProductItemId,
                        Quantity = detail.Quantity,
                        UnitPrice = detail.UnitPrice,
                        Unit = detail.Unit,
                        TotalPrice = detail.TotalPrice,
                        ProductItemResponse = productItemResponse
                    };
                    orderDetailResponses.Add(orderDetailResponse);
                }

                var orderResponse = new OrderResponse.OrderResponseForCustomer()
                {
                    Id = order.Id,
                    FarmHubId = order.FarmHubId,
                    CustomerId = order.CustomerId,
                    StationId = order.StationId,
                    BusinessDayId = order.BusinessDayId,
                    CreatedAt = order.CreatedAt,
                    Code = order.Code,
                    ShipAddress = order.ShipAddress,
                    TotalAmount = order.TotalAmount,
                    IsPaid = order.IsPaid,
                    FullName = order.FullName,
                    PhoneNumber = order.PhoneNumber,
                    FarmHubResponse = _mapper.Map<FarmHubResponse>(order.FarmHub),
                    BusinessDayResponse = _mapper.Map<BusinessDayResponse>(order.BusinessDay),
                    StationResponse = stationResponse,
                    OrderDetailResponse = orderDetailResponses
                };
                orderResponses.Add(orderResponse);
            }

            result.StatusCode = StatusCode.Ok;
            result.Message = EnumConstants.TransferMessage.RESEND_TRANSFER_SUCCESS;
            result.IsError = false;
            result.Payload = new TransferResponse(
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
                stationResponse,
                orderResponses);
        }
        catch (Exception e)
        {
            result.Errors.Add(new Error()
            {
                Code = StatusCode.ServerError,
                Message = e.Message
            });
            result.IsError = true;
            result.StatusCode = StatusCode.ServerError;
            throw;
        }

        return result;
    }
}
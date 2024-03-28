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
using OrderDetail = Capstone.UniFarm.Domain.Models.OrderDetail;

namespace Capstone.UniFarm.Services.CustomServices;

public class CartService : ICartService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CartService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<OperationResult<OrderResponse.OrderResponseForCustomer?>> CheckExistCart(
        Guid customerId,
        Guid farmHubId,
        Guid productItemId,
        Guid stationId,
        Guid businessDayId,
        bool isPaid = false
    )
    {
        var result = new OperationResult<OrderResponse.OrderResponseForCustomer?>();
        try
        {
            var order = await _unitOfWork.OrderRepository.FilterByExpression(
                predicate: x => x.CustomerId == customerId
                                && x.FarmHubId == farmHubId
                                && x.IsPaid == isPaid
                                && x.StationId == stationId
                                && x.BusinessDayId == businessDayId
                /*&& x.CreatedAt.Date == DateTime.Now.Date*/
            ).FirstOrDefaultAsync();
            if (order == null)
            {
                result.Message = EnumConstants.NotificationMessage
                    .CART_DOES_NOT_EXIST_WITH_SAME_PRODUCTITEMID_AND_FARMHUBID_STATIONID_BUSINESSDAYID;
                result.StatusCode = StatusCode.NotFound;
                result.Payload = null;
                result.IsError = true;
                return result;
            }

            // Chưa set điều kiện check ngày hôm nay
            var productItemInMenu = _unitOfWork.ProductItemInMenuRepository
                .FilterByExpression(
                    predicate: x => x.ProductItemId == productItemId
                    /*&& x.Status == EnumConstants.ActiveInactiveEnum.ACTIVE*/
                ).FirstOrDefaultAsync().Result;

            if (productItemInMenu == null)
            {
                result.Message = EnumConstants.NotificationMessage.PRODUCT_ITEM_IN_MENU_DOES_NOT_EXIST;
                result.StatusCode = StatusCode.NotFound;
                result.Payload = null;
                return result;
            }

            var orderDetail = _unitOfWork.OrderDetailRepository
                .FilterByExpression(
                    predicate: x => x.OrderId == order.Id
                                    && x.ProductItemId == productItemId
                ).FirstOrDefaultAsync().Result;
            if (orderDetail == null)
            {
                result.Message = EnumConstants.NotificationMessage.ORDER_DETAIL_DOES_NOT_EXIST;
                result.StatusCode = StatusCode.NotFound;
                result.Payload = null;
                return result;
            }

            // Check Status trong ProductItem trước khi thêm toàn bộ dữ liệu
            var productItem = _unitOfWork.ProductItemRepository
                .FilterByExpression(
                    predicate: x => x.Id == productItemId
                    /*
                    && x.Status == EnumConstants.ActiveInactiveEnum.ACTIVE*/
                ).FirstOrDefaultAsync().Result;

            if (productItem == null)
            {
                result.Message = EnumConstants.NotificationMessage.PRODUCT_ITEM_DOES_NOT_EXIST_OR_INACTIVE;
                result.StatusCode = StatusCode.NotFound;
                result.Payload = null;
                return result;
            }

            var farmHub = _unitOfWork.FarmHubRepository
                .FilterByExpression(
                    predicate: x => x.Id == farmHubId && x.Status == EnumConstants.ActiveInactiveEnum.ACTIVE
                ).FirstOrDefaultAsync().Result;

            if (farmHub == null)
            {
                result.Message = EnumConstants.NotificationMessage.FARMHUB_DOES_NOT_EXIST;
                result.StatusCode = StatusCode.NotFound;
                result.Payload = null;
                return result;
            }

            var station = _unitOfWork.StationRepository.FilterByExpression(
                x => x.Id == stationId
                     && x.Status == EnumConstants.ActiveInactiveEnum.ACTIVE).FirstOrDefaultAsync().Result;
            if (station == null)
            {
                result.Message = EnumConstants.NotificationMessage.STATION_DOES_NOT_EXIST;
                result.StatusCode = StatusCode.NotFound;
                result.Payload = null;
                return result;
            }

            var businessDay = _unitOfWork.BusinessDayRepository.FilterByExpression(
                x => x.Id == businessDayId
                /*&& x.OpenDay == DateTime.Now.Date
                && x.Status == EnumConstants.ActiveInactiveEnum.ACTIVE*/
            ).FirstOrDefaultAsync().Result;

            var stationResponse = _mapper.Map<StationResponse.StationResponseSimple>(station);
            var businessDayResponse = _mapper.Map<BusinessDayResponse>(businessDay);
            var productItemResponse = _mapper.Map<ProductItemResponseForCustomer>(productItem);
            var farmHubResponse = _mapper.Map<FarmHubResponse>(farmHub);
            var orderDetailResponse = _mapper.Map<OrderDetailResponseForCustomer>(orderDetail);
            orderDetailResponse.ProductItemResponse = productItemResponse;

            result.Payload = new OrderResponse.OrderResponseForCustomer(
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
                farmHubResponse,
                businessDayResponse,
                stationResponse,
                orderDetailResponse
            );
            result.Message = EnumConstants.NotificationMessage.CART_EXIST_WITH_SAME_PRODUCTITEMID_AND_FARMHUBID;
            result.StatusCode = StatusCode.Ok;
            result.IsError = false;
        }
        catch (Exception e)
        {
            result.Message = e.Message;
            result.StatusCode = StatusCode.ServerError;
            result.Payload = null;
        }

        return result;
    }


    /// <summary>
    ///   Thêm hoặc cập nhật sản phẩm vào giỏ hàng
    ///   Nếu giỏ hàng chưa tồn tại thì tạo mới
    ///   Nếu giỏ hàng đã tồn tại thì cập nhật số lượng sản phẩm
    ///   Nếu sản phẩm không tồn tại thì trả về thông báo lỗi
    ///   Nếu farmHub không tồn tại thì trả về thông báo lỗi
    ///   Nếu orderDetail không tồn tại thì trả về thông báo lỗi
    ///   Nếu cập nhật thành công thì trả về thông báo thành công
    ///  + Check số lượng trong ProductItemtInMenu
    ///  + Lấy ra order theo ngày
    ///  + Check Status trong ProductItemInMenu trước khi thêm toàn bộ dữ liệu
    ///  + Check Status trong ProductItem trước khi thêm toàn bộ dữ liệu
    ///  + Check Status trong FarmHub trước khi thêm toàn bộ dữ liệu
    ///  + Check Status trong OrderDetail trước khi thêm toàn bộ dữ liệu
    /// </summary>
    public async Task<OperationResult<OrderResponse.OrderResponseForCustomer?>> UpsertToCart(Guid customerId,
        AddToCartRequest request)
    {
        var result = await CheckExistCart(customerId, request.FarmHubId, request.ProductItemId, request.StationId,
            request.BusinessDayId, false);
        var transaction = await _unitOfWork.BeginTransactionAsync();

        var productItemInMenu = _unitOfWork.ProductItemInMenuRepository
            .FilterByExpression(
                predicate: x => x.ProductItemId == request.ProductItemId
                /*&& x.Status == EnumConstants.ActiveInactiveEnum.ACTIVE*/
            ).FirstOrDefaultAsync().Result;
        if (productItemInMenu == null)
        {
            await transaction.RollbackAsync();
            return new OperationResult<OrderResponse.OrderResponseForCustomer?>
            {
                Message = EnumConstants.NotificationMessage.PRODUCT_ITEM_IN_MENU_DOES_NOT_EXIST,
                StatusCode = StatusCode.NotFound,
                Payload = null,
                IsError = true
            };
        }

        var productItemAndFarmHub = _unitOfWork.ProductItemRepository.FilterByExpression(
            predicate: x => x.Id == request.ProductItemId
                            && x.FarmHubId == request.FarmHubId
            /*&& x.Status == EnumConstants.ActiveInactiveEnum.ACTIVE*/
        ).FirstOrDefaultAsync().Result;

        if (productItemAndFarmHub == null)
        {
            await transaction.RollbackAsync();
            return new OperationResult<OrderResponse.OrderResponseForCustomer?>
            {
                Message = EnumConstants.NotificationMessage.PRODUCT_ITEM_AND_FARMHUBID_DOES_NOT_EXIST,
                StatusCode = StatusCode.NotFound,
                Payload = null,
                IsError = true
            };
        }

        var station = _unitOfWork.StationRepository.FilterByExpression(x =>
                    x.Id == request.StationId
                /*&& x.Status == EnumConstants.ActiveInactiveEnum.ACTIVE*/
            ).FirstOrDefaultAsync()
            .Result;
        if (station == null)
        {
            await transaction.RollbackAsync();
            return new OperationResult<OrderResponse.OrderResponseForCustomer?>
            {
                Message = EnumConstants.NotificationMessage.STATION_DOES_NOT_EXIST,
                StatusCode = StatusCode.NotFound,
                Payload = null,
                IsError = true
            };
        }

        var businessDay = _unitOfWork.BusinessDayRepository.FilterByExpression(
            x => x.Id == request.BusinessDayId
            /*&& x.OpenDay == DateTime.Now.Date*/
        ).FirstOrDefaultAsync().Result;
        if (businessDay == null)
        {
            await transaction.RollbackAsync();
            return new OperationResult<OrderResponse.OrderResponseForCustomer?>
            {
                Message = EnumConstants.NotificationMessage.BUSINESSDAY_DOES_NOT_EXIST,
                StatusCode = StatusCode.NotFound,
                Payload = null,
                IsError = true
            };
        }

        try
        {
            if (result.Message ==
                EnumConstants.NotificationMessage
                    .CART_DOES_NOT_EXIST_WITH_SAME_PRODUCTITEMID_AND_FARMHUBID_STATIONID_BUSINESSDAYID
                || result.Message == EnumConstants.NotificationMessage.ORDER_DETAIL_DOES_NOT_EXIST)
            {
                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customerId,
                    FarmHubId = request.FarmHubId,
                    CreatedAt = DateTime.Now,
                    StationId = request.StationId,
                    Code = "Order" + DateTime.Now.ToString("yyyyMMddHHmmss") + customerId,
                    UpdatedAt = DateTime.Now,
                    TotalAmount = (decimal?)(productItemInMenu.SalePrice * request.Quantity),
                    TotalFarmHubPrice = (decimal?)(productItemInMenu.SalePrice * request.Quantity),
                    TotalBenefit = 0,
                    CustomerStatus = EnumConstants.ActiveInactiveEnum.ACTIVE,
                    DeliveryStatus = EnumConstants.ActiveInactiveEnum.ACTIVE,
                    ShipAddress = request.ShipAddress,
                    IsPaid = false
                };

                var orderResponse = await _unitOfWork.OrderRepository.AddAsync(order);
                var count = await _unitOfWork.SaveChangesAsync();

                if (count == 0)
                {
                    await transaction.RollbackAsync();
                    return new OperationResult<OrderResponse.OrderResponseForCustomer?>
                    {
                        Message = EnumConstants.NotificationMessage.CREATE_CART_FAILURE,
                        StatusCode = StatusCode.NotFound,
                        Payload = null,
                        IsError = true
                    };
                }

                var orderDetail = new OrderDetail()
                {
                    OrderId = orderResponse.Id,
                    ProductItemId = request.ProductItemId,
                    Quantity = request.Quantity,
                    UnitPrice = (decimal)productItemInMenu.SalePrice!,
                    TotalPrice = (decimal)(productItemInMenu.SalePrice * request.Quantity),
                    Order = orderResponse,
                };

                await _unitOfWork.OrderDetailRepository.AddAsync(orderDetail);
                var countOrderDetail = await _unitOfWork.SaveChangesAsync();
                if (countOrderDetail == 0)
                {
                    await transaction.RollbackAsync();
                    return new OperationResult<OrderResponse.OrderResponseForCustomer?>
                    {
                        Message = EnumConstants.NotificationMessage.CREATE_CART_ORDER_DETAIL_FAILURE,
                        StatusCode = StatusCode.NotFound,
                        Payload = null,
                        IsError = true
                    };
                }

                var productItem = _unitOfWork.ProductItemRepository.GetByIdAsync(request.ProductItemId).Result;
                var productItemResponse = _mapper.Map<ProductItemResponseForCustomer>(productItem);
                var farmHub = _unitOfWork.FarmHubRepository.GetByIdAsync(request.FarmHubId).Result;
                var farmHubResponse = _mapper.Map<FarmHubResponse>(farmHub);
                var stationResponse = _mapper.Map<StationResponse.StationResponseSimple>(station);
                var businessDayResponse = _mapper.Map<BusinessDayResponse>(businessDay);
                var orderDetailResponseForCustomer = _mapper.Map<OrderDetailResponseForCustomer>(orderDetail);
                orderDetailResponseForCustomer.ProductItemResponse = productItemResponse;
                await transaction.CommitAsync();
                return new OperationResult<OrderResponse.OrderResponseForCustomer?>
                {
                    Message = EnumConstants.NotificationMessage.ADD_TO_CART_SUCCESS,
                    StatusCode = StatusCode.Created,
                    Payload = new OrderResponse.OrderResponseForCustomer(
                        orderResponse.Id,
                        orderResponse.FarmHubId,
                        orderResponse.CustomerId,
                        orderResponse.StationId,
                        orderResponse.BusinessDayId,
                        orderResponse.CreatedAt,
                        orderResponse.Code,
                        orderResponse.ShipAddress,
                        orderResponse.TotalAmount,
                        orderResponse.IsPaid,
                        farmHubResponse,
                        businessDayResponse,
                        stationResponse,
                        orderDetailResponseForCustomer
                    ),
                    IsError = false
                };
            }
            else if (result.Message ==
                     EnumConstants.NotificationMessage.CART_EXIST_WITH_SAME_PRODUCTITEMID_AND_FARMHUBID)
            {
                var order = await _unitOfWork.OrderRepository.FilterByExpression(
                    predicate: x => x.CustomerId == customerId
                                    && x.FarmHubId == request.FarmHubId
                                    && x.IsPaid == false
                ).FirstOrDefaultAsync();
                if (order == null)
                {
                    return new OperationResult<OrderResponse.OrderResponseForCustomer?>
                    {
                        Message = EnumConstants.NotificationMessage
                            .CART_DOES_NOT_EXIST_WITH_SAME_PRODUCTITEMID_AND_FARMHUBID_STATIONID_BUSINESSDAYID,
                        StatusCode = StatusCode.NotFound,
                        Payload = null,
                        IsError = true
                    };
                }

                var orderDetail = await _unitOfWork.OrderDetailRepository.FilterByExpression(
                    predicate: x => x.OrderId == order.Id
                                    && x.ProductItemId == request.ProductItemId
                ).FirstOrDefaultAsync();
                if (orderDetail == null)
                {
                    return new OperationResult<OrderResponse.OrderResponseForCustomer?>
                    {
                        Message = EnumConstants.NotificationMessage.CREATE_CART_ORDER_DETAIL_FAILURE,
                        StatusCode = StatusCode.NotFound,
                        Payload = null,
                        IsError = true
                    };
                }

                if (request.IsAddToCart)
                {
                    orderDetail.Quantity += request.Quantity;
                }
                else
                {
                    orderDetail.Quantity = request.Quantity;
                }

                orderDetail.UnitPrice = (decimal)productItemInMenu.SalePrice!;
                orderDetail.TotalPrice = (decimal)(productItemInMenu.SalePrice * request.Quantity);
                await _unitOfWork.OrderDetailRepository.UpdateAsync(orderDetail);
                var countOrderDetail = await _unitOfWork.SaveChangesAsync();
                if (countOrderDetail == 0)
                {
                    await transaction.RollbackAsync();
                    return new OperationResult<OrderResponse.OrderResponseForCustomer?>
                    {
                        Message = EnumConstants.NotificationMessage.UPDATE_CART_ORDER_DETAIL_SUCCESS,
                        StatusCode = StatusCode.NotFound,
                        Payload = null,
                        IsError = true
                    };
                }

                var productItem = _unitOfWork.ProductItemRepository.GetByIdAsync(request.ProductItemId).Result;
                var productItemResponse = _mapper.Map<ProductItemResponseForCustomer>(productItem);
                var farmHub = _unitOfWork.FarmHubRepository.GetByIdAsync(request.FarmHubId).Result;
                var farmHubResponse = _mapper.Map<FarmHubResponse>(farmHub);
                var businessDayResponse = _mapper.Map<BusinessDayResponse>(businessDay);
                var orderDetailResponseForCustomer = _mapper.Map<OrderDetailResponseForCustomer>(orderDetail);
                orderDetailResponseForCustomer.ProductItemResponse = productItemResponse;
                await transaction.CommitAsync();
                return new OperationResult<OrderResponse.OrderResponseForCustomer?>
                {
                    Message = EnumConstants.NotificationMessage.CART_EXIST_WITH_SAME_PRODUCTITEMID_AND_FARMHUBID,
                    StatusCode = StatusCode.Ok,
                    Payload = new OrderResponse.OrderResponseForCustomer(
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
                        farmHubResponse,
                        businessDayResponse,
                        null,
                        orderDetailResponseForCustomer
                    ),
                };
            }
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return new OperationResult<OrderResponse.OrderResponseForCustomer?>
            {
                Message = EnumConstants.NotificationMessage.ADD_TO_CART_FAILURE + e.Message,
                StatusCode = StatusCode.ServerError,
                Payload = null,
                IsError = true
            };
        }

        return result;
    }


    public async Task<OperationResult<IEnumerable<OrderResponse.OrderResponseForCustomer?>?>> GetCart(
        Expression<Func<Order, bool>>? filter,
        string? orderBy,
        bool? isDesc,
        int pageIndex,
        int pageSize)
    {
        var result = new OperationResult<IEnumerable<OrderResponse.OrderResponseForCustomer?>>();
        try
        {
            var orders = await _unitOfWork.OrderRepository.FilterAll(
                isAscending: isDesc,
                orderBy: orderBy,
                predicate: filter,
                null,
                pageIndex: pageIndex,
                pageSize: pageSize
            ).ToListAsync();
            if (!orders.Any())
            {
                result.Message = EnumConstants.NotificationMessage.NOT_FOUND_ANY_ITEM_IN_CART;
                result.StatusCode = StatusCode.NotFound;
                result.Payload = null;
                result.IsError = true;
                return result;
            }

            var orderResponses = new List<OrderResponse.OrderResponseForCustomer?>();
            foreach (var order in orders)
            {
                var farmHub = await _unitOfWork.FarmHubRepository.FilterByExpression(x => x.Id == order.FarmHubId).FirstOrDefaultAsync();
                var farmHubResponse = _mapper.Map<FarmHubResponse>(farmHub);
                var station = await _unitOfWork.StationRepository.GetByIdAsync(order.StationId.GetValueOrDefault());
                var stationResponse = _mapper.Map<StationResponse.StationResponseSimple>(station);
                var businessDay = _unitOfWork.BusinessDayRepository
                    .GetByIdAsync(order.BusinessDayId.GetValueOrDefault()).Result;
                var businessDayResponse = _mapper.Map<BusinessDayResponse>(businessDay);
                order.OrderDetails = await _unitOfWork.OrderDetailRepository.FilterByExpression(
                    x => x.OrderId == order.Id).ToListAsync();
                foreach (var orderDetail in order.OrderDetails)
                {
                    var productItem = _unitOfWork.ProductItemRepository.GetByIdAsync(orderDetail.ProductItemId).Result;
                    var productItemResponse = _mapper.Map<ProductItemResponseForCustomer>(productItem);
                    var orderDetailResponse = _mapper.Map<OrderDetailResponseForCustomer>(orderDetail);
                    orderDetailResponse.ProductItemResponse = productItemResponse;
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
                        farmHubResponse,
                        businessDayResponse,
                        stationResponse,
                        orderDetailResponse
                    );
                    orderResponses.Add(orderResponse);
                }
            }

            result.Message = EnumConstants.NotificationMessage.GET_CART_SUCCESS;
            result.StatusCode = StatusCode.Ok;
            result.Payload = orderResponses;
            result.IsError = false;
        }
        catch (Exception e)
        {
            result.Message = e.Message;
            result.StatusCode = StatusCode.ServerError;
            result.Payload = null;
        }

        return result;
    }
}
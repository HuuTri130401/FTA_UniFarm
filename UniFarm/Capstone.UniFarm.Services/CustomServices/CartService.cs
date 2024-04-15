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
            var currentDay = DateTime.Now.Date;
            var order = await _unitOfWork.OrderRepository.FilterByExpression(
                predicate: x => x.CustomerId == customerId
                                && x.FarmHubId == farmHubId
                                && x.IsPaid == isPaid
                                && x.StationId == stationId
                                && x.BusinessDayId == businessDayId
            ).FirstOrDefaultAsync();
            if (order == null)
            {
                result.Message = EnumConstants.NotificationMessage
                    .CART_DOES_NOT_EXIST_WITH_SAME_PRODUCTITEMID_AND_FARMHUBID_STATIONID_BUSINESSDAYID;
                result.StatusCode = StatusCode.NotFound;
                result.Payload = null;
                result.IsError = false;
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
                    /*&& x.Status == EnumConstants.ActiveInactiveEnum.ACTIVE*/
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

            result.Payload = new OrderResponse.OrderResponseForCustomer()
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
                FarmHubResponse = farmHubResponse,
                BusinessDayResponse = businessDayResponse,
                StationResponse = stationResponse,
                OrderDetailResponse = new List<OrderDetailResponseForCustomer> { orderDetailResponse }
            };
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
    /// 1. Kiểm tra xem giỏ hàng đã tồn tại chưa 
    /// - Nếu chưa tồn tại CART_DOES_NOT_EXIST_WITH_SAME_PRODUCTITEMID_AND_FARMHUBID_STATIONID_BUSINESSDAYID thì tạo mới order và orderDetail
    /// - Nếu chưa tồn tại ORDER_DETAIL_DOES_NOT_EXIST thì tạo mới orderDetail và cập nhật lại order
    /// - Nếu đã tồn tại CART_EXIST_WITH_SAME_PRODUCTITEMID_AND_FARMHUBID thì cập nhật số lượng sản phẩm
    /// 
    /// </summary>
    public async Task<OperationResult<OrderResponse.OrderResponseForCustomer?>> UpsertToCart(Guid customerId,
        AddToCartRequest request)
    {
        // 1. Kiểm tra xem giỏ hàng đã tồn tại chưa
        var result = await CheckExistCart(customerId, request.FarmHubId, request.ProductItemId, request.StationId,
            request.BusinessDayId,
            false);

        var transaction = await _unitOfWork.BeginTransactionAsync();

        var productItemInMenu = _unitOfWork.ProductItemInMenuRepository
            .FilterByExpression(
                predicate: x => x.ProductItemId == request.ProductItemId
                                && x.Status == EnumConstants.ActiveInactiveEnum.ACTIVE
            ).FirstOrDefaultAsync().Result;
        if (productItemInMenu == null)
        {
            await transaction.RollbackAsync();
            return new OperationResult<OrderResponse.OrderResponseForCustomer?>
            {
                Message = EnumConstants.NotificationMessage.PRODUCT_ITEM_IN_MENU_DOES_NOT_EXIST,
                StatusCode = StatusCode.NotFound,
                Payload = null,
                IsError = false
            };
        }

        var productItemAndFarmHub = _unitOfWork.ProductItemRepository.FilterByExpression(
            predicate: x => x.Id == request.ProductItemId
                            && x.FarmHubId == request.FarmHubId
        ).FirstOrDefaultAsync().Result;

        if (productItemAndFarmHub == null)
        {
            await transaction.RollbackAsync();
            return new OperationResult<OrderResponse.OrderResponseForCustomer?>
            {
                Message = EnumConstants.NotificationMessage.PRODUCT_ITEM_AND_FARMHUBID_DOES_NOT_EXIST,
                StatusCode = StatusCode.NotFound,
                Payload = null,
                IsError = false
            };
        }

        var station = _unitOfWork.StationRepository.FilterByExpression(x =>
                x.Id == request.StationId
                && x.Status == EnumConstants.ActiveInactiveEnum.ACTIVE
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
                IsError = false
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
                IsError = false
            };
        }

        var customer = await _unitOfWork.AccountRepository.GetByIdAsync(customerId);

        try
        {
            if (result.Message ==
                EnumConstants.NotificationMessage
                    .CART_DOES_NOT_EXIST_WITH_SAME_PRODUCTITEMID_AND_FARMHUBID_STATIONID_BUSINESSDAYID)
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
                    CustomerStatus = EnumConstants.ActiveInactiveEnum.ACTIVE,
                    DeliveryStatus = EnumConstants.ActiveInactiveEnum.ACTIVE,
                    IsPaid = false,
                    ShipAddress = customer!.Address,
                    BusinessDayId = request.BusinessDayId,
                    FullName = customer!.FirstName + " " + customer!.LastName,
                    PhoneNumber = customer!.PhoneNumber
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
                        IsError = false
                    };
                }

                var orderDetail = new OrderDetail()
                {
                    OrderId = orderResponse.Id,
                    ProductItemId = request.ProductItemId,
                    Quantity = request.Quantity,
                    UnitPrice = (decimal)productItemInMenu.SalePrice!,
                    TotalPrice = (decimal)(productItemInMenu.SalePrice * request.Quantity),
                    Order = orderResponse
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
                        IsError = false
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
                    Payload = new OrderResponse.OrderResponseForCustomer()
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
                        FarmHubResponse = farmHubResponse,
                        BusinessDayResponse = businessDayResponse,
                        StationResponse = stationResponse,
                        OrderDetailResponse = new List<OrderDetailResponseForCustomer>
                            { orderDetailResponseForCustomer }
                    },
                    IsError = false
                };
            }
            else if (result.Message == EnumConstants.NotificationMessage.ORDER_DETAIL_DOES_NOT_EXIST)
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
                        IsError = false
                    };
                }

                var orderDetail = new OrderDetail()
                {
                    OrderId = order.Id,
                    ProductItemId = request.ProductItemId,
                    Quantity = request.Quantity,
                    UnitPrice = (decimal)productItemInMenu.SalePrice!,
                    TotalPrice = (decimal)(productItemInMenu.SalePrice * request.Quantity),
                    Order = null,
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
                        IsError = false
                    };
                }

                // Cập nhật lại order
                order.TotalAmount += (decimal)(productItemInMenu.SalePrice * request.Quantity);
                await _unitOfWork.OrderRepository.UpdateAsync(order);
                var countOrder = await _unitOfWork.SaveChangesAsync();
                if (countOrder == 0)
                {
                    await transaction.RollbackAsync();
                    return new OperationResult<OrderResponse.OrderResponseForCustomer?>
                    {
                        Message = EnumConstants.NotificationMessage.UPDATE_CART_FAILURE,
                        StatusCode = StatusCode.NotFound,
                        Payload = null,
                        IsError = false
                    };
                }

                var farmHub = _unitOfWork.FarmHubRepository.GetByIdAsync(request.FarmHubId).Result;
                var farmHubResponse = _mapper.Map<FarmHubResponse>(farmHub);
                var businessDayResponse = _mapper.Map<BusinessDayResponse>(businessDay);
                var orderDetails = await _unitOfWork.OrderDetailRepository.FilterByExpression(
                    x => x.OrderId == order.Id
                ).ToListAsync();

                List<OrderDetailResponseForCustomer> orderDetailResponseForCustomer = new();
                foreach (var item in orderDetails)
                {
                    var productItemInOrderDetail = _unitOfWork.ProductItemRepository.GetByIdAsync(item.ProductItemId)
                        .Result;
                    var productItemResponseInOrderDetail =
                        _mapper.Map<ProductItemResponseForCustomer>(productItemInOrderDetail);
                    var orderDetailResponse = _mapper.Map<OrderDetailResponseForCustomer>(item);
                    orderDetailResponse.ProductItemResponse = productItemResponseInOrderDetail;
                    orderDetailResponseForCustomer.Add(orderDetailResponse);
                }

                await transaction.CommitAsync();
                return new OperationResult<OrderResponse.OrderResponseForCustomer?>
                {
                    Message = EnumConstants.NotificationMessage.CREATE_CART_SUCCESS,
                    StatusCode = StatusCode.Created,
                    Payload = new OrderResponse.OrderResponseForCustomer()
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
                        FarmHubResponse = farmHubResponse,
                        BusinessDayResponse = businessDayResponse,
                        StationResponse = _mapper.Map<StationResponse.StationResponseSimple>(station),
                        OrderDetailResponse = orderDetailResponseForCustomer
                    },
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
                        IsError = false
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
                        IsError = false
                    };
                }

                if (request.IsAddToCart)
                {
                    order.TotalAmount -= orderDetail.TotalPrice ?? 0;
                    orderDetail.Quantity += request.Quantity;
                    orderDetail.TotalPrice = (decimal)(productItemInMenu.SalePrice * orderDetail.Quantity);
                    order.TotalAmount += (decimal)(productItemInMenu.SalePrice * orderDetail.Quantity);
                }
                else
                {
                    // Nếu số lượng sản phẩm = 0 thì xóa orderDetail
                    if (request.Quantity == 0)
                    {
                        orderDetail.TotalPrice = 0;
                        // cập nhật lại order
                        order.TotalAmount -= orderDetail.TotalPrice;
                    }
                    else
                    {
                        // cập nhật order
                        order.TotalAmount -= orderDetail.TotalPrice;
                        orderDetail.Quantity = request.Quantity;
                        orderDetail.TotalPrice = (decimal)(productItemInMenu.SalePrice * request.Quantity);
                        order.TotalAmount += orderDetail.TotalPrice;
                    }
                }

                orderDetail.UnitPrice = (decimal)productItemInMenu.SalePrice!;
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
                        IsError = false
                    };
                }

                await _unitOfWork.OrderRepository.UpdateAsync(order);
                var countOrder = await _unitOfWork.SaveChangesAsync();
                if (countOrder == 0)
                {
                    await transaction.RollbackAsync();
                    return new OperationResult<OrderResponse.OrderResponseForCustomer?>
                    {
                        Message = EnumConstants.NotificationMessage.UPDATE_CART_FAILURE,
                        StatusCode = StatusCode.NotFound,
                        Payload = null,
                        IsError = false
                    };
                }

                var farmHub = _unitOfWork.FarmHubRepository.GetByIdAsync(request.FarmHubId).Result;
                var farmHubResponse = _mapper.Map<FarmHubResponse>(farmHub);
                var businessDayResponse = _mapper.Map<BusinessDayResponse>(businessDay);

                var orderDetails = await _unitOfWork.OrderDetailRepository.FilterByExpression(
                    x => x.OrderId == order.Id
                ).ToListAsync();

                List<OrderDetailResponseForCustomer> orderDetailResponseForCustomer = new();
                foreach (var item in orderDetails)
                {
                    var productItemInOrderDetail = _unitOfWork.ProductItemRepository.GetByIdAsync(item.ProductItemId)
                        .Result;
                    var productItemResponseInOrderDetail =
                        _mapper.Map<ProductItemResponseForCustomer>(productItemInOrderDetail);
                    var orderDetailResponse = _mapper.Map<OrderDetailResponseForCustomer>(item);
                    orderDetailResponse.ProductItemResponse = productItemResponseInOrderDetail;
                    orderDetailResponseForCustomer.Add(orderDetailResponse);
                }

                await transaction.CommitAsync();
                return new OperationResult<OrderResponse.OrderResponseForCustomer?>
                {
                    Message = EnumConstants.NotificationMessage.CART_EXIST_WITH_SAME_PRODUCTITEMID_AND_FARMHUBID,
                    StatusCode = StatusCode.Ok,
                    Payload = new OrderResponse.OrderResponseForCustomer()
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
                        FarmHubResponse = farmHubResponse,
                        BusinessDayResponse = businessDayResponse,
                        StationResponse = _mapper.Map<StationResponse.StationResponseSimple>(station),
                        OrderDetailResponse = orderDetailResponseForCustomer
                    }
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
                IsError = false
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
        var result = new OperationResult<IEnumerable<OrderResponse.OrderResponseForCustomer?>?>();
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
                result.Payload = new List<OrderResponse.OrderResponseForCustomer>();
                result.IsError = false;
                return result;
            }

            var orderResponses = new List<OrderResponse.OrderResponseForCustomer?>();
            foreach (var order in orders)
            {
                var farmHub = await _unitOfWork.FarmHubRepository.FilterByExpression(x => x.Id == order.FarmHubId)
                    .FirstOrDefaultAsync();
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
                    var productImage = await _unitOfWork.ProductImageRepository.FilterByExpression(
                        x => x.ProductItemId == productItem!.Id && x.DisplayIndex == 1).FirstOrDefaultAsync();
                    if (productImage != null)
                    {
                        productItemResponse.ImageUrl = productImage.ImageUrl;
                    }

                    var orderDetailResponse = _mapper.Map<OrderDetailResponseForCustomer>(orderDetail);
                    orderDetailResponse.ProductItemResponse = productItemResponse;
                    var productInMenu = await _unitOfWork.ProductItemInMenuRepository.FilterByExpression(
                        x => x.ProductItemId == productItem!.Id
                             && x.Status == EnumConstants.ActiveInactiveEnum.ACTIVE).FirstOrDefaultAsync();
                    if (productInMenu == null)
                    {
                        orderDetailResponse.QuantityInStock = 0;
                    }
                    else
                    {
                        orderDetailResponse.QuantityInStock = productInMenu.Quantity - productInMenu.Sold;
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
                        FarmHubResponse = farmHubResponse,
                        BusinessDayResponse = businessDayResponse,
                        StationResponse = stationResponse,
                        OrderDetailResponse = new List<OrderDetailResponseForCustomer> { orderDetailResponse }
                    };
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


    public async Task<OperationResult<IEnumerable<OrderResponse.OrderResponseForCustomer?>?>> BeforeCheckout(
        List<CheckoutRequest> request)
    {
        var result = new OperationResult<IEnumerable<OrderResponse.OrderResponseForCustomer?>>();
        try
        {
            var orderResponses = new List<OrderResponse.OrderResponseForCustomer?>();
            foreach (var item in request)
            {
                var order = await _unitOfWork.OrderRepository.GetByIdAsync(item.OrderId);
                if (order == null)
                {
                    result.Message = EnumConstants.NotificationMessage.ORDER_DOES_NOT_EXIST;
                    result.StatusCode = StatusCode.NotFound;
                    result.Errors.Add(new Error()
                    {
                        Code = StatusCode.NotFound,
                        Message = item.OrderId + EnumConstants.NotificationMessage.ORDER_DOES_NOT_EXIST
                    });
                    result.Payload = null;
                    result.IsError = false;
                    return result;
                }

                var orderDetailList = new List<OrderDetailResponseForCustomer>();
                foreach (var detail in item.OrderDetailIds)
                {
                    var orderDetail = await _unitOfWork.OrderDetailRepository.GetByIdAsync(detail);
                    if (orderDetail == null)
                    {
                        result.Message = EnumConstants.NotificationMessage.ORDER_DETAIL_DOES_NOT_EXIST;
                        result.StatusCode = StatusCode.NotFound;
                        result.Errors.Add(new Error()
                        {
                            Code = StatusCode.NotFound,
                            Message = detail + EnumConstants.NotificationMessage.ORDER_DETAIL_DOES_NOT_EXIST
                        });
                        result.Payload = null;
                        result.IsError = false;
                        return result;
                    }

                    var productItem = await _unitOfWork.ProductItemRepository.GetByIdAsync(orderDetail.ProductItemId);
                    var productItemResponse = _mapper.Map<ProductItemResponseForCustomer>(productItem);
                    var productImage = await _unitOfWork.ProductImageRepository.FilterByExpression(
                        x => x.ProductItemId == productItem!.Id && x.DisplayIndex == 1).FirstOrDefaultAsync();
                    if (productImage != null)
                    {
                        productItemResponse.ImageUrl = productImage.ImageUrl;
                    }

                    var orderDetailResponse = _mapper.Map<OrderDetailResponseForCustomer>(orderDetail);
                    orderDetailResponse.ProductItemResponse = productItemResponse;
                    var productInMenu = await _unitOfWork.ProductItemInMenuRepository.FilterByExpression(
                        x => x.ProductItemId == productItem!.Id
                             && x.Status == EnumConstants.ActiveInactiveEnum.ACTIVE).FirstOrDefaultAsync();
                    if (productInMenu == null)
                    {
                        orderDetailResponse.QuantityInStock = 0;
                    }
                    else
                    {
                        orderDetailResponse.QuantityInStock = productInMenu.Quantity - productInMenu.Sold;
                    }

                    orderDetailList.Add(orderDetailResponse);
                }

                var farmHub = await _unitOfWork.FarmHubRepository.GetByIdAsync(order.FarmHubId);
                var farmHubResponse = _mapper.Map<FarmHubResponse>(farmHub);
                var station = await _unitOfWork.StationRepository.GetByIdAsync(order.StationId.GetValueOrDefault());
                var stationResponse = _mapper.Map<StationResponse.StationResponseSimple>(station);
                var businessDay =
                    await _unitOfWork.BusinessDayRepository.GetByIdAsync(order.BusinessDayId.GetValueOrDefault());
                var businessDayResponse = _mapper.Map<BusinessDayResponse>(businessDay);
                order.OrderDetails = await _unitOfWork.OrderDetailRepository.FilterByExpression(
                    x => x.OrderId == order.Id).ToListAsync();
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
                    TotalAmount = orderDetailList.Sum(x => x.TotalPrice),
                    IsPaid = order.IsPaid,
                    FullName = order.FullName,
                    PhoneNumber = order.PhoneNumber,
                    FarmHubResponse = farmHubResponse,
                    BusinessDayResponse = businessDayResponse,
                    StationResponse = stationResponse,
                    OrderDetailResponse = orderDetailList
                };
                orderResponses.Add(orderResponse);
            }

            result.Message = "Get order and order detail success";
            result.StatusCode = StatusCode.Ok;
            result.Payload = orderResponses;
            result.IsError = false;
        }
        catch (Exception e)
        {
            result.Message = e.Message;
            result.StatusCode = StatusCode.ServerError;
            result.Errors.Add(new Error()
            {
                Code = StatusCode.ServerError,
                Message = e.Message
            });
            result.Payload = null;
            throw;
        }

        return result;
    }

    public async Task<OperationResult<OrderResponse.OrderResponseForCustomer?>?> UpdateQuantity(Guid payloadId,
        UpdateQuantityRequest request)
    {
        var result = new OperationResult<OrderResponse.OrderResponseForCustomer?>();
        try
        {
            var orderDetail = await _unitOfWork.OrderDetailRepository.GetByIdAsync(request.OrderDetailId);
            if (orderDetail == null)
            {
                result.Message = EnumConstants.NotificationMessage.ORDER_DETAIL_DOES_NOT_EXIST;
                result.StatusCode = StatusCode.NotFound;
                result.Errors.Add(new Error()
                {
                    Code = StatusCode.NotFound,
                    Message = payloadId + EnumConstants.NotificationMessage.ORDER_DETAIL_DOES_NOT_EXIST
                });
                result.Payload = null;
                result.IsError = false;
                return result;
            }
            
            var order = await _unitOfWork.OrderRepository.FilterByExpression(x => x.Id == orderDetail.OrderId 
                    && x.IsPaid == false
                    && x.CustomerId == payloadId)
                .FirstOrDefaultAsync();
            if (order == null)
            {
                result.Message = EnumConstants.NotificationMessage.ORDER_DOES_NOT_EXIST;
                result.StatusCode = StatusCode.NotFound;
                result.Errors.Add(new Error()
                {
                    Code = StatusCode.NotFound,
                    Message = EnumConstants.NotificationMessage.ORDER_DOES_NOT_EXIST
                });
                result.Payload = null;
                result.IsError = false;
                return result;
            }

            var productItem = await _unitOfWork.ProductItemRepository.GetByIdAsync(orderDetail.ProductItemId);
            var productItemResponse = _mapper.Map<ProductItemResponseForCustomer>(productItem);
            var productImage = await _unitOfWork.ProductImageRepository.FilterByExpression(
                x => x.ProductItemId == productItem!.Id && x.DisplayIndex == 1).FirstOrDefaultAsync();
            if (productImage != null)
            {
                productItemResponse.ImageUrl = productImage.ImageUrl;
            }

            var productInMenu = await _unitOfWork.ProductItemInMenuRepository.FilterByExpression(
                x => x.ProductItemId == productItem!.Id
                     && x.Status == EnumConstants.ActiveInactiveEnum.ACTIVE).FirstOrDefaultAsync();
            if (productInMenu == null)
            {
                result.Message = EnumConstants.NotificationMessage.PRODUCT_ITEM_IN_MENU_DOES_NOT_EXIST;
                result.StatusCode = StatusCode.NotFound;
                result.Errors.Add(new Error()
                {
                    Code = StatusCode.NotFound,
                    Message = EnumConstants.NotificationMessage.PRODUCT_ITEM_IN_MENU_DOES_NOT_EXIST
                });
                result.Payload = null;
                result.IsError = false;
                return result;
            }

            var farmHub = await _unitOfWork.FarmHubRepository.GetByIdAsync(order.FarmHubId);
            var farmHubResponse = _mapper.Map<FarmHubResponse>(farmHub);
            var station = await _unitOfWork.StationRepository.GetByIdAsync(order.StationId.GetValueOrDefault());
            var stationResponse = _mapper.Map<StationResponse.StationResponseSimple>(station);
            var businessDay = await _unitOfWork.BusinessDayRepository.GetByIdAsync(order.BusinessDayId.GetValueOrDefault());
            var businessDayResponse = _mapper.Map<BusinessDayResponse>(businessDay);

            if (request.Quantity > productInMenu.Quantity - productInMenu.Sold)
            {
                result.Errors.Add(new Error()
                {
                    Code = StatusCode.BadRequest,
                    Message = "Số lượng sản phẩm trong kho không đủ"
                });
                result.Payload = null;
                result.IsError = false;
                return result;
            }

            if (request.Quantity <= 0)
            {
                order.TotalAmount -= orderDetail.TotalPrice;
            }
            else
            {
                order.TotalAmount -= orderDetail.TotalPrice;
                orderDetail.Quantity = request.Quantity;
                orderDetail.TotalPrice = orderDetail.UnitPrice * (decimal)request.Quantity;
                order.TotalAmount += orderDetail.TotalPrice;
            }

            if (request.Quantity <= 0)
            {
                await _unitOfWork.OrderDetailRepository.DeleteAsync(orderDetail);
            } else {
                await _unitOfWork.OrderDetailRepository.UpdateAsync(orderDetail);
            }
            
            var countOrderDetail = await _unitOfWork.SaveChangesAsync();
            if (countOrderDetail == 0)
            {
                result.Message = "Update order detail failure";
                result.StatusCode = StatusCode.NotFound;
                result.Errors.Add(new Error()
                {
                    Code = StatusCode.NotFound,
                    Message = "Update order detail failure"
                });
                result.Payload = null;
                result.IsError = true;
                return result;
            }

            var isDeleted = false;
            if(order.TotalAmount <= 0)
            {
                await _unitOfWork.OrderRepository.DeleteAsync(order);
                isDeleted = true;
            } else {
                await _unitOfWork.OrderRepository.UpdateAsync(order);
            }
            
            var countOrder = await _unitOfWork.SaveChangesAsync();
            
            if(isDeleted)
            {
                result.Message = "Delete order success";
                result.StatusCode = StatusCode.Ok;
                result.Payload = null;
                result.IsError = false;
                return result;
            }
            
            if (countOrder == 0)
            {
                result.Message = EnumConstants.NotificationMessage.UPDATE_CART_FAILURE;
                result.StatusCode = StatusCode.BadRequest;
                result.Errors.Add(new Error()
                {
                    Code = StatusCode.BadRequest,
                    Message = EnumConstants.NotificationMessage.UPDATE_CART_FAILURE
                });
                result.Payload = null;
                result.IsError = false;
                return result;
            }

            var orderDetailResponse = _mapper.Map<OrderDetailResponseForCustomer>(orderDetail);
            orderDetailResponse.ProductItemResponse = productItemResponse;
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
                FarmHubResponse = farmHubResponse,
                BusinessDayResponse = businessDayResponse,
                StationResponse = stationResponse,
                OrderDetailResponse = new List<OrderDetailResponseForCustomer> { orderDetailResponse }
            };

            result.Message = EnumConstants.NotificationMessage.UPDATE_CART_SUCCESS;
            result.StatusCode = StatusCode.Ok;
            result.Payload = orderResponse;
            result.IsError = false;
        }
        catch (Exception e)
        {
            result.Message = e.Message;
            result.StatusCode = StatusCode.ServerError;
            result.Errors.Add(new Error()
            {
                Code = StatusCode.ServerError,
                Message = e.Message
            });
            result.Payload = null;
            result.IsError = false;
            throw;
        }

        return result;
    }
}
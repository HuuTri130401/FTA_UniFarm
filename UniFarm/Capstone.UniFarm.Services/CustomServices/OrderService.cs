using AutoMapper;
using Capstone.UniFarm.Repositories.UnitOfWork;
using Capstone.UniFarm.Services.Commons;
using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelResponses;
using Microsoft.EntityFrameworkCore;

namespace Capstone.UniFarm.Services.CustomServices;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<OperationResult<OrderResponse.OrderResponseForCustomer?>> CheckExistCart(
        Guid customerId,
        Guid farmHubId,
        Guid productItemId,
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
                                && x.CreatedAt.Date == DateTime.Now.Date
            ).FirstOrDefaultAsync();
            if (order == null)
            {
                result.Message = "Cart does not exist";
                result.StatusCode = StatusCode.NotFound;
                result.Payload = null;
                result.IsError = true;
                return result;
            }

            var orderDetail = _unitOfWork.OrderDetailRepository
                .FilterByExpression(
                    predicate: x => x.OrderId == order.Id
                                    && x.ProductItemId == productItemId
                ).FirstOrDefaultAsync().Result;
            if (orderDetail == null)
            {
                result.Message = "OrderDetail does not exist";
                result.StatusCode = StatusCode.NotFound;
                result.Payload = null;
                return result;
            }

            var productItemInMenu = _unitOfWork.ProductItemInMenuRepository
                .FilterByExpression(
                    predicate: x => x.Id == productItemId && x.Status == "Active"
                ).FirstOrDefaultAsync().Result;

            if (productItemInMenu == null)
            {
                result.Message = "ProductItem does not exist in menu ";
                result.StatusCode = StatusCode.NotFound;
                result.Payload = null;
                return result;
            }

            var productItem = _unitOfWork.ProductItemRepository
                .FilterByExpression(
                    predicate: x => x.Id == productItemId && x.Status == "Active"
                ).FirstOrDefaultAsync().Result;

            if (productItem == null)
            {
                result.Message = "ProductItem does not exist or is not active";
                result.StatusCode = StatusCode.NotFound;
                result.Payload = null;
                return result;
            }

            var farmHub = _unitOfWork.FarmHubRepository
                .FilterByExpression(
                    predicate: x => x.Id == farmHubId && x.Status == "Active"
                ).FirstOrDefaultAsync().Result;

            if (farmHub == null)
            {
                result.Message = "FarmHub does not exist";
                result.StatusCode = StatusCode.NotFound;
                result.Payload = null;
                return result;
            }

            var productItemResponse = _mapper.Map<ProductItemResponseForCustomer>(productItem);
            var farmHubResponse = _mapper.Map<FarmHubResponse>(farmHub);
            var orderDetailResponse = _mapper.Map<OrderDetailResponseForCustomer>(orderDetail);
            orderDetailResponse.ProductItemResponseForCustomer = productItemResponse;

            result.Payload = new OrderResponse.OrderResponseForCustomer
            {
                OrderDetailResponseForCustomer = orderDetailResponse,
                FarmHubResponse = farmHubResponse,
            };
            result.Message = "Cart exists";
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
}
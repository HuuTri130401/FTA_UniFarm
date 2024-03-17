﻿using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Services.Commons;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Capstone.UniFarm.Services.ViewModels.ModelResponses;

namespace Capstone.UniFarm.Services.ICustomServices;

public interface IOrderService
{
    // Kiểm tra tồn tại giỏ hàng được tạo ra cùng ngày với cùng 1 farmHubId và isPaid = false hay chưa 
    Task<OperationResult<OrderResponse.OrderResponseForCustomer?>> CheckExistCart(Guid customerId, Guid farmHubId, Guid productItemId, Guid stationId, Guid businessDayId, bool isPaid = false);

    Task<OperationResult<OrderResponse.OrderResponseForCustomer?>> UpsertToCart(Guid customerId, AddToCartRequest request);
}
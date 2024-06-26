﻿using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Capstone.UniFarm.API.Controllers;

public class CartController : BaseController
{
    private readonly ICartService _cartService;
    private readonly IAccountService _accountService;
    private readonly IOrderService _orderService;

    public CartController(ICartService cartService, IAccountService accountService, IOrderService orderService)
    {
        _cartService = cartService;
        _accountService = accountService;
        _orderService = orderService;
    }

    [HttpPost("carts/check-exist-cart")]
    [SwaggerOperation(Summary =
        "Kiểm tra đã tồn tại giỏ hàng được tạo ra cùng ngày với cùng 1 farmHubId và isPaid = false hay chưa ")]
    public async Task<IActionResult> CheckExistCart([FromBody] AddToCartRequest.CheckExistCartRequest request)
    {
        var response = await _cartService.CheckExistCart(request.CustomerId, request.FarmHubId, request.ProductItemId,
            request.StationId, request.BusinessDayId, request.IsPaid);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }
    
    [HttpPost("cart/upsert-to-cart")]
    [SwaggerOperation(Summary = "Thêm sản phẩm vào giỏ hoặc cập nhật sản phẩm trong giỏ hàng")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> UpsertToCart([FromBody] AddToCartRequest request)
    {
        string authHeader = HttpContext.Request.Headers["Authorization"];
        if (string.IsNullOrEmpty(authHeader))
        {
            return Unauthorized();
        }
        string token = authHeader.Replace("Bearer ", "");
        var defineUser = _accountService.GetIdAndRoleFromToken(token);
        if (defineUser.Payload == null) return HandleErrorResponse(defineUser!.Errors);
        
        var response = await _cartService.UpsertToCart(defineUser.Payload.Id, request);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }


    [HttpGet("carts")]
    [SwaggerOperation(Summary = "Lấy thông tin giỏ hàng của khách hàng")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetCart(
        [FromQuery] string? searchWord,
        [FromQuery] string? orderBy,
        [FromQuery] bool? isDesc,
        [FromQuery] int pageIndex = 0,
        [FromQuery] int pageSize = 10
    )
    {
        string authHeader = HttpContext.Request.Headers["Authorization"];
        if (string.IsNullOrEmpty(authHeader))
        {
            return Unauthorized();
        }
        string token = authHeader.Replace("Bearer ", "");
        var defineUser = _accountService.GetIdAndRoleFromToken(token);
        if (defineUser.Payload == null) return HandleErrorResponse(defineUser!.Errors);

        var response = await _cartService.GetCart(filter: x =>
                x.CustomerId == defineUser.Payload.Id
                && x.IsPaid == false,
            orderBy: orderBy,
            isDesc: isDesc,
            pageIndex: pageIndex,
            pageSize: pageSize);

        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }
    
    
    [HttpPost("cart/Checkout")]
    [Authorize]
    [SwaggerOperation(Summary = "Create order - Tien")]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest orderRequestCreate)
    {
        string authHeader = HttpContext.Request.Headers["Authorization"];
        if (string.IsNullOrEmpty(authHeader))
        {
            return Unauthorized();
        }

        string token = authHeader.Replace("Bearer ", "");
        var defineUser = _accountService.GetIdAndRoleFromToken(token);
        if (defineUser.Payload == null) return HandleErrorResponse(defineUser!.Errors);
        var result = await _orderService.Checkout(defineUser.Payload.Id, orderRequestCreate);
        return result.IsError ? HandleErrorResponse(result.Errors) : Ok(result.Payload);
    }


    
    [HttpPost("carts/before-checkout")]
    [SwaggerOperation(Summary = "Thanh toán giỏ hàng")]
    [Authorize]
    public async Task<IActionResult> BeforeCheckOut([FromBody] List<CheckoutRequest> request)
    {
        string authHeader = HttpContext.Request.Headers["Authorization"];
        if (string.IsNullOrEmpty(authHeader))
        {
            return Unauthorized();
        }
        string token = authHeader.Replace("Bearer ", "");
        var defineUser = _accountService.GetIdAndRoleFromToken(token);
        if (defineUser.Payload == null) return HandleErrorResponse(defineUser!.Errors);

        var response = await _cartService.BeforeCheckout(request);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }
    
    // Update quantity of OrderDetailId
    [HttpPut("cart/update-quantity")]
    [SwaggerOperation(Summary = "Cập nhật số lượng sản phẩm trong giỏ hàng")]
    [Authorize]
    public async Task<IActionResult> UpdateQuantity([FromBody] UpdateQuantityRequest request)
    {
        string authHeader = HttpContext.Request.Headers["Authorization"];
        if (string.IsNullOrEmpty(authHeader))
        {
            return Unauthorized();
        }
        string token = authHeader.Replace("Bearer ", "");
        var defineUser = _accountService.GetIdAndRoleFromToken(token);
        if (defineUser.Payload == null) return HandleErrorResponse(defineUser!.Errors);

        var response = await _cartService.UpdateQuantity(defineUser.Payload.Id, request);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }
}
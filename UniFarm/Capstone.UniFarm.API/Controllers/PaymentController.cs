using System.Globalization;
using Capstone.UniFarm.Domain.Enum;
using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Capstone.UniFarm.API.Controllers;


public class PaymentController : BaseController
{
    private readonly IVnPayService _vnPayService;
    private readonly IAccountService _accountService;
    private readonly IPaymentService _paymentService;

    public PaymentController(IVnPayService vnPayService, IAccountService accountService, IPaymentService paymentService)
    {
        _vnPayService = vnPayService;
        _accountService = accountService;
        _paymentService = paymentService;
    }

    [HttpPost("payment/create-payment-url")]
    public IActionResult CreatePaymentUrl([FromBody] VnPaymentRequestModel model)
    {
        var url = _vnPayService.CreatePaymentUrl(HttpContext, model);
        return Ok(url);
    }

    [HttpGet("payment/payment-callback")] // Unique route for PaymentCallBack
    public IActionResult PaymentCallBack()
    {
        var response = _vnPayService.PaymentExecute(Request.Query);

        if (response == null || response.VnPayResponseCode != "00")
        {
            return RedirectToAction(nameof(PaymentFail));
        }

        // save payment
        var responsePayment = _vnPayService.DepositPayment(response).Result;

        if (responsePayment.IsError)
        {
            return RedirectToAction(nameof(PaymentFail));
        }

        return RedirectToAction(nameof(PaymentSuccess));
    }

    [HttpGet("payment/fail")]
    public IActionResult PaymentFail()
    {
        return BadRequest("Tạo giao dịch thất bại!");
    }

    [HttpGet("payment/success")]
    public IActionResult PaymentSuccess()
    {
        return Ok("Tạo giao dịch thành công!");
    }

    [HttpPost("payment/deposit-test")]
    [Authorize]
    [SwaggerOperation(Summary = "Deposit payment testing - Tien")]
    public async Task<IActionResult> CreatePayment([FromBody] PaymentRequestCreateModel model)
    {
        string authHeader = HttpContext.Request.Headers["Authorization"];
        if (string.IsNullOrEmpty(authHeader))
        {
            return Unauthorized();
        }

        string token = authHeader.Replace("Bearer ", "");
        var defineUser = _accountService.GetIdAndRoleFromToken(token);
        if (defineUser.Payload == null) return HandleErrorResponse(defineUser!.Errors);
        var response =await _paymentService.DepositMoneyTesting(defineUser.Payload.Id, model);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }

    [HttpGet("payments")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "Get all payment - Admin - Tien")]
    public async Task<IActionResult> GetPayment(
        [FromQuery] string? from,
        [FromQuery] string? to,
        [FromQuery] string? type,
        [FromQuery] bool? isAscending = false,
        [FromQuery] string? orderBy = "PaymentDay",
        [FromQuery] int pageIndex = 0,
        [FromQuery] int pageSize = 10
        )
    {
        var response = await _paymentService.GetPayment(
            isAscending,
            orderBy,
            x => (string.IsNullOrEmpty(from) || x.From!.Contains(from)) &&
                 (string.IsNullOrEmpty(to) || x.To!.Contains(to)) &&
                 (string.IsNullOrEmpty(type) || x.Type.Contains(type)),
            pageIndex,
            pageSize);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }
    
    
    [HttpPost("payment/create-withdraw-request")]
    [Authorize(Roles = "FarmHub, Customer")]
    [SwaggerOperation(Summary = "Tạo yêu cầu rút tiền - FarmHub, Customer - Tien")]
    public async Task<IActionResult> CreateWithdrawRequest([FromBody] PaymentWithdrawRequest request)
    {
        string authHeader = HttpContext.Request.Headers["Authorization"];
        if (string.IsNullOrEmpty(authHeader))
        {
            return Unauthorized();
        }

        string token = authHeader.Replace("Bearer ", "");
        var defineUser = _accountService.GetIdAndRoleFromToken(token);
        if (defineUser.Payload == null) return HandleErrorResponse(defineUser!.Errors);

        var response = await _paymentService.CreatePaymentWithdrawRequest(defineUser.Payload.Id, request);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }
    
    [HttpPut("payment/update-withdraw-request")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "Cập nhật trạng thái yêu cầu rút tiền - Admin - Tien")]
    public async Task<IActionResult> UpdateWithdrawRequest([FromBody] PaymentUpdateStatus request)
    {
        if(ModelState.IsValid == false) return BadRequest(ModelState);
        var response = await _paymentService.UpdateWithdrawRequest(request);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }
    
    // get all payments of FarmHub and Customer 
    [HttpGet("payments/user")]
    [Authorize(Roles = "FarmHub, Customer")]
    [SwaggerOperation(Summary = "Get all payment - FarmHub, Customer - Tien")]
    public async Task<IActionResult> GetAllPayment(
        [FromQuery] EnumConstants.PaymentEnum? status,
        [FromQuery] bool? isAscending = false,
        [FromQuery] string? orderBy = "PaymentDay",
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

        var response = await _paymentService.GetPaymentForUser(
            isAscending,
            orderBy,
            x => string.IsNullOrEmpty(status.ToString()) || x.Status!.Contains(status.ToString()!),
            x => x.Id == defineUser.Payload.Id,
            pageIndex,
            pageSize);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }
}
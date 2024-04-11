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

    public PaymentController(IVnPayService vnPayService, IAccountService accountService)
    {
        _vnPayService = vnPayService;
        _accountService = accountService;
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
        var responsePayment = _vnPayService.SavePayment(response).Result;

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

    [HttpPost("payment/create-payment")]
    [Authorize]
    [SwaggerOperation(Summary = "Create payment for fake data - Tien")]
    public IActionResult CreatePayment([FromBody] PaymentRequestCreateModel model)
    {
        string authHeader = HttpContext.Request.Headers["Authorization"];
        if (string.IsNullOrEmpty(authHeader))
        {
            return Unauthorized();
        }

        string token = authHeader.Replace("Bearer ", "");
        var defineUser = _accountService.GetIdAndRoleFromToken(token);
        if (defineUser.Payload == null) return HandleErrorResponse(defineUser!.Errors);


        var response = _vnPayService.CreatePayment(defineUser.Payload.Id, model).Result;
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
        var response = await _vnPayService.GetPayment(
            isAscending,
            orderBy,
            x => (string.IsNullOrEmpty(from) || x.From!.Contains(from)) &&
                 (string.IsNullOrEmpty(to) || x.To!.Contains(to)) &&
                 (string.IsNullOrEmpty(type) || x.Type.Contains(type)),
            pageIndex,
            pageSize);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }
    
}
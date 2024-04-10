
using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Capstone.UniFarm.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentController : BaseController
{
    private readonly IVnPayService _vnPayService;
    private readonly IAccountService _accountService;

    public PaymentController(IVnPayService vnPayService, IAccountService accountService)
    {
        _vnPayService = vnPayService;
        _accountService = accountService;
    }

    [HttpPost]
    public IActionResult CreatePaymentUrl([FromBody] VnPaymentRequestModel model)
    {
        var url = _vnPayService.CreatePaymentUrl(HttpContext, model);
        return Ok(url);
    }

    [HttpGet("PaymentCallBack")] // Unique route for PaymentCallBack
    public  IActionResult PaymentCallBack()
    {
        var response = _vnPayService.PaymentExecute(Request.Query);

        if (response == null || response.VnPayResponseCode != "00")
        {
            return RedirectToAction(nameof(PaymentFail));
        }

        // save payment
        var responsePayment =  _vnPayService.SavePayment(response).Result;
        
        if(responsePayment.IsError)
        {
            return RedirectToAction(nameof(PaymentFail));
        }
      
        return RedirectToAction(nameof(PaymentSuccess));
    }

    [HttpGet("fail")]
    public IActionResult PaymentFail()
    {
        return BadRequest("Tạo giao dịch thất bại!");
    }

    [HttpGet("success")]
    public IActionResult PaymentSuccess()
    {
        return Ok("Tạo giao dịch thành công!");
    }
    
    [HttpPost("create-payment")]
    [Authorize]
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
}
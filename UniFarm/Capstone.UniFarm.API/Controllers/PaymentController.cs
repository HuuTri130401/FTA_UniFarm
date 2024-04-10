using System.Security.Cryptography;
using System.Text;
using Capstone.UniFarm.API.Configurations;
using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Capstone.UniFarm.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentController : BaseController
{
    private readonly IVnPayService _vnPayService;

    public PaymentController(IVnPayService vnPayService)
    {
        _vnPayService = vnPayService;
    }

    [HttpPost]
    public IActionResult CreatePaymentUrl([FromBody] VnPaymentRequestModel model)
    {
        var url = _vnPayService.CreatePaymentUrl(HttpContext, model);
        return Ok(url);
    }

    [HttpGet("PaymentCallBack")] // Unique route for PaymentCallBack
    public IActionResult PaymentCallBack()
    {
        var response = _vnPayService.PaymentExecute(Request.Query);

        if (response == null || response.VnPayResponseCode != "00")
        {
            return RedirectToAction(nameof(PaymentFail));
        }

        // Lưu đơn hàng vô database
        return RedirectToAction(nameof(PaymentSuccess));
    }

    [HttpGet("fail")] // Unique route for PaymentFail
    public IActionResult PaymentFail()
    {
        return BadRequest("Tao thanh toan khong thanh cong");
    }

    [HttpGet("success")] // Unique route for PaymentSuccess
    public IActionResult PaymentSuccess()
    {
        return Ok("Tao thanh toan thanh cong");
    }
}
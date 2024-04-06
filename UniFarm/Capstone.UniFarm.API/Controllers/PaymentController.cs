using System.Security.Cryptography;
using System.Text;
using Capstone.UniFarm.API.Configurations;
using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Capstone.UniFarm.API.Controllers;

public class PaymentController : BaseController
{
    private readonly IPaymentService _paymentService;
    private readonly IAccountService _accountService;
    private readonly VNPayConfig _vnPayConfig;

    public PaymentController(IPaymentService paymentService, IAccountService accountService, VNPayConfig vnPayConfig)
    {
        _paymentService = paymentService;
        _accountService = accountService;
        _vnPayConfig = vnPayConfig;
    }

    [HttpPost("/payment")]
    public IActionResult ProcessPayment()
    {
        if (!_vnPayConfig.IsConfigured())
        {
            return StatusCode(500, "VNPay is not properly configured.");
        }

        string paymentUrl = GetPaymentUrl();
        return Redirect(paymentUrl);
    }

    private string GetPaymentUrl()
    {
        var vnp_Amount = 1000000;
        var vnp_BankCode = _vnPayConfig.vnp_BankCode;
        var vnp_BankTranNo = _vnPayConfig.vnp_TxnRef;
        var vnp_CardType = _vnPayConfig.vnp_CardType;
        var vnp_PayDate = _vnPayConfig.vnp_CreateDate;
        var vnp_OrderInfo = _vnPayConfig.vnp_OrderInfo;
        var vnp_ResponseCode = "00";
        var vnp_TmnCode = _vnPayConfig.vnp_TmnCode;
        var vnp_TransactionNo = _vnPayConfig.vnp_TxnRef;
        var vnp_TransactionStatus = 00;
        var vnp_TxnRef = _vnPayConfig.vnp_TxnRef;
        var version = _vnPayConfig.vnp_Version;
        var command = _vnPayConfig.vnp_Command;
        var returnUrl = _vnPayConfig.vnp_Returnurl;

        var vnp_Params = new Dictionary<string, string>
        {
            { "vnp_Version", version },
            { "vnp_Command", command },
            { "vnp_TmnCode", vnp_TmnCode },
            { "vnp_BankCode", vnp_BankCode },
            { "vnp_BankTranNo", vnp_BankTranNo },
            { "vnp_CardType", vnp_CardType },
            { "vnp_PayDate", vnp_PayDate },
            { "vnp_OrderInfo", vnp_OrderInfo },
            { "vnp_TransactionNo", vnp_TransactionNo.ToString() },
            { "vnp_ResponseCode", vnp_ResponseCode },
            { "vnp_TransactionStatus", vnp_TransactionStatus.ToString() },
            { "vnp_TxnRef", vnp_TxnRef.ToString() },
            { "vnp_SecureHashType", "SHA256" },
            { "vnp_Returnurl", returnUrl }
        };
        
        var vnp_SecureHash = _vnPayConfig.vnp_HashSecret;
        var vnp_Url = _vnPayConfig.vnp_Url;
        
        var query = vnp_Params
            .OrderBy(x => x.Key)
            .Aggregate("", (current, next) => current + $"{next.Key}={next.Value}&");
        
        query = query.Substring(0, query.Length - 1);
        query += $"&vnp_Amount={vnp_Amount}";
        query += $"&vnp_SecureHash={vnp_SecureHash}";
        var paymentUrl = $"{vnp_Url}?{query}";
        return paymentUrl;
    }

    [HttpPost]
    [Route("/api/v1/vnpay/return")]
    public IActionResult VNPayReturn([FromQuery] string vnp_ResponseCode, [FromQuery] string vnp_TransactionNo)
    {
        Console.WriteLine($"VNPay Response Code: {vnp_ResponseCode}");
        Console.WriteLine($"VNPay Transaction No: {vnp_TransactionNo}");

        // You can return any response here, such as an OkResult or custom response.
        return Ok();
    }

    /*[HttpPost("payment")]
    public async Task<IActionResult> CreatePayment(PaymentRequestCreate paymentRequest)
    {
        if (ModelState.IsValid)
        {
            var response = await _paymentService.CreatePayment(paymentRequest);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        return BadRequest("Model is invalid");
    }


    [HttpGet("payments")]
    [SwaggerOperation(Summary = "Get payments by accountId - Customer - Done {Tien}")]
    public async Task<IActionResult> GetPayments(
        [FromQuery] string? status,
        [FromQuery] bool? isAscending,
        [FromQuery] string? orderBy = null,
        [FromQuery] string[]? includeProperties = null,
        [FromQuery] int pageIndex = 0,
        [FromQuery] int pageSize = 10)
    {
        string authHeader = HttpContext.Request.Headers["Authorization"];
        if (string.IsNullOrEmpty(authHeader))
        {
            return Unauthorized();
        }

        // The token is prefixed with "Bearer ", so we need to remove that prefix
        string token = authHeader.Replace("Bearer ", "");

        var defineUser = _accountService.GetIdAndRoleFromToken(token);
        if (defineUser.Payload == null) return HandleErrorResponse(defineUser!.Errors);

        var response = await _paymentService.GetAllForCustomer(
            isAscending,
            orderBy,
            filterWallet: null,
            filterAccount: x => x.Id == defineUser.Payload!.Id,
            filterPayment: x => string.IsNullOrEmpty(status) || x.Status == status,
            includeProperties,
            pageIndex, pageSize);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }


    [HttpGet("admin/payments")]
    [SwaggerOperation(Summary = "Get payments by accountId - Admin - Done {Tien}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllPayments(
        [FromQuery] Guid? accountId,
        [FromQuery] string? status,
        [FromQuery] bool? isAscending,
        [FromQuery] string? orderBy = null,
        [FromQuery] string[]? includeProperties = null,
        [FromQuery] int pageIndex = 0,
        [FromQuery] int pageSize = 10)
    {
        var response = await _paymentService.GetAll(
            isAscending,
            orderBy,
            filterWallet: x => Guid.Empty == accountId && x.AccountId == accountId,
            filterAccount: null,
            filterPayment: x => string.IsNullOrEmpty(status) || x.Status == status,
            includeProperties,
            pageIndex, pageSize);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }*/
}
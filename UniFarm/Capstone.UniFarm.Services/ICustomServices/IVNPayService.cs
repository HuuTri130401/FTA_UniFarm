using System.Linq.Expressions;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Services.Commons;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Capstone.UniFarm.Services.ViewModels.ModelResponses;
using Microsoft.AspNetCore.Http;

namespace Capstone.UniFarm.Services.ICustomServices
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model);
        VnPaymentResponseModel PaymentExecute(IQueryCollection collections);
        
        Task<OperationResult<Payment>> SavePayment(VnPaymentResponseModel response);
        
        Task<OperationResult<Payment>> CreatePayment(Guid? accountId, PaymentRequestCreateModel requestModel);
    }
}
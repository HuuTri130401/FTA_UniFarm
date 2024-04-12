using System.Linq.Expressions;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Services.Commons;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Capstone.UniFarm.Services.ViewModels.ModelResponses;

namespace Capstone.UniFarm.Services.ICustomServices;

public interface IPaymentService
{
    Task<OperationResult<Payment>> DepositMoneyTesting(Guid? accountId, PaymentRequestCreateModel requestModel);
    Task<OperationResult<PaymentResponse>> CreatePaymentWithdrawRequest(Guid? accountId, PaymentWithdrawRequest request);
    
    Task<OperationResult<IEnumerable<PaymentResponse>>> GetPayment(bool? isAscending, string? orderBy, Expression<Func<Payment, bool>>? filter, int pageIndex, int pageSize);

    Task<OperationResult<IEnumerable<PaymentResponse>>> GetPaymentForUser(bool? isAscending, string? orderBy,
        Expression<Func<Payment, bool>>? filter, Expression<Func<Account, bool>> filterAccount, int pageIndex,
        int pageSize);
    
    Task<OperationResult<PaymentResponse>> UpdateWithdrawRequest(PaymentUpdateStatus request);
}
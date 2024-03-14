using System.Linq.Expressions;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Services.Commons;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;

namespace Capstone.UniFarm.Services.ICustomServices;

public interface IPaymentService
{
    Task<OperationResult<IEnumerable<Payment>>> GetAll(
        bool? isAscending, 
        string? orderBy = null, 
        Expression<Func<Wallet, bool>>? filterWallet= null, 
        Expression<Func<Account, bool>>? filterAccount = null, 
        Expression<Func<Payment, bool>>? filterPayment = null, 
        string[]? includeProperties = null, 
        int pageIndex = 0, 
        int pageSize = 10);
    
    Task<OperationResult<IEnumerable<Payment>>> GetAllForCustomer(
        bool? isAscending, 
        string? orderBy = null, 
        Expression<Func<Wallet, bool>>? filterWallet= null, 
        Expression<Func<Account, bool>>? filterAccount = null, 
        Expression<Func<Payment, bool>>? filterPayment = null, 
        string[]? includeProperties = null, 
        int pageIndex = 0, 
        int pageSize = 10);
    
    Task<OperationResult<Payment>> CreatePayment(PaymentRequestCreate paymentRequestCreate);
}
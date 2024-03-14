using System.Linq.Expressions;
using AutoMapper;
using Capstone.UniFarm.Domain.Enum;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Repositories.UnitOfWork;
using Capstone.UniFarm.Services.Commons;
using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;

namespace Capstone.UniFarm.Services.CustomServices;

public class PaymentService : IPaymentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IAccountService _accountService;
    private readonly IWalletService _walletService;

    public PaymentService(IUnitOfWork unitOfWork, IMapper mapper, IAccountService accountService, IWalletService walletService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _accountService = accountService;
        _walletService = walletService;
    }

    public async Task<OperationResult<IEnumerable<Payment>>> GetAll(
        bool? isAscending,
        string? orderBy = null,
        Expression<Func<Wallet, bool>>? filterWallet = null,
        Expression<Func<Account, bool>>? filterAccount = null,
        Expression<Func<Payment, bool>>? filterPayment = null, 
        string[]? includeProperties = null,
        int pageIndex = 0, int pageSize = 10)
    {
        var result = new OperationResult<IEnumerable<Payment>>();
        try
        {
            
            var wallets = await _walletService.FindByExpression(filterWallet!);
            if(wallets.Payload == null)
            {
                result.IsError = false;
                result.Message = "Can not found wallet of account Id";
                result.StatusCode = StatusCode.NotFound;
                return result;
            }
            
            Expression<Func<Payment, bool>> filterPayment1 = x => x.WalletId == wallets.Payload.Id;

            var parameter = Expression.Parameter(typeof(Payment), "x");
            var body = Expression.AndAlso(
                Expression.Invoke(filterPayment1, parameter),
                Expression.Invoke(filterPayment!, parameter)
            );
            var combinedFilter = Expression.Lambda<Func<Payment, bool>>(body, parameter);
            
            var payments = _unitOfWork.PaymentRepository.FilterAll(isAscending, orderBy, combinedFilter , includeProperties, pageIndex, pageSize);
            if(payments.Any())
            {
                result.IsError = false;
                result.Message = "Payment not found";
                result.StatusCode = StatusCode.NotFound;
                return result;
            }
            result.Payload = payments;
            result.IsError = false;
            result.Message = "Get all payment successfully";
        }
        catch (Exception e)
        {
            result.IsError = false;
            result.StatusCode = StatusCode.ServerError;
            result.Message = e.Message;
        }

        return result;
    }

    public async Task<OperationResult<IEnumerable<Payment>>> GetAllForCustomer(
        bool? isAscending, 
        string? orderBy = null, 
        Expression<Func<Wallet, bool>>? filterWallet = null,
        Expression<Func<Account, bool>>? filterAccount = null, 
        Expression<Func<Payment, bool>>? filterPayment = null, 
        string[]? includeProperties = null,
        int pageIndex = 0, int pageSize = 10)
    {
        var result = new OperationResult<IEnumerable<Payment>>();
        
        try
        {
            var accounts = await _accountService.GetAccountByExpression(filterAccount);
            if(accounts.Payload == null)
            {
                result.IsError = false;
                result.Message = "Account not found";
                result.StatusCode = StatusCode.NotFound;
                return result;
            }
            var wallets = await _walletService.FindByExpression(filterWallet);
            if(wallets.Payload == null)
            {
                result.IsError = false;
                result.Message = "Wallet not found";
                result.StatusCode = StatusCode.NotFound;
                return result;
            }
            
            var payments = _unitOfWork.PaymentRepository.FilterAll(isAscending, orderBy, filterPayment, includeProperties, pageIndex, pageSize);
            
            if(payments == null)
            {
                result.IsError = false;
                result.Message = "Payment not found";
                result.StatusCode = StatusCode.NotFound;
                return result;
            }
            result.Payload = payments;
            result.IsError = false;
            result.Message = "Get all payment for customer successfully";
        }
        catch (Exception e)
        {
            result.IsError = false;
            result.StatusCode = StatusCode.ServerError;
            result.Message = e.Message;
        }

        return result;
    }

    public async  Task<OperationResult<Payment>> CreatePayment(PaymentRequestCreate paymentRequestCreate)
    {
        var result = new OperationResult<Payment>();
        try
        {
            var wallets = await _walletService.FindByExpression(x => x.Id == paymentRequestCreate.WalletId);
            if(wallets.Payload == null)
            {
                result.IsError = false;
                result.Message = "Wallet not found";
                result.StatusCode = StatusCode.NotFound;
                return result;
            }
            
            var payment = _mapper.Map<Payment>(paymentRequestCreate);
            
            if(payment.Type == EnumConstants.PaymentTypeEnum.DEPOSIT)
            {
                if (payment.Status == EnumConstants.PaymentEnum.SUCCESS)
                {
                    wallets.Payload.Balance += paymentRequestCreate.Amount;
                    wallets.Payload.UpdatedAt = DateTime.Now;
                    await _unitOfWork.WalletRepository.UpdateAsync(wallets.Payload);
                    payment.From = EnumConstants.PaymentTypeEnum.VNPAY;
                    payment.To = EnumConstants.PaymentTypeEnum.WALLET;
                }
                else
                {
                    result.IsError = false;
                    result.Message = "Deposit failed";
                    result.StatusCode = StatusCode.BadRequest;
                    return result;
                }
            }
            else if (payment.Type == EnumConstants.PaymentTypeEnum.WITHDRAW)
            {
                if(payment.Status == EnumConstants.PaymentEnum.SUCCESS)
                {
                    if (wallets.Payload.Balance >= paymentRequestCreate.Amount)
                    {
                        wallets.Payload.Balance -= paymentRequestCreate.Amount;
                        wallets.Payload.UpdatedAt = DateTime.Now;
                        await _unitOfWork.WalletRepository.UpdateAsync(wallets.Payload);
                        payment.From =  EnumConstants.PaymentTypeEnum.WALLET;
                        payment.To =  EnumConstants.PaymentTypeEnum.VNPAY;
                    }
                    else
                    {
                        result.IsError = false;
                        result.Message = "Balance is not enough";
                        result.StatusCode = StatusCode.BadRequest;
                        return result;
                    }
                }
            }
            
            var count = await _unitOfWork.SaveChangesAsync();
            if (count == 0)
            {
                result.IsError = false;
                result.Message = "Update wallet balance failed";
                result.StatusCode = StatusCode.BadRequest;
                return result;
            }
            await _unitOfWork.PaymentRepository.AddAsync(payment);
            count = await _unitOfWork.SaveChangesAsync();
            if (count == 0)
            {
                result.IsError = false;
                result.Message = "Update wallet failed";
                result.StatusCode = StatusCode.BadRequest;
                return result;
            }
            result.Payload = payment;
            result.IsError = false;
            result.Message = "Create payment successfully";
        }
        catch (Exception e)
        {
            result.IsError = false;
            result.StatusCode = StatusCode.ServerError;
            result.Message = e.Message;
        }

        return result;
    }
}
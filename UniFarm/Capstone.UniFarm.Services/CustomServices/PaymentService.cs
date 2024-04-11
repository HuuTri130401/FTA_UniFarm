using System.Linq.Expressions;
using AutoMapper;
using Capstone.UniFarm.Domain.Enum;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Repositories.UnitOfWork;
using Capstone.UniFarm.Services.Commons;
using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Capstone.UniFarm.Services.ViewModels.ModelResponses;
using Microsoft.EntityFrameworkCore;

namespace Capstone.UniFarm.Services.CustomServices;

public class PaymentService : IPaymentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IAccountService _accountService;
    private readonly IWalletService _walletService;

    public PaymentService(IUnitOfWork unitOfWork, IMapper mapper, IAccountService accountService,
        IWalletService walletService)
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
            if (wallets.Payload == null)
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

            var payments = _unitOfWork.PaymentRepository.FilterAll(isAscending, orderBy, combinedFilter,
                includeProperties, pageIndex, pageSize);
            if (payments.Any())
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
            if (accounts.Payload == null)
            {
                result.IsError = false;
                result.Message = "Account not found";
                result.StatusCode = StatusCode.NotFound;
                return result;
            }

            var wallets = await _walletService.FindByExpression(filterWallet);
            if (wallets.Payload == null)
            {
                result.IsError = false;
                result.Message = "Wallet not found";
                result.StatusCode = StatusCode.NotFound;
                return result;
            }

            var payments = _unitOfWork.PaymentRepository.FilterAll(isAscending, orderBy, filterPayment,
                includeProperties, pageIndex, pageSize);

            if (payments == null)
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

    public async Task<OperationResult<PaymentResponse>> CreatePaymentWithdrawRequest(Guid? accountId,
        PaymentWithdrawRequest request)
    {
        var result = new OperationResult<PaymentResponse>();
        var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var wallet = await _unitOfWork.WalletRepository.FilterByExpression(x => x.AccountId == accountId)
                .FirstOrDefaultAsync();
            if (wallet == null)
            {
                result.Message = "Wallet not found";
                result.IsError = true;
                result.Errors.Add(new Error()
                {
                    Code = StatusCode.NotFound,
                    Message = "Wallet not found"
                });
                return result;
            }

            if (wallet.Balance < request.Amount)
            {
                result.Message = "Not enough money";
                result.IsError = true;
                result.Errors.Add(new Error()
                {
                    Code = StatusCode.BadRequest,
                    Message = "Not enough money"
                });
                return result;
            }

            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                WalletId = wallet.Id,
                From = EnumConstants.FromToWallet.WALLET.ToString(),
                To = EnumConstants.FromToWallet.BANK.ToString(),
                Amount = request.Amount,
                Status = EnumConstants.PaymentEnum.PENDING.ToString(),
                Type = EnumConstants.PaymentMethod.WITHDRAW.ToString(),
                Wallet = null,
                BankName = request.BankName,
                BankOwnerName = request.BankOwnerName,
                BankAccountNumber = request.BankAccountNumber,
                Note = request.Note,
            };

            wallet.Balance -= request.Amount;
            wallet.UpdatedAt = DateTime.Now;
            wallet.Payments = null;
            await _unitOfWork.WalletRepository.UpdateAsync(wallet);
            var countUpdate = _unitOfWork.SaveChangesAsync();

            if (countUpdate.Result == 0)
            {
                await transaction.RollbackAsync();
                result.Message = "Payment failed";
                result.IsError = true;
                result.Errors.Add(new Error()
                {
                    Code = StatusCode.BadRequest,
                    Message = "Payment failed"
                });
                return result;
            }

            await _unitOfWork.PaymentRepository.AddAsync(payment);
            var count = _unitOfWork.SaveChangesAsync();
            if (count.Result == 0)
            {
                await transaction.RollbackAsync();
                result.Message = "Payment failed";
                result.IsError = true;
                result.Errors.Add(new Error()
                {
                    Code = StatusCode.BadRequest,
                    Message = "Payment failed"
                });
                return result;
            }

            await transaction.CommitAsync();

            var account = await _unitOfWork.AccountRepository.FilterByExpression(x => x.Id == wallet.AccountId)
                .FirstOrDefaultAsync();
            if (account == null)
            {
                result.Message = "Account not found";
                result.IsError = true;
                result.Errors.Add(new Error()
                {
                    Code = StatusCode.NotFound,
                    Message = "Account not found"
                });
                return result;
            }

            var paymentResponse = new PaymentResponse()
            {
                Id = payment.Id,
                UserName = account!.UserName,
                Balance = wallet.Balance ?? 0,
                TransferAmount = payment.Amount,
                From = payment.From!,
                To = payment.To!,
                CreatedAt = payment.CreatedAt,
                UpdatedAt = payment.UpdatedAt,
                PaymentDay = payment.PaymentDay,
                Status = payment.Status!,
                Type = payment.Type,
                BankName = payment.BankName,
                BankOwnerName = payment.BankOwnerName,
                BankAccountNumber = payment.BankAccountNumber,
                Note = payment.Note
            };

            result.Payload = paymentResponse;
            result.Message = "Create payment withdraw request success";
            result.IsError = false;
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            result.Message = e.Message;
            result.IsError = true;
            result.Errors.Add(new Error()
            {
                Code = StatusCode.ServerError,
                Message = e.Message
            });
        }

        return result;
    }

    public async Task<OperationResult<IEnumerable<PaymentResponse>>> GetPayment(
        bool? isAscending, string? orderBy, Expression<Func<Payment, bool>>? filter, int pageIndex, int pageSize)
    {
        var result = new OperationResult<IEnumerable<PaymentResponse>>();
        try
        {
            var payments = await _unitOfWork.PaymentRepository
                .FilterAll(isAscending, orderBy, filter, null, pageIndex, pageSize).ToListAsync();

            var paymentResponses = new List<PaymentResponse>();
            foreach (var payment in payments)
            {
                var wallet = await _unitOfWork.WalletRepository.FilterByExpression(x => x.Id == payment.WalletId)
                    .FirstOrDefaultAsync();
                if (wallet == null) continue;

                var account = await _unitOfWork.AccountRepository.FilterByExpression(x => x.Id == wallet.AccountId)
                    .FirstOrDefaultAsync();
                if (account == null) continue;

                var paymentResponse = new PaymentResponse()
                {
                    Id = payment.Id,
                    UserName = account!.UserName,
                    Balance = wallet.Balance ?? 0,
                    TransferAmount = payment.Amount,
                    From = payment.From!,
                    To = payment.To!,
                    CreatedAt = payment.CreatedAt,
                    UpdatedAt = payment.UpdatedAt,
                    PaymentDay = payment.PaymentDay,
                    Status = payment.Status!,
                    Type = payment.Type,
                    BankName = payment.BankName,
                    BankOwnerName = payment.BankOwnerName,
                    BankAccountNumber = payment.BankAccountNumber,
                    Note = payment.Note
                };
                paymentResponses.Add(paymentResponse);
            }

            result.Payload = paymentResponses;
            result.Message = "Get payment success";
            result.IsError = false;
        }
        catch (Exception e)
        {
            result.Message = e.Message;
            result.IsError = true;
            result.Errors.Add(new Error()
            {
                Code = StatusCode.ServerError,
                Message = e.Message
            });
        }

        return result;
    }
}
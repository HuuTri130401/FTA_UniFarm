using System.Linq.Expressions;
using AutoMapper;
using Capstone.UniFarm.Domain.Enum;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Repositories.UnitOfWork;
using Capstone.UniFarm.Services.Commons;
using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ThirdPartyService;
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
                BalanceBefore = wallet.Balance ?? 0,
                Amount = request.Amount,
                Status = EnumConstants.PaymentEnum.PENDING.ToString(),
                Type = EnumConstants.PaymentMethod.WITHDRAW.ToString(),
                Wallet = null,
                BankName = request.BankName,
                BankOwnerName = request.BankOwnerName,
                BankAccountNumber = request.BankAccountNumber,
                Note = request.Note,
                CreatedAt = DateTime.UtcNow.AddHours(7),
                Code = "WDR"+ Utils.RandomString(7)
            };

            wallet.Balance -= request.Amount;
            wallet.UpdatedAt = DateTime.UtcNow.AddHours(7);
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
                Balance = payment.BalanceBefore,
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
                    Balance = payment.BalanceBefore,
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

    public async Task<OperationResult<IEnumerable<PaymentResponse>>> GetPaymentForUser(bool? isAscending,
        string? orderBy, Expression<Func<Payment, bool>>? filter, Expression<Func<Account, bool>> filterAccount,
        int pageIndex,
        int pageSize)
    {
        var result = new OperationResult<IEnumerable<PaymentResponse>>();

        try
        {
            var account = await _unitOfWork.AccountRepository.FilterByExpression(filterAccount).FirstOrDefaultAsync();
            if (account == null)
            {
                result.IsError = false;
                result.Message = "Account not found";
                result.StatusCode = StatusCode.NotFound;
                return result;
            }

            var wallet = await _unitOfWork.WalletRepository.FilterByExpression(x => x.AccountId == account.Id)
                .FirstOrDefaultAsync();

            if (wallet == null)
            {
                result.IsError = false;
                result.Message = "Wallet not found";
                result.StatusCode = StatusCode.NotFound;
                return result;
            }

            Expression<Func<Payment, bool>> filterPayment = x => x.WalletId == wallet.Id;

            var paymentList = _unitOfWork.PaymentRepository
                .GetAllWithoutPaging(isAscending, orderBy, filterPayment, null);

            var payments = await paymentList.Where(filter!).ToListAsync();
            if (!payments.Any())
            {
                result.IsError = false;
                result.Message = "Payment not found";
                result.StatusCode = StatusCode.NotFound;
                return result;
            }

            var paymentResponses = new List<PaymentResponse>();
            foreach (var payment in payments)
            {
                var paymentResponse = new PaymentResponse()
                {
                    Id = payment.Id,
                    UserName = account!.UserName,
                    Balance = payment.BalanceBefore,
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
                    Code = payment.Code,
                    Note = payment.Note
                };
                paymentResponses.Add(paymentResponse);
            }

            result.Payload = paymentResponses;
            result.Message = "Get payment for user success";
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

    public async Task<OperationResult<PaymentResponse>> UpdateWithdrawRequest(PaymentUpdateStatus request)
    {
        var result = new OperationResult<PaymentResponse>();
        var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var payment = await _unitOfWork.PaymentRepository.FilterByExpression(x => x.Id == request.Id)
                .FirstOrDefaultAsync();
            if (payment == null)
            {
                result.Message = "Payment not found";
                result.IsError = true;
                result.Errors.Add(new Error()
                {
                    Code = StatusCode.NotFound,
                    Message = "Payment not found"
                });
                return result;
            }

            if (payment.Status == EnumConstants.PaymentEnum.PENDING.ToString())
            {
                var wallet = await _unitOfWork.WalletRepository.FilterByExpression(x => x.Id == payment.WalletId)
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

                if (request.Status.ToString() == EnumConstants.PaymentEnum.DENIED.ToString())
                {
                    wallet.Balance += payment.Amount;
                }
                else if (request.Status.ToString() == EnumConstants.PaymentEnum.SUCCESS.ToString())
                {
                    payment.PaymentDay = DateTime.UtcNow.AddHours(7);
                }
                else
                {
                    result.Message = "Payment status is already updated";
                    result.Payload = null;
                    return result;
                }

                await _unitOfWork.WalletRepository.UpdateAsync(wallet);
                var countUpdate = _unitOfWork.SaveChangesAsync();
                if (countUpdate.Result == 0)
                {
                    await transaction.RollbackAsync();
                    result.Message = "Update wallet failed";
                    result.IsError = true;
                    result.Errors.Add(new Error()
                    {
                        Code = StatusCode.BadRequest,
                        Message = "Update wallet failed"
                    });
                    return result;
                }

                payment.Status = request.Status.ToString();
                payment.UpdatedAt = DateTime.UtcNow.AddHours(7);
                await _unitOfWork.PaymentRepository.UpdateAsync(payment);
                var count = _unitOfWork.SaveChangesAsync();
                if (count.Result == 0)
                {
                    await transaction.RollbackAsync();
                    result.Message = "Update payment failed";
                    result.IsError = true;
                    result.Errors.Add(new Error()
                    {
                        Code = StatusCode.BadRequest,
                        Message = "Update payment failed"
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
                    Balance = payment.BalanceBefore,
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
                    Code = payment.Code,
                    Note = payment.Note
                };
                result.Message = "Update status success!";
                result.IsError = false;
                result.Payload = paymentResponse;
                return result;
            }

            result.Message = "Payment status is already updated";
            result.Payload = null;
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

    public async Task<OperationResult<Payment>> DepositMoneyTesting(Guid? accountId,
        PaymentRequestCreateModel requestModel)
    {
        var result = new OperationResult<Payment>();
        var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var wallet = await _unitOfWork.WalletRepository.FilterByExpression(x => x.Id == requestModel.WalletId)
                .FirstOrDefaultAsync();
            if (wallet == null)
            {
                wallet = await _unitOfWork.WalletRepository.FilterByExpression(x => x.AccountId == accountId)
                    .FirstOrDefaultAsync();

                if (wallet == null)
                {
                    await transaction.RollbackAsync();
                    result.Message = "Wallet not found";
                    result.IsError = true;
                    result.Errors.Add(new Error()
                    {
                        Code = StatusCode.NotFound,
                        Message = "Wallet not found"
                    });
                    return result;
                }
            }

            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                WalletId = requestModel.WalletId,
                From = requestModel.From.ToString(),
                To = requestModel.To.ToString(),
                BalanceBefore = wallet.Balance ?? 0,
                Amount = requestModel.Amount,
                PaymentDay = DateTime.UtcNow.AddHours(7),
                Status = EnumConstants.PaymentEnum.SUCCESS.ToString(),
                Type = requestModel.Type.ToString(),
                Wallet = null,
            };
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
            }

            if (requestModel.Type.ToString() == EnumConstants.PaymentMethod.DEPOSIT.ToString())
            {
                wallet.Balance += payment.Amount;
            }
            else
            {
                if (wallet.Balance >= payment.Amount)
                {
                    wallet.Balance -= payment.Amount;
                }
                else
                {
                    await transaction.RollbackAsync();
                    result.Message = "Not enough money";
                    result.IsError = true;
                    result.Errors.Add(new Error()
                    {
                        Code = StatusCode.BadRequest,
                        Message = "Not enough money"
                    });
                }
            }

            wallet.UpdatedAt = DateTime.UtcNow.AddHours(7);
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
            }

            await transaction.CommitAsync();
            payment.Wallet = null;
            result.Payload = payment;
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
}
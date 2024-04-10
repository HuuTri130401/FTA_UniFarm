using System.Linq.Expressions;
using AutoMapper;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Repositories.UnitOfWork;
using Capstone.UniFarm.Services.Commons;
using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelResponses;
using Microsoft.Extensions.Logging;

namespace Capstone.UniFarm.Services.CustomServices;

public class TransactionService : ITransactionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<TransactionService> _logger;

    public TransactionService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<TransactionService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }
    public async Task<OperationResult<IEnumerable<Transaction>>> GetAll(
    bool? isAscending,
    string? orderBy = null,
    Expression<Func<Transaction, bool>>? filter = null,
    string[]? includeProperties = null,
    int pageIndex = 0,
    int pageSize = 10)
    {
        var result = new OperationResult<IEnumerable<Transaction>>();
        try
        {
            var transactions = _unitOfWork.TransactionRepository.FilterAll(
                isAscending,
                orderBy,
                filter,
                includeProperties,
                pageIndex,
                pageSize);

            if (!transactions.Any())
            {
                result.StatusCode = StatusCode.NoContent;
                result.Message = "Chưa có giao dịch nào được tạo.";
                return result;
            }

            result.Payload = transactions;
            result.Message = "Lấy danh sách giao dịch thành công.";
            result.StatusCode = StatusCode.Ok;
            result.IsError = false;
        }
        catch (Exception e)
        {
            result.StatusCode = StatusCode.ServerError;
            result.Message = e.Message;
            result.IsError = true;
            throw;
        }
        return result;
    }

    public async Task<OperationResult<List<TransactionResponse>>> GetAllTransaction(Guid accountId)
    {
        var result = new OperationResult<List<TransactionResponse>>();
        try
        {
            var wallet = await _unitOfWork.WalletRepository.GetWalletByAccountIdAsync(accountId);
            if(wallet != null)
            {
                //var listTransactions = await _unitOfWork.TransactionRepository.GetAllTransactions(wallet.Id);
                //var listTransactionsResponse = _mapper.Map<List<TransactionResponse>>(listTransactions);
                //if (listTransactionsResponse == null || !listTransactionsResponse.Any())
                //{
                //    result.AddResponseStatusCode(StatusCode.Ok, $"List Transactions with accountId: {accountId} is Empty!", listTransactionsResponse);
                //    return result;
                //}
                //result.AddResponseStatusCode(StatusCode.Ok, "Get List Transactions Done.", listTransactionsResponse);

                var listTransactions = await _unitOfWork.TransactionRepository.GetAllTransactions(wallet.Id);
                var listTransactionResponses = new List<TransactionResponse>();

                foreach (var transaction in listTransactions)
                {
                    var payeeWallet =  _unitOfWork.WalletRepository.GetById(transaction.PayeeWalletId);
                    var payeeAccount = await _unitOfWork.AccountRepository.GetByIdAsync(payeeWallet.AccountId);

                    var transactionResponse = _mapper.Map<TransactionResponse>(transaction);
                    // Thiết lập PayeeName dựa trên thông tin Account tìm được
                    transactionResponse.PayeeName = payeeAccount.UserName;
                    listTransactionResponses.Add(transactionResponse);
                }

                if (!listTransactionResponses.Any())
                {
                    result.AddResponseStatusCode(StatusCode.Ok, $"List Transactions with accountId: {accountId} is Empty!", listTransactionResponses);
                    return result;
                }

                result.AddResponseStatusCode(StatusCode.Ok, "Get List Transactions Done.", listTransactionResponses);
            }
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred in GetAllTransaction Service Method");
            throw;
        }
    }


    //public async Task<OperationResult<IEnumerable<Transaction>>> GetAll(
    //    bool? isAscending, 
    //    string? orderBy = null, 
    //    Expression<Func<Transaction, bool>>? filter = null, 
    //    string[]? includeProperties = null,
    //    int pageIndex = 0, 
    //    int pageSize = 10)
    //{
    //    var result = new OperationResult<IEnumerable<Transaction>>();
    //    try
    //    {
    //        var transactions = _unitOfWork.TransactionRepository.FilterAll(
    //            isAscending, 
    //            orderBy, 
    //            filter, 
    //            includeProperties, 
    //            pageIndex, 
    //            pageSize);

    //        if (!transactions.Any())
    //        {
    //            result.StatusCode = StatusCode.NoContent;
    //            result.Message = "Chưa có giao dịch nào được tạo.";
    //            return result;
    //        }

    //        result.Payload = transactions;
    //        result.Message = "Lấy danh sách giao dịch thành công.";
    //        result.StatusCode = StatusCode.Ok;
    //        result.IsError = false;
    //    }
    //    catch (Exception e)
    //    {
    //        result.StatusCode = StatusCode.ServerError;
    //        result.Message = e.Message;
    //        result.IsError = true;
    //        throw;
    //    }
    //    return result;
    //}
}
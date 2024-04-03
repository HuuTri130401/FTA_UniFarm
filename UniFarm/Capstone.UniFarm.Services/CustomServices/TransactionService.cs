using System.Linq.Expressions;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Repositories.UnitOfWork;
using Capstone.UniFarm.Services.Commons;
using Capstone.UniFarm.Services.ICustomServices;

namespace Capstone.UniFarm.Services.CustomServices;

public class TransactionService : ITransactionService
{
    private readonly IUnitOfWork _unitOfWork;
    
    public TransactionService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
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
}
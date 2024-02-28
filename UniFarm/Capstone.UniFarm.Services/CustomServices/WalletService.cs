using System.Linq.Expressions;
using AutoMapper;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Repositories.UnitOfWork;
using Capstone.UniFarm.Services.Commons;
using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Microsoft.Extensions.Logging;

namespace Capstone.UniFarm.Services.CustomServices;

public class WalletService : IWalletService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ProductService> _logger;
    private readonly IMapper _mapper;

    public WalletService(IUnitOfWork unitOfWork, ILogger<ProductService> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }
    
    public async Task<OperationResult<Wallet>> Create(WalletRequest objectRequestCreate)
    {
        var result = new OperationResult<Wallet>();
        try
        {
            var objectWallet = _mapper.Map<Wallet>(objectRequestCreate);
            var objectCreated = await _unitOfWork.WalletRepository.AddAsync(objectWallet);
            var checkResult = await _unitOfWork.SaveChangesAsync();
            if (checkResult > 0)
            {
                result.StatusCode = StatusCode.Created;
                result.Payload = objectWallet;
            }
            else
            {
                result.IsError = true;
                result.AddError(StatusCode.BadRequest, "Add Wallet Failed!");
                result.Payload = null;
            }
        }
        catch (Exception ex )
        {
            result.AddError(StatusCode.BadRequest, ex.Message);
            _logger.LogError("Wallet create error" + ex.Message);
            throw;
        }

        return result;
    }

    public Task<OperationResult<Wallet>> Update(Guid Id, Wallet objectRequestUpdate)
    {
        throw new NotImplementedException();
    }
    
    public Task<OperationResult<IEnumerable<Wallet>>> GetAll(bool? isAscending, string? orderBy = null, Expression<Func<Area, bool>>? filter = null, string[]? includeProperties = null,
        int pageIndex = 0, int pageSize = 10)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<Wallet>> GetById(Guid objectId)
    {
        throw new NotImplementedException();
    }
}
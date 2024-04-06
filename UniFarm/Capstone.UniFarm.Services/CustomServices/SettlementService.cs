using AutoMapper;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Repositories.UnitOfWork;
using Capstone.UniFarm.Services.Commons;
using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Capstone.UniFarm.Services.ViewModels.ModelResponses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Services.CustomServices
{
    public class SettlementService : ISettlementService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<SettlementService> _logger;

        public SettlementService(IUnitOfWork unitOfWork, ILogger<SettlementService> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<OperationResult<FarmHubSettlementRequest>> CreateSettlementForBusinessDay(Guid businessDayId)
        {
            var result = new OperationResult<FarmHubSettlementRequest>();
            try
            {
                var totalsAndNumOrderOfFarmHub = await _unitOfWork.OrderRepository.CalculateTotalForBusinessDayByFarmHub(businessDayId);
                var priceTable = await _unitOfWork.PriceTableRepository.GetPriceTable();

                foreach (var totalAmountAndNumOrder in totalsAndNumOrderOfFarmHub)
                {
                    var farmHubId = totalAmountAndNumOrder.Key;
                    var totalSalesAndNumberOrder = totalAmountAndNumOrder.Value;

                    var commissionFee = await _unitOfWork.OrderDetailRepository.CalculateCommissionFee(farmHubId, businessDayId);
                    var dailyFee = await CalculateDailyFee((decimal)totalSalesAndNumberOrder.TotalAmount);

                    FarmHubSettlementRequest farmHubSettlementRequest = new FarmHubSettlementRequest();

                    farmHubSettlementRequest.FarmHubId = farmHubId;
                    farmHubSettlementRequest.BusinessDayId = businessDayId;
                    farmHubSettlementRequest.PriceTableId = priceTable.Id;
                    farmHubSettlementRequest.TotalSales = (decimal)totalSalesAndNumberOrder.TotalAmount;
                    farmHubSettlementRequest.CommissionFee = commissionFee;
                    farmHubSettlementRequest.DailyFee = dailyFee;
                    farmHubSettlementRequest.NumOfOrder = totalSalesAndNumberOrder.OrderCount;
                    farmHubSettlementRequest.DeliveryFeeOfOrder = 30000;
                    farmHubSettlementRequest.AmountToBePaid = commissionFee + dailyFee + (totalSalesAndNumberOrder.OrderCount * 30000);
                    farmHubSettlementRequest.Profit = (decimal)totalSalesAndNumberOrder.TotalAmount - (commissionFee + dailyFee + (totalSalesAndNumberOrder.OrderCount * 30000));
                    farmHubSettlementRequest.PaymentStatus = "Pending";

                    var farmhubSetlement = _mapper.Map<FarmHubSettlement>(farmHubSettlementRequest);
                    farmhubSetlement.Id = new Guid();
                    await _unitOfWork.FarmHubSettlementRepository.AddAsync(farmhubSetlement);
                    var checkResult = _unitOfWork.Save();
                    if (checkResult > 0)
                    {
                        result.AddResponseStatusCode(StatusCode.Created, "Add Settlement Success!", farmHubSettlementRequest);
                        return result;
                    }
                }
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<OperationResult<FarmHubSettlement>> CreateSettlementForFarmHub(Guid businessDayId, Guid farmHubId)
        {
            var result = new OperationResult<FarmHubSettlement>();
            try
            {
                var existingSettlement = await _unitOfWork.FarmHubSettlementRepository.GetFarmHubSettlementAsync(businessDayId, farmHubId);
                var totalsAndNumOrderOfFarmHub = await _unitOfWork.OrderRepository.CalculateTotalForBusinessDayOfOneFarmHub(businessDayId, farmHubId);
                if (totalsAndNumOrderOfFarmHub.OrderCount == 0)
                {
                    result.AddResponseStatusCode(StatusCode.Ok, "No completed orders found for this FarmHub.", null);
                    return result;
                }
                var priceTable = await _unitOfWork.PriceTableRepository.GetPriceTable();

                var commissionFee = await _unitOfWork.OrderDetailRepository.CalculateCommissionFee(farmHubId, businessDayId);
                var dailyFee = await CalculateDailyFee((decimal)totalsAndNumOrderOfFarmHub.TotalAmount);

                // Khởi tạo đối tượng FarmHubSettlement mới và thiết lập các thuộc tính
                if(existingSettlement == null)
                {
                    existingSettlement = new FarmHubSettlement
                    {
                        Id = Guid.NewGuid(), // Tạo ID mới
                        FarmHubId = farmHubId,
                        BusinessDayId = businessDayId,
                        PriceTableId = priceTable.Id,
                        TotalSales = (decimal)totalsAndNumOrderOfFarmHub.TotalAmount,
                        CommissionFee = commissionFee,
                        DailyFee = dailyFee,
                        NumOfOrder = totalsAndNumOrderOfFarmHub.OrderCount,
                        DeliveryFeeOfOrder = 30000,
                        AmountToBePaid = commissionFee + dailyFee + (totalsAndNumOrderOfFarmHub.OrderCount * 30000),
                        Profit = (decimal)totalsAndNumOrderOfFarmHub.TotalAmount - (commissionFee + dailyFee + (totalsAndNumOrderOfFarmHub.OrderCount * 30000)),
                        PaymentStatus = "Pending",
                    };
                    await _unitOfWork.FarmHubSettlementRepository.AddAsync(existingSettlement);
                }
                else
                {
                    existingSettlement.TotalSales = (decimal)totalsAndNumOrderOfFarmHub.TotalAmount;
                    existingSettlement.CommissionFee = commissionFee;
                    existingSettlement.DailyFee = dailyFee;
                    existingSettlement.NumOfOrder = totalsAndNumOrderOfFarmHub.OrderCount;
                    existingSettlement.DeliveryFeeOfOrder = 30000; // Giả sử là giá trị cố định
                    existingSettlement.AmountToBePaid = commissionFee + dailyFee + (totalsAndNumOrderOfFarmHub.OrderCount * 30000);
                    existingSettlement.Profit = (decimal)totalsAndNumOrderOfFarmHub.TotalAmount - (commissionFee + dailyFee + (totalsAndNumOrderOfFarmHub.OrderCount * 30000));
                    existingSettlement.PaymentStatus = "Pending";

                    _unitOfWork.FarmHubSettlementRepository.Update(existingSettlement);
                }

                var checkResult = _unitOfWork.Save(); 
                if (checkResult > 0)
                {
                    result.AddResponseStatusCode(StatusCode.Created, "Settlement processed successfully!", existingSettlement);
                    return result;
                }
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<OperationResult<FarmHubSettlementResponse>> GetSettlementForFarmHub(Guid businessDayId, Guid farmHubId)
        {
            var result = new OperationResult<FarmHubSettlementResponse>();
            try
            {
                var farmHubSettlement = await _unitOfWork.FarmHubSettlementRepository.GetFarmHubSettlementAsync(businessDayId, farmHubId);
                if (farmHubSettlement == null)
                {
                    result.AddError(StatusCode.NotFound, $"Can't found farmHubSettlement with businessDayId: {businessDayId} and farmHubId: {farmHubId}");
                    return result;
                }
                var menuResponse = _mapper.Map<FarmHubSettlementResponse>(farmHubSettlement);
                result.AddResponseStatusCode(StatusCode.Ok, $"Get FarmHubSettlement by businessDayId: {businessDayId} and farmHubId: {farmHubId} Success!", menuResponse);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in GetSettlementForFarmHub service method for businessDayId: {businessDayId} and farmHubId: {farmHubId}");
                throw;
            }
        }

        private async Task<decimal> CalculateDailyFee(decimal totalSales)
        {
            var priceTable = await _unitOfWork.PriceTableRepository.GetPriceTable();
            decimal dailyFee = 0m;
            foreach (var item in priceTable.PriceTableItems)
            {
                if (totalSales >= item.FromAmount && totalSales <= item.ToAmount)
                {
                    decimal fee = totalSales * item.Percentage / 100;

                    fee = Math.Max(fee, item.MinFee);
                    fee = Math.Min(fee, item.MaxFee);

                    dailyFee = fee;
                    break; // Giả sử chỉ áp dụng một mức phí
                }
            }
            return dailyFee;
        }

    }
}

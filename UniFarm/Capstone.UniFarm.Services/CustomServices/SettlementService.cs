using AutoMapper;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Repositories.UnitOfWork;
using Capstone.UniFarm.Services.Commons;
using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Capstone.UniFarm.Services.ViewModels.ModelResponses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Capstone.UniFarm.Domain.Enum.EnumConstants;

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

        public async Task<OperationResult<bool>> PaymentProfitForFarmHubInBusinessDay(Guid businessDayId, Guid systemAcountId)
        {
            var result = new OperationResult<bool>();
            try
            {
                var businessDayStatusPaymentConfirm = await _unitOfWork.BusinessDayRepository.GetBusinessDayByIdAsync(businessDayId);
                var systemWallet = await _unitOfWork.WalletRepository.GetWalletByAccountIdAsync(systemAcountId);

                if (businessDayStatusPaymentConfirm.Status == CommonEnumStatus.PaymentConfirm.ToString())
                {
                    var listFarmHubSettlement = await _unitOfWork.FarmHubSettlementRepository.GetAllFarmHubSettlementAsync(businessDayId);
                    if (listFarmHubSettlement == null || !listFarmHubSettlement.Any())
                    {
                        result.AddResponseStatusCode(StatusCode.Ok, "List FarmHub Settlement Is Empty!", true);
                        return result;
                    }

                    foreach (var farmHubSettlement in listFarmHubSettlement)
                    {
                        //Check farmhub settlement is pending in one businessday
                        if (farmHubSettlement.PaymentStatus != "Paid" && farmHubSettlement.Profit > 0
                            && farmHubSettlement.BusinessDayId == businessDayId)
                        {
                            var accountRole = await _unitOfWork.AccountRoleRepository.GetAccountRoleForFarmHubAsync(farmHubSettlement.FarmHubId);
                            var accountId = accountRole.AccountId;

                            var profit = farmHubSettlement.Profit;
                            var farmHubWallet = await _unitOfWork.WalletRepository.GetWalletByAccountIdAsync(accountId);
                            if (farmHubWallet != null && systemWallet != null)
                            {
                                var transaction = new Transaction()
                                {
                                    Id = Guid.NewGuid(),
                                    Amount = profit,
                                    PaymentDate = DateTime.UtcNow.AddHours(7),
                                    Status = "Success",
                                    PayerWalletId = systemWallet.Id,
                                    PayeeWalletId = farmHubWallet.Id,
                                    TransactionType = TransactionEnum.Payout.ToString()
                                };
                                await _unitOfWork.TransactionRepository.AddAsync(transaction);
                                var balanceOfFarmHub = farmHubWallet.Balance + profit;
                                await _unitOfWork.WalletRepository.UpdateBalance(farmHubWallet.Id, (decimal)balanceOfFarmHub);
                                var balanceOfSystem = systemWallet.Balance - profit;
                                await _unitOfWork.WalletRepository.UpdateBalance(systemWallet.Id, (decimal)balanceOfSystem);
                                farmHubSettlement.PaymentStatus = "Paid";
                                farmHubSettlement.PaymentDate = DateTime.UtcNow.AddHours(7);
                                await _unitOfWork.FarmHubSettlementRepository.UpdateEntityAsync(farmHubSettlement);
                                await _unitOfWork.SaveChangesAsync();
                            }
                        }
                        else
                        {
                            break;
                        }
                    }

                    var listTransactions = await _unitOfWork.TransactionRepository.GetAllTransactionPayments();
                    if (listTransactions != null)
                    {
                        foreach (var transaction in listTransactions)
                        {
                            var order = await _unitOfWork.OrderRepository.GetByIdAsync((Guid)transaction.OrderId);
                            if (order != null 
                                && await _unitOfWork.OrderRepository.IsOrderCancelledAsync(order)
                                && !await _unitOfWork.TransactionRepository.AlreadyRefundedAsync((Guid)transaction.OrderId))
                            {
                                var accountId = order.CustomerId;
                                var customerWallet = await _unitOfWork.WalletRepository.GetWalletByAccountIdAsync(accountId);
                                var newTransaction = new Transaction()
                                {
                                    Id = Guid.NewGuid(),
                                    Amount = transaction.Amount,
                                    PaymentDate = DateTime.UtcNow.AddHours(7),
                                    Status = "Success",
                                    PayerWalletId = systemWallet.Id,
                                    PayeeWalletId = customerWallet.Id,
                                    TransactionType = TransactionEnum.Refund.ToString(),
                                    OrderId = transaction.OrderId,
                                };
                                await _unitOfWork.TransactionRepository.AddAsync(newTransaction);
                                var balanceOfCustomer = customerWallet.Balance + transaction.Amount;
                                await _unitOfWork.WalletRepository.UpdateBalance(customerWallet.Id, (decimal)balanceOfCustomer);
                                var balanceOfSystem = systemWallet.Balance - transaction.Amount;
                                await _unitOfWork.WalletRepository.UpdateBalance(systemWallet.Id, (decimal)balanceOfSystem);
                                await _unitOfWork.SaveChangesAsync();
                            }
                        }
                    }
                }

                businessDayStatusPaymentConfirm.Status = CommonEnumStatus.Completed.ToString();
                await _unitOfWork.BusinessDayRepository.UpdateEntityAsync(businessDayStatusPaymentConfirm);
                result.AddResponseStatusCode(StatusCode.Created, $"Payment Profit For FarmHub and Refund For Customer In BusinessDay: {businessDayId} Success!", true);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in PaymentProfitForFarmHubInBusinessDay method for businessDayId: {businessDayId}");
                throw;
            }
        }

        //public async Task<OperationResult<FarmHubSettlement>> CreateSettlementForFarmHub(Guid businessDayId, Guid farmHubId)
        //{
        //    var result = new OperationResult<FarmHubSettlement>();
        //    try
        //    {
        //        var existingSettlement = await _unitOfWork.FarmHubSettlementRepository.GetFarmHubSettlementAsync(businessDayId, farmHubId);
        //        var totalsAndNumOrderOfFarmHub = await _unitOfWork.OrderRepository.CalculateTotalForBusinessDayOfOneFarmHub(businessDayId, farmHubId);
        //        var priceTable = await _unitOfWork.PriceTableRepository.GetPriceTable();
        //        var commissionFee = await _unitOfWork.OrderDetailRepository.CalculateCommissionFee(farmHubId, businessDayId);
        //        var dailyFee = await CalculateDailyFee((decimal)totalsAndNumOrderOfFarmHub.TotalAmount);

        //        if (totalsAndNumOrderOfFarmHub.OrderCount == 0 && existingSettlement == null)
        //        {
        //            existingSettlement = new FarmHubSettlement
        //            {
        //                Id = Guid.NewGuid(),
        //                FarmHubId = farmHubId,
        //                BusinessDayId = businessDayId,
        //                PriceTableId = priceTable.Id,
        //                TotalSales = 0,
        //                CommissionFee = 0,
        //                DailyFee = 0,
        //                NumOfOrder = 0,
        //                DeliveryFeeOfOrder = 30000,
        //                AmountToBePaid = 0,
        //                Profit = 0,
        //                PaymentStatus = "Pending",
        //            };
        //            await _unitOfWork.FarmHubSettlementRepository.AddAsync(existingSettlement);
        //        }
        //        else if (totalsAndNumOrderOfFarmHub.OrderCount > 0 && existingSettlement == null)
        //        {
        //            existingSettlement = new FarmHubSettlement
        //            {
        //                Id = Guid.NewGuid(),
        //                FarmHubId = farmHubId,
        //                BusinessDayId = businessDayId,
        //                PriceTableId = priceTable.Id,
        //                TotalSales = (decimal)totalsAndNumOrderOfFarmHub.TotalAmount,
        //                CommissionFee = commissionFee,
        //                DailyFee = dailyFee,
        //                NumOfOrder = totalsAndNumOrderOfFarmHub.OrderCount,
        //                DeliveryFeeOfOrder = 30000,
        //                AmountToBePaid = commissionFee + dailyFee + (totalsAndNumOrderOfFarmHub.OrderCount * 30000),
        //                Profit = (decimal)totalsAndNumOrderOfFarmHub.TotalAmount - (commissionFee + dailyFee + (totalsAndNumOrderOfFarmHub.OrderCount * 30000)),
        //                PaymentStatus = "Pending",
        //            };
        //            await _unitOfWork.FarmHubSettlementRepository.AddAsync(existingSettlement);
        //        }

        //        var checkResult = _unitOfWork.Save();
        //        if (checkResult > 0)
        //        {
        //            result.AddResponseStatusCode(StatusCode.Created, "Settlement processed successfully!", existingSettlement);
        //            return result;
        //        }
        //        return result;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        public async Task<OperationResult<FarmHubSettlementResponse>> GetSettlementForFarmHub(Guid businessDayId, Guid farmHubId)
        {
            var result = new OperationResult<FarmHubSettlementResponse>();
            try
            {
                var totalsAndNumOrderOfFarmHub = await _unitOfWork.OrderRepository.CalculateTotalForBusinessDayOfOneFarmHub(businessDayId, farmHubId);
                var priceTable = await _unitOfWork.PriceTableRepository.GetPriceTable();
                var commissionFee = await _unitOfWork.OrderDetailRepository.CalculateCommissionFee(farmHubId, businessDayId);
                var existingSettlement = await _unitOfWork.FarmHubSettlementRepository.GetFarmHubSettlementAsync(businessDayId, farmHubId);
                if (existingSettlement != null && totalsAndNumOrderOfFarmHub.OrderCount > 0 && existingSettlement.PaymentStatus == "Pending")
                {
                        var dailyFee = await CalculateDailyFee((decimal)totalsAndNumOrderOfFarmHub.TotalAmount);
                        existingSettlement.TotalSales = (decimal)totalsAndNumOrderOfFarmHub.TotalAmount;
                        existingSettlement.CommissionFee = commissionFee;
                        existingSettlement.DailyFee = dailyFee;
                        existingSettlement.NumOfOrder = totalsAndNumOrderOfFarmHub.OrderCount;
                        existingSettlement.DeliveryFeeOfOrder = 30000; // Giả sử là giá trị cố định
                        existingSettlement.AmountToBePaid = commissionFee + dailyFee + (totalsAndNumOrderOfFarmHub.OrderCount * 30000);
                        existingSettlement.Profit = (decimal)totalsAndNumOrderOfFarmHub.TotalAmount - (commissionFee + dailyFee + (totalsAndNumOrderOfFarmHub.OrderCount * 30000));
                        existingSettlement.PaymentStatus = "Pending";
                        _unitOfWork.FarmHubSettlementRepository.Update(existingSettlement);
                        _unitOfWork.Save();
                        var farmHubSettlementResponse = _mapper.Map<FarmHubSettlementResponse>(existingSettlement);
                        result.AddResponseStatusCode(StatusCode.Ok, $"[PaymentStatus: Pending] Get FarmHubSettlement by businessDayId: {businessDayId} and farmHubId: {farmHubId} Success!", farmHubSettlementResponse);
                        return result;
                }
                else if (existingSettlement != null && totalsAndNumOrderOfFarmHub.OrderCount > 0 && existingSettlement.PaymentStatus == "Paid")
                {
                    var dailyFee = await CalculateDailyFee((decimal)totalsAndNumOrderOfFarmHub.TotalAmount);
                    existingSettlement.TotalSales = (decimal)totalsAndNumOrderOfFarmHub.TotalAmount;
                    existingSettlement.CommissionFee = commissionFee;
                    existingSettlement.DailyFee = dailyFee;
                    existingSettlement.NumOfOrder = totalsAndNumOrderOfFarmHub.OrderCount;
                    existingSettlement.DeliveryFeeOfOrder = 30000; // Giả sử là giá trị cố định
                    existingSettlement.AmountToBePaid = commissionFee + dailyFee + (totalsAndNumOrderOfFarmHub.OrderCount * 30000);
                    existingSettlement.Profit = (decimal)totalsAndNumOrderOfFarmHub.TotalAmount - (commissionFee + dailyFee + (totalsAndNumOrderOfFarmHub.OrderCount * 30000));
                    existingSettlement.PaymentStatus = "Paid";
                    _unitOfWork.FarmHubSettlementRepository.Update(existingSettlement);
                    _unitOfWork.Save();
                    var farmHubSettlementResponse = _mapper.Map<FarmHubSettlementResponse>(existingSettlement);
                    result.AddResponseStatusCode(StatusCode.Ok, $"[PaymentStatus: Paid] Get FarmHubSettlement by businessDayId: {businessDayId} and farmHubId: {farmHubId} Success!", farmHubSettlementResponse);
                    return result;
                }
                else
                {
                    if (totalsAndNumOrderOfFarmHub.OrderCount == 0)
                    {
                        existingSettlement = new FarmHubSettlement
                        {
                            Id = Guid.NewGuid(),
                            FarmHubId = farmHubId,
                            BusinessDayId = businessDayId,
                            PriceTableId = priceTable.Id,
                            TotalSales = 0,
                            CommissionFee = 0,
                            DailyFee = 0,
                            NumOfOrder = 0,
                            DeliveryFeeOfOrder = 30000,
                            AmountToBePaid = 0,
                            Profit = 0,
                            PaymentStatus = "Pending",
                        };
                        await _unitOfWork.FarmHubSettlementRepository.AddAsync(existingSettlement);
                    }
                    else if (totalsAndNumOrderOfFarmHub.OrderCount > 0)
                    {
                        var dailyFee = await CalculateDailyFee((decimal)totalsAndNumOrderOfFarmHub.TotalAmount);
                        existingSettlement = new FarmHubSettlement
                        {
                            Id = Guid.NewGuid(),
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

                    var checkResult = _unitOfWork.Save();
                    if (checkResult > 0)
                    {
                        var farmHubSettlementCreated = _mapper.Map<FarmHubSettlementResponse>(existingSettlement);
                        result.AddResponseStatusCode(StatusCode.Created, "Settlement processed successfully!", farmHubSettlementCreated);
                        return result;
                    }
                    return result;
                }
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

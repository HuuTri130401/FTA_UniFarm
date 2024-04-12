using System.Linq.Expressions;
using Capstone.UniFarm.Domain.Enum;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Repositories.UnitOfWork;
using Capstone.UniFarm.Services.Commons;
using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ThirdPartyService;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Capstone.UniFarm.Services.ViewModels.ModelResponses;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Capstone.UniFarm.Services.CustomServices
{
    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _config;
        private readonly IUnitOfWork _unitOfWork;

        public VnPayService(IConfiguration config, IUnitOfWork unitOfWork)
        {
            _config = config;
            _unitOfWork = unitOfWork;
        }

        public string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model)
        {
            var tick = DateTime.Now.Ticks.ToString();
            var vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", _config["VnPay:Version"]);
            vnpay.AddRequestData("vnp_Command", _config["VnPay:Command"]);
            vnpay.AddRequestData("vnp_TmnCode", _config["VnPay:TmnCode"]);
            vnpay.AddRequestData("vnp_Amount",
                (model.Amount * 100)
                .ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000

            vnpay.AddRequestData("vnp_CreateDate", model.CreatedDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", _config["VnPay:CurrCode"]);
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(context));
            vnpay.AddRequestData("vnp_Locale", _config["VnPay:Locale"]);

            vnpay.AddRequestData("vnp_OrderInfo", model.WalletId.ToString());
            vnpay.AddRequestData("vnp_OrderType", model.PaymentMethod.ToString()); //default value: other
            vnpay.AddRequestData("vnp_ReturnUrl", _config["VnPay:PaymentBackReturnUrl"]);
            vnpay.AddRequestData("vnp_TxnRef",
                tick); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày

            var paymentUrl = vnpay.CreateRequestUrl(_config["VnPay:BaseUrl"], _config["VnPay:HashSecret"]);
            return paymentUrl;
        }

        public VnPaymentResponseModel PaymentExecute(IQueryCollection collections)
        {
            var vnpay = new VnPayLibrary();
            foreach (var (key, value) in collections)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, value.ToString());
                }
            }

            var vnp_orderId = Convert.ToInt64(vnpay.GetResponseData("vnp_TxnRef"));
            var vnp_TransactionId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
            var vnp_SecureHash = collections.FirstOrDefault(p => p.Key == "vnp_SecureHash").Value;
            var vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            var vnp_OrderInfo = vnpay.GetResponseData("vnp_OrderInfo");
            var vnp_OrderType = vnpay.GetResponseData("vnp_OrderType");
            var vnp_Amount = Convert.ToDouble(vnpay.GetResponseData("vnp_Amount")) / 100;

            bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, _config["VnPay:HashSecret"]);
            if (!checkSignature)
            {
                return new VnPaymentResponseModel
                {
                    Success = false
                };
            }

            return new VnPaymentResponseModel
            {
                Success = true,
                WalletId = Guid.Parse(vnp_OrderInfo),
                PaymentMethod = vnp_OrderType,
                Amount = vnp_Amount,
                Token = vnp_SecureHash,
                VnPayResponseCode = vnp_ResponseCode
            };
        }

        public async Task<OperationResult<Payment>> DepositPayment(VnPaymentResponseModel response)
        {
            var result = new OperationResult<Payment>();
            var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var wallet = _unitOfWork.WalletRepository.FilterByExpression(x => x.Id == response.WalletId)
                    .FirstOrDefault();
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

                var payment = new Payment
                {
                    Id = Guid.NewGuid(),
                    WalletId = response.WalletId,
                    From = EnumConstants.FromToWallet.BANK.ToString(),
                    To = EnumConstants.FromToWallet.WALLET.ToString(),
                    BalanceBefore = wallet.Balance ?? 0,
                    Amount = Math.Round((decimal)response.Amount, 2),
                    PaymentDay = DateTime.Now,
                    Status = EnumConstants.PaymentEnum.SUCCESS.ToString(),
                    Type = response.PaymentMethod.ToString(),
                    Wallet = null,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    BankName = "VnPay",
                    BankOwnerName = "VnPay",
                    BankAccountNumber = "VnPay",
                    Code = "DEP" + Utils.RandomString(7)
                };
                
                // create random code start with DEP and 8 random characters and numbers

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

                if (response.PaymentMethod.ToString() == EnumConstants.PaymentMethod.DEPOSIT.ToString())
                {
                    wallet.Balance += payment.Amount;
                }
                else
                {
                    result.Message = "For deposit money only";
                    result.IsError = true;
                    result.Errors.Add(new Error()
                    {
                        Code = StatusCode.NotFound,
                        Message = "For deposit money only!"
                    });
                    return result;
                }

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
}
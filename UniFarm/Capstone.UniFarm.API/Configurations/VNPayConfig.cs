using System.Security.Cryptography;
using System.Text;

namespace Capstone.UniFarm.API.Configurations;

public class VNPayConfig
{
    public string vnp_Version { get; set; }
    public string vnp_Command { get; set; }
    public string vnp_TmnCode { get; set; }
    public string vnp_BankCode { get; set; }
    public string vnp_Locale { get; set; }
    public string vnp_CurrCode { get; set; }
    public string vnp_TxnRef { get; set; }
    public string vnp_OrderInfo { get; set; }
    public string vnp_OrderType { get; set; }
    public string vnp_Returnurl { get; set; }
    public string vnp_Url { get; set; }
    public string vnp_CardType { get; set; }
    public string vnp_HashSecret { get; set; }
    public string vnp_CreateDate { get; set; }

    public VNPayConfig(IConfiguration configuration)
    {
        vnp_Version = configuration["VNPayConfig:vnp_Version"];
        vnp_Command = configuration["VNPayConfig:vnp_Command"];
        vnp_TmnCode = configuration["VNPayConfig:vnp_TmnCode"];
        vnp_BankCode = configuration["VNPayConfig:vnp_BankCode"];
        vnp_Locale = configuration["VNPayConfig:vnp_Locale"];
        vnp_CurrCode = configuration["VNPayConfig:vnp_CurrCode"];
        vnp_TxnRef = RandomNumberGenerator.GetInt32(100000, 999999).ToString();
        vnp_OrderInfo = configuration["VNPayConfig:vnp_OrderInfo"];
        vnp_OrderType = configuration["VNPayConfig:vnp_OrderType"];
        vnp_HashSecret = ComputeSHA256Hash(configuration["VNPayConfig:vnp_HashSecret"]);
        vnp_Returnurl = configuration["VNPayConfig:vnp_Returnurl"];
        vnp_CreateDate = DateTime.Now.ToString("yyyyMMddHHmmss");
        vnp_Url = configuration["VNPayConfig:vnp_Url"];
        vnp_CardType = configuration["VNPayConfig:vnp_CardType"];
    }

    public bool IsConfigured()
    {
        return !string.IsNullOrEmpty(vnp_TmnCode) && !string.IsNullOrEmpty(vnp_HashSecret);
    }
    
    public static string ComputeSHA256Hash(string input)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }
}
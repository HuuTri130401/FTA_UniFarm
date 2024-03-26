namespace Capstone.UniFarm.API.Configurations;

public class VNPayConfig
{
    public string vnp_Returnurl { get; set; }
    public string vnp_Url { get; set; }
    public string vnp_TmnCode { get; set; }
    public string vnp_HashSecret { get; set; }

    public VNPayConfig(IConfiguration configuration)
    {
        vnp_Returnurl = configuration["VNPayConfig:vnp_Returnurl"];
        vnp_Url = configuration["VNPayConfig:vnp_Url"];
        vnp_TmnCode = configuration["VNPayConfig:vnp_TmnCode"];
        vnp_HashSecret = configuration["VNPayConfig:vnp_HashSecret"];
    }

    public bool IsConfigured()
    {
        return !string.IsNullOrEmpty(vnp_TmnCode) && !string.IsNullOrEmpty(vnp_HashSecret);
    }
}
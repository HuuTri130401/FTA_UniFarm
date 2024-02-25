using System.ComponentModel.DataAnnotations;

namespace Capstone.UniFarm.Services.ViewModels.ModelRequests
{
    public class GoogleLoginRequest
    {
        [Required]
        public string GoogleAccessToken { get; set; }

    }
}

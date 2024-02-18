using System.ComponentModel.DataAnnotations;

namespace Capstone.UniFarm.Services.ViewModels.Authen.Request
{
    public class GoogleLoginVM
    {
        [Required]
        public string GoogleAccessToken { get; set; }

    }
}

using System.ComponentModel.DataAnnotations;

namespace Capstone.UniFarm.API.ViewModels.Authen.Request
{
    public class GoogleLoginVM
    {
        [Required]
        public string GoogleAccessToken { get; set; }

    }
}

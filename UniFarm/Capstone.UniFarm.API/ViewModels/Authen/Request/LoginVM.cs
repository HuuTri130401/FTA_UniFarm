using System.ComponentModel.DataAnnotations;

namespace Capstone.UniFarm.API.ViewModels.Authen.Request
{
    public class LoginVM
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}

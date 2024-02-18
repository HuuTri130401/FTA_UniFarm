using System.ComponentModel.DataAnnotations;

namespace Capstone.UniFarm.Services.ViewModels.Authen.Request
{
    public record LoginVM
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}

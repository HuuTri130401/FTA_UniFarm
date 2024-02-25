using System.ComponentModel.DataAnnotations;

namespace Capstone.UniFarm.Services.ViewModels.ModelRequests
{
    public record LoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}

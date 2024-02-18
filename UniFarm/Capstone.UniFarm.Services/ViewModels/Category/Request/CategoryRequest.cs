using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Capstone.UniFarm.Services.ViewModels.Category.Request
{
    public record CategoryRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Code { get; set; }
        public string Description { get; set; }
        [Required]
        public string Image { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        [Required]
        public int Status { get; set; }
        public int Index { get; set; }
    }
}

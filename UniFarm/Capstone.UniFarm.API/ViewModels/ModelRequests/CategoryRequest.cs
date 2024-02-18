using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Capstone.UniFarm.API.ViewModels.ModelRequests
{
    public class CategoryRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Code { get; set; }
        public string Description { get; set; }
        [Required]
        public string Image { get; set; }
        [Required]
        public int Status { get; set; }
        public int Index { get; set; }
    }
}

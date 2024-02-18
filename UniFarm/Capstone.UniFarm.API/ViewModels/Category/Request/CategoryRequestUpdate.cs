using System.ComponentModel.DataAnnotations;

namespace Capstone.UniFarm.API.ViewModels.Category.Request
{
    public class CategoryRequestUpdate
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public int Status { get; set; }
        public int Index { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace Capstone.UniFarm.Services.ViewModels.ModelRequests
{
    public class CategoryRequestUpdate
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public int Status { get; set; }
        public int Index { get; set; }
    }
}

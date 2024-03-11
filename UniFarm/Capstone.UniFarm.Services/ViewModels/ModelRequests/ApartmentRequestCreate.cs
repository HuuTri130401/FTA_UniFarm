using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Capstone.UniFarm.Services.ViewModels.ModelRequests;

public record ApartmentRequestCreate
{
    public Guid? Id { get; set; }
    public Guid? AreaId { get; set; }
    [StringLength(255)]
    [Required(ErrorMessage = "Name can not be empty")]
    public string Name { get; set; }
    [StringLength(100, MinimumLength = 5 , ErrorMessage = "The code must be at least 5 characters long.")]
    public string Code { get; set; }
    [StringLength(255)]
    [Required(ErrorMessage = "Address can not be empty")]
    public string Address { get; set; }
    [StringLength(100)]
    [RegularExpression("^(Active|Inactive)$", ErrorMessage = "Status can be either 'Active' or 'Inactive'.")]
    public string? Status { get; set; }
}
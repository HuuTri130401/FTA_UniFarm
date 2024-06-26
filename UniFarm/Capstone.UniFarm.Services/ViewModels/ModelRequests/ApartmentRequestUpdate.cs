﻿using System.ComponentModel.DataAnnotations;

namespace Capstone.UniFarm.Services.ViewModels.ModelRequests;

public record ApartmentRequestUpdate
{
    public Guid? Id { get; set; }
    public Guid? AreaId { get; set; }
    [StringLength(255)]
    [Required(ErrorMessage = "Name can not be empty")]
    public string Name { get; set; }
    [StringLength(100, MinimumLength = 5 , ErrorMessage = "The code must be at least 5 characters long.")]
    [RegularExpression(@"^[A-Za-z0-9-]+$", ErrorMessage = "Format code should be number, character or '-'. No special characters allowed.")]
    public string Code { get; set; }
    [StringLength(255)]
    [Required(ErrorMessage = "Address can not be empty")]
    public string Address { get; set; }
    [StringLength(100)]
    [RegularExpression("^(Active|Inactive)$", ErrorMessage = "Status can be either 'Active' or 'Inactive'.")]
    public string? Status { get; set; }
}
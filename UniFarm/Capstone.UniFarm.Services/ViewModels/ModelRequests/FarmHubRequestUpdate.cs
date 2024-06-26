﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Services.ViewModels.ModelRequests
{
    public class FarmHubRequestUpdate
    {
        [StringLength(50, ErrorMessage = "Name length cannot exceed 50 characters")]
        [MinLength(3, ErrorMessage = "Name length must be at least 3 characters")]
        public string? Name { get; set; }
        [StringLength(10, ErrorMessage = "Code length cannot exceed 10 characters")]
        [MinLength(4, ErrorMessage = "Code length must be at least 4 characters")]
        public string? Code { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        [StringLength(100, ErrorMessage = "Address length cannot exceed 100 characters")]
        public string? Address { get; set; }
        [RegularExpression("^(Active|Inactive)$", ErrorMessage = "Status must be either 'Active' or 'Inactive'")]
        public string? Status { get; set; }
    }
}

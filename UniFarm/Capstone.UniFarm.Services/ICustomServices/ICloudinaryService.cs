using Capstone.UniFarm.Services.Commons;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Services.ICustomServices
{
    public interface ICloudinaryService
    {
        Task<string> UploadImageAsync(IFormFile? imageFile);
    }
}

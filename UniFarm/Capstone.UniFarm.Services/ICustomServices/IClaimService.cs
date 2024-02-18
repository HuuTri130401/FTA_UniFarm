using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Services.ICustomServices;

public interface IClaimsService
{
    public Guid GetCurrentUserId { get; }
}

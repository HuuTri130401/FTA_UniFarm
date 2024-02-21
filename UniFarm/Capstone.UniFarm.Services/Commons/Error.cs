using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Services.Commons;

public class Error
{
    public StatusCode Code { get; set; }
    public string Message { get; set; }
}

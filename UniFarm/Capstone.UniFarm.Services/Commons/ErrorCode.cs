using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Services.Commons
{
    public enum ErrorCode
    {
        BadRequest = 400,
        NotFound = 404,
        ServerError = 500,
        UnAuthorize = 401,
        Forbidden = 403,
        UnknownError = 999
    }
}

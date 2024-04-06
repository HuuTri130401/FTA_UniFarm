using Hangfire.Dashboard;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Services.CustomServices
{
    public class AuthorizationFilterService : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            throw new NotImplementedException();
        }
        public bool Authorize(DashboardContext context)
        {
            // Lấy thông tin người dùng từ context (ví dụ, sử dụng xác thực cookie)
            var httpContext = context.GetHttpContext();

            // Kiểm tra điều kiện xác thực, ví dụ:
            return httpContext.User.Identity.IsAuthenticated && httpContext.User.IsInRole("Admin");
        }
    }
}

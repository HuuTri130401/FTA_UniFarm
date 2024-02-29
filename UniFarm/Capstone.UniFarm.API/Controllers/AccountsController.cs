using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Microsoft.AspNetCore.Mvc;

namespace Capstone.UniFarm.API.Controllers
{
    [ApiController]
    public class AccountsController : BaseController
    {
        private readonly IAccountService _accountService;
        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }
        
        #region API Create account for farmhub, CollectedHubStaff, StationHubStaff
        [HttpPost("account/create")]
        public async Task<IActionResult> CreateAccount([FromBody] RegisterRequest model)
        {
            var response = await _accountService.CreateAccount(model);
            return response.IsError ? HandleErrorResponse(response!.Errors) : Created("/api/v1/account/create",response.Payload);
        }
        #endregion
    }
}

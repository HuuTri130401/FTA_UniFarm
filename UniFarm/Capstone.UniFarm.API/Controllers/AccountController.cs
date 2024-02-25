using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Microsoft.AspNetCore.Mvc;

namespace Capstone.UniFarm.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : BaseController
    {
        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }
        
        #region API Create account for farmhub
        [HttpPost("create-account")]
        public async Task<IActionResult> CreateAccount([FromBody] RegisterRequest model)
        {
            var response = await _accountService.CreateAccount(model);
            return response.IsError ? HandleErrorResponse(response!.Errors) : Created("/api/account",response.Payload);
        }
        #endregion
    }
}

using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.Authen.Request;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Text;
using Capstone.UniFarm.Domain.Enum;

namespace Capstone.UniFarm.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : BaseController
    {
        private readonly UserManager<Account> _userManager;
        private readonly SignInManager<Account> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IAccountService _accountService;

        public LoginController(
            UserManager<Account> userManager,
            SignInManager<Account> signInManager,
            IConfiguration configuration,
            IAccountService accountService
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _accountService = accountService;
        }

        #region API Register account Customer
        [HttpPost("register")]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<IActionResult> Register([FromBody] RegisterVM model)
        {
            model.Role = RoleEnum.Customer;
            var response = await _accountService.CreateAccount(model);
            return response.IsError ? HandleErrorResponse(response.Errors) : Created("/api/login",response.Payload);
        }
        #endregion

        #region API Login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginVM model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email); 

            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return Unauthorized();
            }

            var signInResult = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

            if (!signInResult.Succeeded)
            {
                return Unauthorized();
            }

            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var token = _accountService.GenerateJwtToken(user, key);
            return Ok(new { Token = token });
        }
        #endregion

        #region API Login/Register google
        [HttpGet("google/login")]
        [SwaggerOperation(Summary = "Initiate Google login")]
        public async Task Login()
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties()
            {
                RedirectUri = Url.Action("GoogleResponse")
            });
        }
        #endregion

        #region API Google Reponse
        [HttpGet("google/response")]
        [SwaggerOperation(Summary = "Handle Google authentication response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            if (!result.Succeeded)
            {
                return BadRequest();
            }
            var claims = result.Principal.Claims;
            var info = await _signInManager.GetExternalLoginInfoAsync();
            var result2 = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            var refreshToken = info.AuthenticationProperties.GetTokenValue("refresh_token");
            var response = await _accountService.HandleLoginGoogle(claims);
            if(response == null || response.IsError)
            {
                return HandleErrorResponse(response!.Errors);
            }
            response.AccessToken = _accountService.GenerateJwtToken(response.Payload, key);
            return Ok(response);
        }
        #endregion

        #region API Logout
        [HttpGet("logout")]
        [SwaggerOperation(Summary = "Log the user out")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return NoContent();
        }
        #endregion
    }
}

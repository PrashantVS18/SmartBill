using Microsoft.AspNetCore.Http;
using Billing.Utility;
using Microsoft.AspNetCore.Mvc;
using Biling.DataModels.LoginModels;
using Billing.BussinessLogic;
using Microsoft.AspNetCore.Authorization;
namespace Billing.API.Controllers.Login
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _loginService;

        public LoginController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var result = await _loginService.LoginAsync(request);
                string abc = null;
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

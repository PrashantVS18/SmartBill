using Microsoft.AspNetCore.Http;

using Billing.Utility;
using Microsoft.AspNetCore.Mvc;
using Biling.DataModels.LoginModels;

namespace Billing.API.Controllers.Login
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        [HttpPost]
        [Route("login")]
        public IActionResult Login([FromBody] LoginRequest logInRequest)
        {
            try
            {
                LoginResponse response = new LoginResponse()
                {
                    Success = true,
                    Message = "sucessfully login",
                    Token = "abcd",
                    User = new User() { Username = logInRequest.UserName}
                };
                return Ok(response);
            }
            catch(Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

using Greenhouse.Data.Models;
using Greenhouse.Models;
using Greenhouse.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Greenhouse.Controllers
{
    [Route("api/identity")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IJwtHelper _jwtHelper;

        public IdentityController(UserManager<User> userManager,
            IJwtHelper jwtHelper)
        {
            _userManager = userManager;
            _jwtHelper = jwtHelper;
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginBody loginBody) 
        {
            var user = await _userManager.FindByNameAsync(loginBody.Username);
            if (user == null) return Unauthorized();

            var loggedIn = await _userManager.CheckPasswordAsync(user, loginBody.Password);
            if (!loggedIn) return Unauthorized();

            var accessToken = _jwtHelper.CreateJwtToken(new Claim(ClaimTypes.NameIdentifier, user.Id));
            var loginResult = new LoginResult
            {
                AccessToken = accessToken
            };

            return Ok(loginResult);
        }

        [HttpPost("registry")]
        public async Task<ActionResult> Registry([FromBody] RegistryBody registryBody)
        {
            if (await _userManager.FindByNameAsync(registryBody.Username) != null)
            {
                return BadRequest("This username already exists.");
            }                

            if (await _userManager.FindByEmailAsync(registryBody.Email) != null)
            {
                return BadRequest("This email already exists.");
            }

            var user = new User
            {
                UserName = registryBody.Username,
                Email = registryBody.Email
            };

            var createResult = await _userManager.CreateAsync(user, registryBody.Password);
            if (!createResult.Succeeded)
            {
                return BadRequest(string.Join(Environment.NewLine, createResult.Errors.Select(err => err.Description)));
            }

            return NoContent();
        }        
    }
}

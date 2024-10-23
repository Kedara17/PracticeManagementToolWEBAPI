using AuthApi.Helpers;
using Azure.Core;
using DataServices.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly TokenGeneration _tokenGeneration;
        public LoginController(TokenGeneration tokenGeneration)
        {
            _tokenGeneration = tokenGeneration;
        }
        [HttpPost]
        public async Task<IActionResult> GetToken(string emailId, string password)
        {
            // Validate email and password format manually or using custom validators
            if (string.IsNullOrEmpty(emailId) || !emailId.EndsWith("@miraclesoft.com"))
            {
                return BadRequest("Email must be ends with @miraclesoft.com.");
            }

            if (string.IsNullOrEmpty(password) || password.Length < 4)
            {
                return BadRequest("Password must be at least 4 characters long.");
            }

            // Use the AuthResponse object to retrieve both the token and the role
            var authResponse = await _tokenGeneration.Validate(emailId, password);
            if (authResponse != null && !string.IsNullOrEmpty(authResponse.Token))
            {
                // Return both token and role in the response
                return Ok(new
                {
                    Token = authResponse.Token,
                    Role = authResponse.Role
                });
            }
            else
            {
                return Unauthorized("Invalid emailId or password.");
            }
        }


        [HttpGet]
        [Authorize(Roles = "User,Admin")]  // Both User and Admin can access
        public IActionResult Hello()
        {
            return Ok("Hello User");
        }

        [HttpPost("admin")]
        [Authorize(Roles = "Admin")]  // Only Admin can perform this operation
        public IActionResult AdminOperation()
        {
            return Ok("This is an admin operation.");
        }

    }
}







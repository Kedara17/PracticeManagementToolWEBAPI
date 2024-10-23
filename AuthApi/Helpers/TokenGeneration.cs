using DataServices.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthApi.Helpers
{
    public class TokenGeneration
    {
        private IConfiguration _config;
        private ILogger<TokenGeneration> _logger;
        private string[] SourceRoles = new[] { "Admin", "User" };
        private IEmployeeLoginRepository _employeeLoginRepository;
        public TokenGeneration(IConfiguration config, ILogger<TokenGeneration> logger, IEmployeeLoginRepository employeeLoginRepository)
        {
            _config = config;
            _logger = logger;
            _employeeLoginRepository = employeeLoginRepository;
        }
        public class AuthResponse
        {
            public string Token { get; set; }
            public string Role { get; set; }
        }

        public async Task<AuthResponse> Validate(string emailId, string password)
        {
            AuthResponse authResponse = null;
            bool isValidUser = await _employeeLoginRepository.Validate(emailId, password);

            if (isValidUser)
            {
                string role = await _employeeLoginRepository.GetUserRole(emailId);
                string employeeName = await _employeeLoginRepository.GetEmployeeName(emailId);
                string token = GenerateToken(emailId, role, employeeName); 
                authResponse = new AuthResponse
                {
                    Token = token,
                    Role = role
                };
            }

            return authResponse;
        }


        private string GenerateToken(string emailId, string role, string employeeName)
        {
            try
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, emailId),
                    new Claim(ClaimTypes.Role, role), // Add the user's role to the claims
                    new Claim("EmployeeName", employeeName) // Custom claim for employee name
                };

                var token = new JwtSecurityToken(
                    issuer: _config["Jwt:Issuer"],
                    audience: _config["Jwt:Issuer"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(120),
                    signingCredentials: credentials
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }

    }
}





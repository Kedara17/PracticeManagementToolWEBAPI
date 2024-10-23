
using System.Security.Cryptography;
using System.Text;
using DataServices.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DataServices.Repositories
{
    public class EmployeeLoginRepository : IEmployeeLoginRepository
    {
        private readonly DataBaseContext _dbContext;
        private readonly ILogger _logger;

        public EmployeeLoginRepository(DataBaseContext dbContext, ILogger<EmployeeLoginRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<string> GetUserRole(string emailId)
        {
            // Assuming you have a relationship between Employee and Role tables
            var employee = await _dbContext.TblEmployee.Include(e => e.Roles)
                                                     .FirstOrDefaultAsync(e => e.EmailId == emailId);
            return employee?.Roles?.RoleName; // Return the role name
        }
        public async Task<string> GetEmployeeName(string emailId)
        {
            // Assuming TblEmployee has a property "Name" to store employee's name
            var employee = await _dbContext.TblEmployee
                .FirstOrDefaultAsync(e => e.EmailId == emailId);

            return employee?.Name; // Return the employee's name
        }

        public async Task<bool> Validate(string emailId, string password)
        {
            try
            {
                if (!string.IsNullOrEmpty(emailId) && !string.IsNullOrEmpty(password))
                {
                    // Get the user by email ID
                    var employee = await _dbContext.TblEmployee
                        .FirstOrDefaultAsync(u => u.EmailId == emailId);

                    if (employee == null)
                    {
                        return false; // No such user
                    }

                    // Hash the input password using the same method as when storing it
                    var hashedPassword = HashPassword(password);

                    // Compare the hashed password with the stored hashed password
                    if (employee.Password == hashedPassword)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash returns byte array
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Convert byte array to a string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServices.Repositories
{
    public interface IEmployeeLoginRepository
    {
        public Task<bool> Validate(string username, string password);
        Task<string> GetUserRole(string emailId); // New method to get the user's role
        Task<string> GetEmployeeName(string emailId);
    }
}



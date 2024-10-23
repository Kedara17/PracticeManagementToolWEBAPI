using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServices.Models
{
    public class Role : RoleDTO
    {
        public ICollection<Employee> Employee { get; set; }
    }

    public class RoleDTO 
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string RoleName { get; set; }
    }

}

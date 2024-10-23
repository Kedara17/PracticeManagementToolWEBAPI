using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServices.Models
{
    public class Department : DepartmentDTO
    {
        public ICollection<Employee> Employee { get; set; }
        public ICollection<Technology> Technology { get; set; }

    }
    public class DepartmentDTO : AuditData
    {
        public string Name { get; set; }        
    }
    public class DepartmentBaseDTO
    {
        [Required(ErrorMessage = "The Name field is required.")]
        [MinLength(3)]
        [MaxLength(50)]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Special characters and Digits are not allowed.")]
        public string Name { get; set; }
    }
    public class DepartmentCreateDTO : DepartmentBaseDTO
    {
    }
    public class DepartmentUpdateDTO : DepartmentBaseDTO
    {
        public string Id { get; set; }
    }

}
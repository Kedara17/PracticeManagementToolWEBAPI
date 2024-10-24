using DataServices.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataServices.Models
{
    public class Technology : TechnologyDTO
    {
        [ForeignKey("DepartmentId")]
        public string? DepartmentId { get; set; }
        public ICollection<ProjectTechnology> ProjectTechnology { get; set; }
        public ICollection<EmployeeTechnology> EmployeeTechnology { get; set; }
        public ICollection<SOWRequirementTechnology> SOWRequirementTechnology { get; set; }
        public ICollection<POCTechnology> POCTechnology { get; set; }
        public ICollection<NewLeadEnquiryTechnology> NewLeadEnquiryTechnology { get; set; }
        public Department? Department { get; set; }
    }
    public class TechnologyDTO : AuditData
    {
        public string Name { get; set; }
        public string? Department { get; set; }
    }
    public class TechnologyCreateDTO
    {
        [Required]
        [MinLength(3)]
        [MaxLength(50)]
        [StringLength(50)]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Special characters and Digits are not allowed.")]
        public string Name { get; set; }
        public string? Department { get; set; }
    }
    public class TechnologyUpdateDTO : TechnologyCreateDTO
    {
        public string Id { get; set; }
        public bool IsActive { get; set; }
    }
}
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
    public class Technology : AuditData
    {
        [ForeignKey("DepartmentId")]
        public string? DepartmentId { get; set; }
        public string Name { get; set; }
        public ICollection<ProjectTechnology> ProjectTechnology { get; set; }
        public ICollection<EmployeeTechnology> EmployeeTechnology { get; set; }
        public ICollection<SOWRequirementTechnology> SOWRequirementTechnology { get; set; }
        public ICollection<POCTechnology> POCTechnology { get; set; }
        public ICollection<NewLeadEnquiryTechnology> NewLeadEnquiryTechnology { get; set; }
        public Department? Department { get; set; }
    }
   
}
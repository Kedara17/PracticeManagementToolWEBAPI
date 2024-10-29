using DataServices.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

/*namespace DataServices.Models
{
    public class Certifications 
    {
       public string EmployeeId {  get; set; }
         
    }
    public class CertificationsDTO : AuditData
    {
        public string Employee { get; set; }
        public string Name { get; set; }
        public DateTime ExamDate { get; set; }

        public DateTime ValidTill { get; set; }
        public string Status { get; set; }

        public string Comments { get; set; }
    }
}*/




namespace DataServices.Models
{
    public class Certifications : CertificationsDTO
    {
        [StringLength(36)]
        public string? EmployeeId { get; set; }  // This is the FK for the Employee table

        [ForeignKey("Id")]  // Specify that EmployeeId is the ForeignKey
        public Employee Employee { get; set; }  // Navigation property to Employee entity
    }
    public class CertificationsDTO : AuditData
    {


        [Required]
        public string? EmployeeId { get; set; }

        public string? Name { get; set; }
        public DateTime? ExamDate { get; set; }
        public DateTime? ValidTill { get; set; }
        public string? Status { get; set; }
        public string? Comments { get; set; }
    }

}







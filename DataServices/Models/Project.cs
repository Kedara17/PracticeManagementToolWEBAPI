using DataServices.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServices.Models
{
    public class Project : ProjectDTO
    {
        public string ClientId { get; set; }
        [ForeignKey("ClientId")]
        public Client? Client { get; set; }
        [ForeignKey("TechnicalProjectManager")]
        public Employee? TechnicalProjectManagers { get; set; }
        [ForeignKey("SalesContact")]
        public Employee? SalesContacts { get; set; }
        [ForeignKey("PMO")]
        public Employee? PMOs { get; set; }
        public ICollection<SOW> SOWs { get; set; }
        public ICollection<ProjectEmployee> ProjectEmployees { get; set; }
        public ICollection<ProjectTechnology> Technology { get; set; }
        public ICollection<SuccessStories> SuccessStories { get; set; }
    }
    public class ProjectDTO : AuditData
    {
        public string Client { get; set; }
        [Required(ErrorMessage = "The ProjectName field is required.")]
        [MinLength(3)]
        [MaxLength(50)]
        [StringLength(50, ErrorMessage = "The ProjectName cannot exceed 50 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Special characters and Digits are not allowed.")]
        public string? ProjectName { get; set; }
        public string[] Technology { get; set; }
        public string? TechnicalProjectManager { get; set; }
        public string? SalesContact { get; set; }
        public string? PMO { get; set; }
        public DateTime? SOWSubmittedDate { get; set; }
        public DateTime? SOWSignedDate { get; set; }
        public DateTime? SOWValidTill { get; set; }
        public DateTime? SOWLastExtendedDate { get; set; }
    }
}

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
    public class Interviews : InterviewsDTO
    {
        public string? SOWRequirementId { get; set; }
        public string? StatusId { get; set; }

        [ForeignKey("SOWRequirementId")]
        public SOWRequirement SOWRequirement { get; set; }

        [ForeignKey("Recruiter")]
        public Employee Employee { get; set; }

        [ForeignKey("StatusId")]
        public InterviewStatus Status { get; set; }

    }
    public class InterviewsDTO : AuditData
    {
        public string? SOWRequirement { get; set; }
        [Required(ErrorMessage = "The Name field is required.")]
        [MinLength(3)]
        [MaxLength(200)]
        [StringLength(200, ErrorMessage = "The Name cannot exceed 200 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Special characters and Digits are not allowed.")]
        public string? Name { get; set; }
        public DateTime? InterviewDate { get; set; }
        [RegularExpression(@"^[0-9\s]+$", ErrorMessage = "Only Digits are allowed.")]
        public int? YearsOfExperience { get; set; }
        public string? Status { get; set; }
        public DateTime? On_Boarding { get; set; }
        public string? Recruiter { get; set; }

    }
}

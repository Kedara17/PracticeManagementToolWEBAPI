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
    public class SOW : SOWDTO
    {
        public string? ClientId { get; set; }
        public string? ProjectId { get; set; }

        [ForeignKey("ClientId")]
        public Client Client { get; set; }
        [ForeignKey("ProjectId")]
        public Project Project { get; set; }
        [ForeignKey("Status")]
        public SOWStatus SOWStatus { get; set; }
        public ICollection<SOWRequirement> SOWRequirement { get; set; }

    }
    public class SOWDTO : AuditData
    {
        [Required(ErrorMessage = "The Title field is required.")]
        [MinLength(3)]
        [MaxLength(200)]
        [StringLength(200, ErrorMessage = "The Title cannot exceed 200 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Special characters and Digits are not allowed.")]
        public string? Title { get; set; }
        public string? Client { get; set; }
        public string? Project { get; set; }
        public DateTime? PreparedDate { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public string? Status { get; set; }
        [MinLength(3)]
        [MaxLength(500)]
        [StringLength(500, ErrorMessage = "The Comments cannot exceed 200 characters.")]
        public string? Comments { get; set; }
    }
}

using DataServices.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServices.Models
{
    public class InterviewStatus : InterviewStatusDTO
    {
        public ICollection<Interviews> Interviews { get; set; }

    }
    public class InterviewStatusDTO : AuditData
    {
        [Required(ErrorMessage = "The Status field is required.")]
        [MinLength(3)]
        [MaxLength(50)]
        [StringLength(50, ErrorMessage = "The Status cannot exceed 50 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Special characters and Digits are not allowed.")]
        public string? Status { get; set; }
    }
}




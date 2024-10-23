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
    public class Webinars : WebinarsDTO
    {
        [ForeignKey("Speaker")]
        public Employee Employee { get; set; }
    }
    public class WebinarsDTO : AuditData
    {
        [Required(ErrorMessage = "The Title field is required.")]
        [MinLength(3)]
        [MaxLength(200)]
        [StringLength(200, ErrorMessage = "The Title cannot exceed 200 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Special characters and Digits are not allowed.")]
        public string? Title { get; set; }
        public string? Speaker { get; set; }
        public string? Status { get; set; }
        public DateTime? WebinarDate { get; set; }
        [RegularExpression(@"^[0-9\s]+$", ErrorMessage = "Only Digits are allowed.")]
        public int? NumberOfAudience { get; set; }

    }
}

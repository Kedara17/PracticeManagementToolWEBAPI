using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServices.Models
{
    public class SOWStatus : SOWStatusDTO
    {
        public ICollection<SOW> SOWs { get; set; }

    }
    public class SOWStatusDTO : AuditData
    {
        [Required(ErrorMessage = "The Status field is required.")]
        [MinLength(3)]
        [MaxLength(50)]
        [StringLength(50, ErrorMessage = "The Status cannot exceed 50 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Special characters and Digits are not allowed.")]
        public string Status { get; set; }

    }
}

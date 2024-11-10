using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServices.Models.ViewModels
{
    public class TechnologyViewModel : AuditData
    {
        [Required]
        [MinLength(3)]
        [MaxLength(50)]
        [StringLength(50)]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Special characters and Digits are not allowed.")]
        public string Name { get; set; }
        public string? Department { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServices.Models
{
    public class BestPerformers : BestPerformersDTO
    {
        public Employee? Employee { get; set; }
        public Client? Client { get; set; }
        public Project? Project { get; set; }
    }
    public class BestPerformersDTO : AuditData
    {
     //   [Key]
        //public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required(ErrorMessage = "Employee ID is required.")]
        [MaxLength(36, ErrorMessage = "Employee ID cannot exceed 36 characters.")]
        public string? EmployeeID { get; set; }

        [Required(ErrorMessage = "Frequency is required.")]
        [MaxLength(50, ErrorMessage = "Frequency cannot exceed 50 characters.")]
        public string? Frequency { get; set; }

        [Required(ErrorMessage = "Client ID is required.")]
        [MaxLength(36, ErrorMessage = "Client ID cannot exceed 36 characters.")]
        public string? ClientID { get; set; }

        [Required(ErrorMessage = "Project ID is required.")]
        [MaxLength(36, ErrorMessage = "Project ID cannot exceed 36 characters.")]
        public string? ProjectID { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataServices.Models;
using System.Text.Json.Serialization;

namespace DataServices.Models
{
    public class BestPerformers : BestPerformersDTO
    {
        [ForeignKey("EmployeeID")]
        public string? EmployeeID { get; set; }

        [ForeignKey("ClientID")]
        public string? ClientID { get; set; }

        [ForeignKey("ProjectID")]
        public Employee? ProjectID { get; set; }
        public Client? Employee { get; set; }
        public Client? Client { get; set; }
        public Project? Project { get; set; }
    }
    public class BestPerformersDTO : AuditData
    {
     //   [Key]
        //public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required(ErrorMessage = "Employee ID is required.")]
        [MaxLength(36, ErrorMessage = "Employee ID cannot exceed 36 characters.")]
        public string? Employee { get; set; }

        [Required(ErrorMessage = "Frequency is required.")]
        [MaxLength(50, ErrorMessage = "Frequency cannot exceed 50 characters.")]
        public string Frequency { get; set; }

        [Required(ErrorMessage = "Client ID is required.")]
        [MaxLength(36, ErrorMessage = "Client ID cannot exceed 36 characters.")]
        public string? Client { get; set; }

        [Required(ErrorMessage = "Project ID is required.")]
        [MaxLength(36, ErrorMessage = "Project ID cannot exceed 36 characters.")]
        public string? Project { get; set; }

    }
    public class BestPerfomersBaseDTO
    {
        public string Name { get; set; }
        public string? EmployeeID { get; set; }
        [Required]
        [MinLength(3)]
        [MaxLength(50)]
        [StringLength(50)]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Special characters and Digits are not allowed.")]
        public string Frequency { get; set; }
        public string? ClientID { get; set; }
        public string? ProjectID { get; set; }
    }
    public class BestPerfomersCreateDTO : BestPerfomersBaseDTO
    { }
    public class BestPerformersUpdateDTO : BestPerfomersCreateDTO
    {
        public string Id { get; set; }
    }
}

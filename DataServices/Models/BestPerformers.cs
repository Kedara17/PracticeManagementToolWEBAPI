using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required(ErrorMessage = "Employee ID is required.")]
        [MaxLength(36, ErrorMessage = "Employee ID cannot exceed 36 characters.")]
        public string EmployeeID { get; set; } = string.Empty; // Make non-nullable if required

        [Required(ErrorMessage = "Frequency is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Frequency must be greater than 0.")]
        public int Frequency { get; set; } // Change to int if it is numeric

        [Required(ErrorMessage = "Client ID is required.")]
        [MaxLength(36, ErrorMessage = "Client ID cannot exceed 36 characters.")]
        public string ClientID { get; set; } = string.Empty; // Make non-nullable if required

        [Required(ErrorMessage = "Project ID is required.")]
        [MaxLength(36, ErrorMessage = "Project ID cannot exceed 36 characters.")]
        public string ProjectID { get; set; } = string.Empty; // Make non-nullable if required
    }
}
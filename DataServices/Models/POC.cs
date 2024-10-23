using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServices.Models
{
    public class POC : POCDTO
    {
        public string? ClientId { get; set; }
        public Client? Client { get; set; }
        public ICollection<POCTeam> POCTeam { get; set; }
        public ICollection<POCTechnology> POCTechnology { get; set; }

    }

    public class POCDocumentDTO
    {
        public string? Id { get; set; }
        public IFormFile Document { get; set; }

    }

    public class POCDTO : AuditData
    {
        [Required(ErrorMessage = "The Title field is required.")]
        [MinLength(3)]
        [MaxLength(200)]
        [StringLength(200, ErrorMessage = "The Title cannot exceed 200 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Special characters and Digits are not allowed.")]
        public string? Title { get; set; }
        public string? Client { get; set; }
        public string? Status { get; set; }
        public DateTime? TargetDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string? Document {  get; set; }

    }
}

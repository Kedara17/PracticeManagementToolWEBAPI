using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DataServices.Models
{
    public class NewLeadEnquiry : NewLeadEnquiryDTO
    {
        [ForeignKey("EmployeeID")]
        public Employee Employee { get; set; }
        [ForeignKey("AssignTo")]
        public Employee AssignedEmployee { get; set; }
        public ICollection<NewLeadEnquiryTechnology> NewLeadEnquiryTechnology { get; set; }
        public ICollection<NewLeadEnquiryFollowup> NewLeadEnquiryFollowup { get; set; }
        public ICollection<NewLeadEnquiryDocuments> NewLeadEnquiryDocuments { get; set; }
    }

    public class NewLeadEnquiryFileNameDTO
    {
        public string Id { get; set; }
        public IFormFile FileName { get; set; }

    }

    public class NewLeadEnquiryDTO : AuditData
    {
        [MinLength(30)]
        [MaxLength(50)]
        [StringLength(36)]
        public string EmployeeID { get; set; }
        [StringLength(36)]
        public string AssignTo { get; set; }
        [NotMapped]
        public string[] Technology { get; set; }
        [NotMapped]
        public string FileName { get; set; }

        [Required(ErrorMessage = "The Name field is required.")]
        [MinLength(3)]
        [MaxLength(50)]
        [StringLength(50, ErrorMessage = "The Name cannot exceed 50 characters.")]
        [RegularExpression(@"^[^0-9]*[.\-'\&]*$", ErrorMessage = "CompanyName can only contain letters, spaces, and certain special characters (., -, ', &).")]
        public string? CompanyName { get; set; }
        [Required(ErrorMessage = "The Name field is required.")]
        [MinLength(3)]
        [MaxLength(50)]
        [StringLength(50, ErrorMessage = "The Name cannot exceed 50 characters.")]
        [RegularExpression(@"^[^0-9]*[.\-'\&]*$", ErrorMessage = "CompanyName can only contain letters, spaces, and certain special characters (., -, ', &).")]
        public string? CompanyRepresentative { get; set; }
        [Required(ErrorMessage = "The Name field is required.")]
        [MinLength(3)]
        [MaxLength(50)]
        [StringLength(50, ErrorMessage = "The Name cannot exceed 50 characters.")]
        [RegularExpression(@"^[^0-9]*[.\-'\&]*$", ErrorMessage = "CompanyName can only contain letters, spaces, and certain special characters (., -, ', &).")]
        public string? RepresentativeDesignation { get; set; }
        [Required(ErrorMessage = "The Name field is required.")]
        [MinLength(3)]
        [MaxLength(50)]
        [StringLength(50, ErrorMessage = "The Name cannot exceed 50 characters.")]
        [RegularExpression(@"^[^0-9]*[.\-'\&]*$", ErrorMessage = "CompanyName can only contain letters, spaces, and certain special characters (., -, ', &).")]
        public string? Requirement { get; set; }
        public DateTime? EnquiryDate { get; set; }
        [StringLength(50)]
        public string Status { get; set; }
        [StringLength(500)]
        public string Comments { get; set; }

    }
}

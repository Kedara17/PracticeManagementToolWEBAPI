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

    public class NewLeadEnquiryProfileDTO
    {
        public string? Id { get; set; }
        public IFormFile Profile { get; set; }

    }

    public class NewLeadEnquiryDTO : AuditData
    {
        public string? EmployeeID { get; set; }
        public string? AssignTo { get; set; }
        [NotMapped]
        public string[] Technology { get; set; }
        public string? CompanyName { get; set; }
        public string? CompanyRepresentative { get; set; }
        public string? RepresentativeDesignation { get; set; }
        public string? Requirement { get; set; }
        public DateTime? EnquiryDate { get; set; }
        public string? Status { get; set; }
        public string? Comments { get; set; }
        public string? Profile { get; set; }

    }
}

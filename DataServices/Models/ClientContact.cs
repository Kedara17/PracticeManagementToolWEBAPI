using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServices.Models
{
    public class ClientContact : ClientContactDTO
    {
        public string ClientId { get; set; }

        [ForeignKey("ClientId")]
        public Client Client { get; set; }

        public string ContactTypeId { get; set; }
        [ForeignKey("ContactTypeId")]
        public ContactType ContactType { get; set; }
    }
    public class ClientContactDTO : AuditData
    {
        public string Client { get; set; }
        [Required(ErrorMessage = "The ContactValue field is required.")]
        [MinLength(3)]
        [MaxLength(50)]
        [StringLength(50, ErrorMessage = "The ContactValue cannot exceed 50 characters.")]
        //[RegularExpression(@"^[a-zA-Z0-9\s]+$", ErrorMessage = "Special characters and Digits are not allowed.")]
        public string? ContactValue { get; set; }

        public string? ContactType { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServices.Models
{
    public class Client : ClientDTO
    {
        public ICollection<ClientContact> ClientContact { get; set; }
        public ICollection<SOW> SOWs { get; set; }
        public ICollection<Project> Project { get; set; }
        public ICollection<POC> POC { get; set; }
        [ForeignKey("SalesEmployee")]
        public Employee Employee { get; set; }
    }

    public class ClientDTO : AuditData
    {
        [Required(ErrorMessage = "The Name field is required.")]
        [MinLength(3)]
        [MaxLength(50)]
        [StringLength(50, ErrorMessage = "The Name cannot exceed 50 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Special characters and Digits are not allowed.")]
        public string Name { get; set; }
        [StringLength(50)]
        public string ?LineofBusiness { get; set; }
        public string ?SalesEmployee { get; set; }
        [StringLength(50)]
        public string Country { get; set; }
        [StringLength(50)]
        public string City { get; set; }
        [StringLength(50)]
        public string State { get; set; }
        [StringLength(500)]
        public string Address { get; set; }
    }
}

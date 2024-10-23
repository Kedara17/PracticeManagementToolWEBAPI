using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServices.Models
{
    public class ContactType : ContactTypeDTO
    {
        public ICollection<ClientContact> ClientContact { get; set; }
    }
        public class ContactTypeDTO :AuditData
    {
        [Required(ErrorMessage = "The TypeName field is required.")]
        [MinLength(3)]
        [MaxLength(50)]
        [StringLength(50, ErrorMessage = "The TypeName cannot exceed 50 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Special characters and Digits are not allowed.")]
        public string TypeName { get; set; }
        
    }

}

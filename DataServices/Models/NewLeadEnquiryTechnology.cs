using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServices.Models
{
  public class NewLeadEnquiryTechnology : NewLeadEnquiryTechnologyDTO
    {

        [ForeignKey("TechnologyID")]
        public Technology Technology { get; set; }

        [ForeignKey("NewLeadEnquiryID")]
        public NewLeadEnquiry NewLeadEnquiry { get; set; }
    }
    public class NewLeadEnquiryTechnologyDTO : AuditData
    {
        public string? NewLeadEnquiryID { get; set; }
        public string? TechnologyID { get; set; }
    }
}

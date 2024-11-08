using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServices.Models
{
    public class NewLeadEnquiryDocuments : AuditData
    {
        [ForeignKey("NewLeadEnquiryID")]
        public NewLeadEnquiry NewLeadEnquirys { get; set; }
        public string NewLeadEnquiryID { get; set; }
        public string FileName { get; set; }

       
    }
    public class NewLeadEnquiryDocumentsDTO : AuditData
    {
        public string? NewLeadEnquiryID { get; set; }
        public string? FileName { get; set; }
    }
}

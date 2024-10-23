using DataServices.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServices.Models
{
    public class SOWRequirement : SOWRequirementDTO
    {
        public string? SOWId { get; set; }
        public string? DesignationId { get; set; }
        public ICollection<Interviews> Interviews { get; set; }
        public ICollection<SOWProposedTeam> SOWProposedTeam { get; set; }
        public ICollection<SOWRequirementTechnology> Technology { get; set; }
        [ForeignKey("SOWId")]
        public SOW SOW { get; set; }
        [ForeignKey("DesignationId")]
        public Designation Designation { get; set; }

    }

    public class SOWRequirementDTO : AuditData
    {
        public string? SOW { get; set; }
        public string? Designation { get; set; }
        public string[] Technology { get; set; }
        public int? TeamSize { get; set; }

    }
}

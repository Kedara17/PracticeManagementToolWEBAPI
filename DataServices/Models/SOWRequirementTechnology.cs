using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServices.Models
{
    public class SOWRequirementTechnology : SOWRequirementTechnologyDTO
    {
        public string? TechnologyId { get; set; }
        public string? SOWRequirementId { get; set; }
        [ForeignKey("SOWRequirementId")]
        public SOWRequirement SOWRequirements { get; set; }
        [ForeignKey("TechnologyId")]
        public Technology Technology { get; set; }
    }
    public class SOWRequirementTechnologyDTO : AuditData
    {
        public string? Technology { get; set; }
        public string? SOWRequirementId { get; set; }

    }
}

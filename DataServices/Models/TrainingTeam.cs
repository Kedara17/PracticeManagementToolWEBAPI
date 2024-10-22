using DataServices.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DataServices.Models
{
    public class TrainingTeam : TrainingTeamDTO
    {
        public string? TrainingId { get; set; }
        public string? EmployeeId { get; set; }
        //public Trainings? Trainings { get; set; }
        [ForeignKey("TrainingId")]  // Specify the foreign key for Trainings
        public Trainings? Trainings { get; set; }
        public Employee? Employee { get; set; }

    }

    public class TrainingTeamDTO : AuditData
    {
        public string? Trainings { get; set; }
        public string? Employee { get; set; }

    }
}

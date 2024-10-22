using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServices.Models
{
    public class Trainings : TrainingsDTO
    {
        public string? EmployeeId { get; set; }
        public Employee? Employee { get; set; }
        public ICollection<TrainingTeam> TrainingTeam { get; set; }
    }
    public class TrainingsDTO : AuditData
    {
        public string? Topic { get; set; }
        public string? Employee { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Status { get; set; }
        public string? Comments { get; set; }

    }
}



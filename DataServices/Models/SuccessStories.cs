

//model cls  SuccessStories


using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataServices.Models;
using Microsoft.AspNetCore.Http;

namespace DataServices.Models
{
    public class SuccessStories : SuccessStoriesDTO
    {
        [StringLength(36)]
        public string? ClientID { get; set; }
        public string? ProjectId { get; set; }


        [ForeignKey("ClientID")]
        public Client Client { get; set; }

        [ForeignKey("ProjectId")]
        public Project Project { get; set; }

        [ForeignKey("AssignTo")]
        public Employee Employee { get; set; }





    }

}


public class SuccessStoriesDTO : AuditData
{
    public string? Client { get; set; }
    public string? Project { get; set; }
    public string? AssignTo { get; set; }





    public string? Status { get; set; }
    public string? Comments { get; set; }


}



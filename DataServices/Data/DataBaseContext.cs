using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataServices.Models;
using Microsoft.EntityFrameworkCore;

namespace DataServices.Data
{
    public class DataBaseContext : DbContext
    {
        public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options)
        {
        }
        public DbSet<SuccessStories> TblSuccessStories { get; set; }
        public DbSet<Employee> TblEmployee { get; set; }
        public DbSet<Role> TblRole { get; set; }
        public DbSet<Blogs> TblBlogs { get; set; }        
        public DbSet<Designation> TblDesignation { get; set; }
        public DbSet<Technology> TblTechnology { get; set; }
        public DbSet<Project> TblProject { get; set; }
        public DbSet<ProjectEmployee> TblProjectEmployee { get; set; }
        public DbSet<ProjectTechnology> TblProjectTechnology { get; set; }
        public DbSet<Client> TblClient { get; set; }
        public DbSet<Interviews> TblInterviews { get; set; }
        public DbSet<SOWRequirement> TblSOWRequirement { get; set; }
        public DbSet<InterviewStatus> TblInterviewStatus { get; set; }
        public DbSet<SOWStatus> TblSOWStatus { get; set; }
        public DbSet<SOW> TblSOW { get; set; }
        public DbSet<Department> TblDepartment { get; set; }
        public DbSet<EmployeeTechnology> TblEmployeeTechnology { get; set; }
        public DbSet<ContactType> TblContactType { get; set; }
        public DbSet<ClientContact> TblClientContact { get; set; }
        public DbSet<Webinars> TblWebinars { get; set; }
        public DbSet<SOWProposedTeam> TblSOWProposedTeam { get; set; }
        public DbSet<SOWRequirementTechnology> TblSOWRequirementTechnology { get; set; }
        public DbSet<POC> TblPOC { get; set; }
        public DbSet<POCTeam> TblPOCTeam { get; set; }
        public DbSet<POCTechnology> TblPOCTechnology { get; set; }
        public DbSet<NewLeadEnquiry> TblNewLeadEnquiry { get; set; }
        public DbSet<NewLeadEnquiryTechnology> TblNewLeadEnquiryTechnology { get; set; }
        public DbSet<NewLeadEnquiryFollowup> TblNewLeadEnquireFollowup { get; set; }
        public DbSet<NewLeadEnquiryDocuments> TblNewLeadEnquiryDocuments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           
            //----------3rd table_Employee------------------------------------
            modelBuilder.Entity<Employee>()
            .HasOne(e => e.Designation)
            .WithMany(d => d.Employee)
            .HasForeignKey(e => e.DesignationId);

            modelBuilder.Entity<Employee>()
            .HasOne(e => e.Department)
            .WithMany(d => d.Employee)
            .HasForeignKey(e => e.DepartmentId);

            modelBuilder.Entity<Employee>()
            .HasOne(e => e.Roles)
            .WithMany(d => d.Employee)
            .HasForeignKey(e => e.Role);

            modelBuilder.Entity<Employee>()
            .HasOne(e => e.ReportingToEmployee)
            .WithMany(d => d.Subordinates)
            .HasForeignKey(e => e.ReportingTo);    


            //----------4th table Technology------------------------------------
            modelBuilder.Entity<Technology>()
                        .HasOne(t => t.Department)
                        .WithMany(d => d.Technology)
                        .HasForeignKey(t => t.DepartmentId);



            //----------5th table EmployeeTechnology------------------------------------
            modelBuilder.Entity<EmployeeTechnology>()
                        .HasOne(et => et.Technologies)
                        .WithMany(t => t.EmployeeTechnology)
                        .HasForeignKey(et => et.Technology);

            modelBuilder.Entity<EmployeeTechnology>()
            .HasOne(et => et.Employee)
            .WithMany(t => t.Technology)
            .HasForeignKey(et => et.EmployeeID);


            //----------6th table Client------------------------------------
            modelBuilder.Entity<Client>()
                        .HasOne(c => c.Employee)
                        .WithMany(e => e.Client)
                        .HasForeignKey(c => c.SalesEmployee);


            //----------8th table ClientContact------------------------------------
            modelBuilder.Entity<ClientContact>()
                        .HasOne(cc => cc.Client)
                        .WithMany(c => c.ClientContact)
                        .HasForeignKey(cc => cc.ClientId);

            modelBuilder.Entity<ClientContact>()
                .HasOne(cc => cc.ContactType)
                .WithMany(c => c.ClientContact)
                .HasForeignKey(cc => cc.ContactTypeId);

            //----------13th SOW table------------------------------------

            modelBuilder.Entity<SOW>()
                        .HasOne(pt => pt.Client)
                        .WithMany(c => c.SOWs)
                        .HasForeignKey(pt => pt.ClientId);

            modelBuilder.Entity<SOW>()
                        .HasOne(pt => pt.Project)
                        .WithMany(c => c.SOWs)
                        .HasForeignKey(pt => pt.ProjectId);

            modelBuilder.Entity<SOW>()
                        .HasOne(pt => pt.SOWStatus)
                        .WithMany(c => c.SOWs)
                        .HasForeignKey(pt => pt.Status);

            //----------14th SOWRequirement table------------------------------------
            modelBuilder.Entity<SOWRequirement>()
                        .HasOne(pt => pt.SOW)
                        .WithMany(c => c.SOWRequirement)
                        .HasForeignKey(pt => pt.SOWId);

            modelBuilder.Entity<SOWRequirement>()
                        .HasOne(pt => pt.Designation)
                        .WithMany(c => c.SOWRequirement)
                        .HasForeignKey(pt => pt.DesignationId);

            modelBuilder.Entity<SOWRequirementTechnology>()
                        .HasOne(et => et.Technology)
                        .WithMany(t => t.SOWRequirementTechnology)
                        .HasForeignKey(et => et.TechnologyId);

            modelBuilder.Entity<SOWRequirementTechnology>()
            .HasOne(et => et.SOWRequirements)
            .WithMany(t => t.Technology)
            .HasForeignKey(et => et.SOWRequirementId);

            //----------15th SOWPropsedTeam table------------------------------------
            modelBuilder.Entity<SOWProposedTeam>()
                        .HasOne(pt => pt.SOWRequirement)
                        .WithMany(c => c.SOWProposedTeam)
                        .HasForeignKey(pt => pt.SOWRequirementId);

            modelBuilder.Entity<SOWProposedTeam>()
                        .HasOne(pt => pt.Employee)
                        .WithMany(c => c.SOWProposedTeam)
                        .HasForeignKey(pt => pt.EmployeeId);



            //----------17th Interviews table------------------------------------
            modelBuilder.Entity<Interviews>()
                        .HasOne(pt => pt.SOWRequirement)
                        .WithMany(c => c.Interviews)
                        .HasForeignKey(pt => pt.SOWRequirementId);

            modelBuilder.Entity<Interviews>()
                                    .HasOne(pt => pt.Employee)
                                    .WithMany(c => c.Interviews)
                                    .HasForeignKey(pt => pt.Recruiter);

            modelBuilder.Entity<Interviews>()
                                    .HasOne(pt => pt.Status)
                                    .WithMany(c => c.Interviews)
                                    .HasForeignKey(pt => pt.StatusId);


            //----------18th Webinars table------------------------------------
            modelBuilder.Entity<Webinars>()
                                    .HasOne(pt => pt.Employee)
                                    .WithMany(c => c.Webinars)
                                    .HasForeignKey(pt => pt.Speaker);


            //----------19th Blogs table------------------------------------
            modelBuilder.Entity<Blogs>()
                                   .HasOne(pt => pt.Employee)
                                   .WithMany(c => c.Blog)
                                   .HasForeignKey(pt => pt.Author);

            //-----------Foreign key relation for Project model class-----------

            modelBuilder.Entity<Project>()
                .HasOne(c => c.Client)
                .WithMany(c => c.Project)
                .HasForeignKey(c => c.ClientId);

            modelBuilder.Entity<Project>()
                .HasOne(c => c.TechnicalProjectManagers)
                .WithMany(c => c.TechnicalProjects)
                .HasForeignKey(c => c.TechnicalProjectManager);

            modelBuilder.Entity<Project>()
                .HasOne(c => c.SalesContacts)
                .WithMany(c => c.SalesProjects)
                .HasForeignKey(c => c.SalesContact);

            modelBuilder.Entity<Project>()
                .HasOne(c => c.PMOs)
                .WithMany(c => c.PMOProjects)
                .HasForeignKey(c => c.PMO);

            //Foreign key relation for ProjectTechnology model class

            modelBuilder.Entity<ProjectEmployee>()
               .HasOne(c => c.Project)
               .WithMany(c => c.ProjectEmployees)
               .HasForeignKey(c => c.ProjectId);

            modelBuilder.Entity<ProjectEmployee>()
                .HasOne(c => c.Employee)
                .WithMany(c => c.ProjectEmployees)
                .HasForeignKey(c => c.EmployeeId);

            //Foreign key relation for ProjectTechnology model class

            modelBuilder.Entity<ProjectTechnology>()
               .HasOne(c => c.Project)
               .WithMany(c => c.Technology)
               .HasForeignKey(c => c.ProjectId);

            modelBuilder.Entity<ProjectTechnology>()
                .HasOne(c => c.Technologies)
                .WithMany(c => c.ProjectTechnology)
                .HasForeignKey(c => c.TechnologyId);

            //---------- POC table------------------------------------
            modelBuilder.Entity<POC>()
                                   .HasOne(pt => pt.Client)
                                   .WithMany(c => c.POC)
                                   .HasForeignKey(pt => pt.ClientId);

            modelBuilder.Entity<POCTeam>()
                                   .HasOne(pt => pt.Employee)
                                   .WithMany(c => c.POCTeam)
                                   .HasForeignKey(pt => pt.EmployeeId);

            modelBuilder.Entity<POCTechnology>()
                                   .HasOne(pt => pt.Technology)
                                   .WithMany(c => c.POCTechnology)
                                   .HasForeignKey(pt => pt.TechnologyId);

            //---------NewLeadEnquiry table---------------------------------
            modelBuilder.Entity<NewLeadEnquiry>()
               .HasOne(nle => nle.Employee)
               .WithMany(nle => nle.NewLeadEnquiry)
               .HasForeignKey(nle => nle.EmployeeID);

            modelBuilder.Entity<NewLeadEnquiry>()
                .HasOne(nle => nle.Employee)
                .WithMany(nle => nle.NewLeadEnquiry)
                .HasForeignKey(nle => nle.AssignTo);

            //-------------NewLeadEnquiryTechnology table---------------------------
            modelBuilder.Entity<NewLeadEnquiryTechnology>()
               .HasOne(t => t.Technology)
               .WithMany(t => t.NewLeadEnquiryTechnology)
               .HasForeignKey(nlt => nlt.TechnologyID);

            modelBuilder.Entity<NewLeadEnquiryTechnology>()
              .HasOne(t => t.NewLeadEnquiry)
              .WithMany(t => t.NewLeadEnquiryTechnology)
              .HasForeignKey(t => t.NewLeadEnquiryID);

            //--------------NewLeadEnquiryFollowup---------------------
            modelBuilder.Entity<NewLeadEnquiryFollowup>()
            .HasOne(f => f.NewLeadEnquiry)
            .WithMany(f => f.NewLeadEnquiryFollowup)
            .HasForeignKey(f => f.NewLeadEnquiryID);

            modelBuilder.Entity<NewLeadEnquiryFollowup>()
                .HasOne(f => f.Employee)
                .WithMany(f => f.NewLeadEnquiryFollowup)
                .HasForeignKey(f => f.AssignTo);

            //-------------- NewLeadEnquiryDocuments-----------------
            modelBuilder.Entity<NewLeadEnquiryDocuments>()
                .HasOne(f => f.NewLeadEnquiry)
                .WithMany(f => f.NewLeadEnquiryDocuments)
                .HasForeignKey(f => f.NewLeadEnquiryID);
            //----------SuccessStories table------------------------------------

            modelBuilder.Entity<SuccessStories>()
               .HasOne(c => c.Client)
               .WithMany(c => c.SuccessStories)
               .HasForeignKey(c => c.ClientID);

            modelBuilder.Entity<SuccessStories>()
             .HasOne(c => c.Project)
             .WithMany(c => c.SuccessStories)
             .HasForeignKey(c => c.ProjectId);





            modelBuilder.Entity<SuccessStories>()
                .HasOne(c => c.Employee)
                .WithMany(c => c.SuccessStories)
                .HasForeignKey(c => c.AssignTo);
        }

    }

}

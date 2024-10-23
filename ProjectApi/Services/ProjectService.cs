using DataServices.Data;
using DataServices.Models;
using DataServices.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace ProjectApi.Services
{
    public class ProjectService : IProjectService
    {
        private readonly DataBaseContext _context;
        private readonly IRepository<Project> _repository;

        public ProjectService(DataBaseContext context, IRepository<Project> repository)
        {
            _context = context;
            _repository = repository;
        }

        public async Task<IEnumerable<ProjectDTO>> GetAll()
        {
            var projects = await _context.TblProject
                .Include(c => c.Client)
                .Include(t => t.TechnicalProjectManagers)
                .Include(s => s.SalesContacts)
                .Include(p => p.PMOs)
                .ToListAsync();


            var projDtos = projects.Select(project => new ProjectDTO
            { 
                    Id = project.Id,
                    Client = project.Client?.Name,
                    ProjectName = project.ProjectName,
                    TechnicalProjectManager = project.TechnicalProjectManagers?.Name,
                    SalesContact = project.SalesContacts?.Name,
                    PMO = project.PMOs?.Name,
                    SOWSubmittedDate = project.SOWSubmittedDate,
                    SOWSignedDate = project.SOWSignedDate,
                    SOWValidTill = project.SOWValidTill,
                    SOWLastExtendedDate = project.SOWLastExtendedDate,
                    IsActive = project.IsActive,
                    CreatedBy = project.CreatedBy,
                    CreatedDate = project.CreatedDate,
                    UpdatedBy = project.UpdatedBy,
                    UpdatedDate = project.UpdatedDate
                }).ToList(); 
            
            return projDtos;

        }

        public async Task<ProjectDTO> Get(string id)
        {
            var project = await _context.TblProject
                .Include(c => c.Client)
                .Include(t => t.TechnicalProjectManagers)
                .Include(s => s.SalesContacts)
                .Include(p => p.PMOs)
                .FirstOrDefaultAsync(t => t.Id == id);
            if (project == null) return null;

            return new ProjectDTO
            {
                Id = project.Id,
                Client = project.Client?.Name,
                ProjectName = project.ProjectName,
                TechnicalProjectManager = project.TechnicalProjectManagers?.Name,
                SalesContact = project.SalesContacts?.Name,
                PMO = project.PMOs?.Name,
                SOWSubmittedDate = project.SOWSubmittedDate,
                SOWSignedDate = project.SOWSignedDate,
                SOWValidTill = project.SOWValidTill,
                SOWLastExtendedDate = project.SOWLastExtendedDate,
                IsActive = project.IsActive,
                CreatedBy = project.CreatedBy,
                CreatedDate = project.CreatedDate,
                UpdatedBy = project.UpdatedBy,
                UpdatedDate = project.UpdatedDate
            };
        }

        public async Task<ProjectDTO> Add(ProjectDTO projDto)
        {
            // Check if the project name already exists
            var existingProject = await _context.TblProject
                .FirstOrDefaultAsync(t => t.ProjectName == projDto.ProjectName);

            if (existingProject != null)
                throw new ArgumentException("A project with the same name already exists.");

            var project = new Project();

            var client = await _context.TblClient
               .FirstOrDefaultAsync(d => d.Name == projDto.Client);

            if (client == null)
                throw new KeyNotFoundException("Client not found");

            var technicalProjectManager = await _context.TblEmployee
               .FirstOrDefaultAsync(d => d.Name == projDto.TechnicalProjectManager);

            if (technicalProjectManager == null)
                throw new KeyNotFoundException("TechnicalProjectManagerId not found");

            var salesContact = await _context.TblEmployee
               .FirstOrDefaultAsync(d => d.Name == projDto.SalesContact);

            if (salesContact == null)
                throw new KeyNotFoundException("SalesContact not found");

            var pmo = await _context.TblEmployee
               .FirstOrDefaultAsync(d => d.Name == projDto.PMO);

            if (pmo == null)
                throw new KeyNotFoundException("PMO not found");

            project.ClientId = client.Id;
            project.ProjectName = projDto.ProjectName;
            project.TechnicalProjectManager = technicalProjectManager.Id;
            project.SalesContact = salesContact.Id;
            project.PMO = pmo.Id;
            project.SOWSubmittedDate = projDto.SOWSubmittedDate;
            project.SOWSignedDate = projDto.SOWSignedDate;
            project.SOWValidTill = projDto.SOWValidTill;
            project.SOWLastExtendedDate = projDto.SOWLastExtendedDate;
            project.IsActive = projDto.IsActive;
            project.CreatedBy = projDto.CreatedBy;
            project.CreatedDate = projDto.CreatedDate;
            project.UpdatedBy = projDto.UpdatedBy;
            project.UpdatedDate = projDto.UpdatedDate;
            

            _context.TblProject.Add(project);
            await _context.SaveChangesAsync();
            projDto.Id = project.Id;

            if (projDto.Technology != null && projDto.Technology.Any())
            {
                foreach (var technologyId in projDto.Technology)
                {
                    //var technology = await _context.TblTechnology.FirstOrDefaultAsync(t => t.Id == technologyId) ?? throw new KeyNotFoundException($"Technology with ID {technologyId} not found.");
                    var projectTechnology = new ProjectTechnology
                    {
                        ProjectId = project.Id,
                        TechnologyId = technologyId.ToString(),
                    };

                    await _context.TblProjectTechnology.AddAsync(projectTechnology);
                }
                await _context.SaveChangesAsync();
            }
            return projDto;
        }

        public async Task<ProjectDTO> Update(ProjectDTO projDto)
        {
            // Check if the project name already exists
            var existingProject = await _context.TblProject
                .FirstOrDefaultAsync(t => t.ProjectName == projDto.ProjectName);

            if (existingProject != null)
                throw new ArgumentException("A project with the same name already exists.");

            var project = await _context.TblProject.FindAsync(projDto.Id);

            if (project == null)
                throw new KeyNotFoundException("Project not found");

            var client = await _context.TblClient
              .FirstOrDefaultAsync(d => d.Name == projDto.Client);

            if (client == null)
                throw new KeyNotFoundException("Client not found");

            var technicalProjectManager = await _context.TblEmployee
                .FirstOrDefaultAsync(d => d.Name == projDto.TechnicalProjectManager);

            if (technicalProjectManager == null)
                throw new KeyNotFoundException("TechnicalProjectManagerId not found");

            var salesContact = await _context.TblEmployee
               .FirstOrDefaultAsync(d => d.Name == projDto.SalesContact);

            if (salesContact == null)
                throw new KeyNotFoundException("SalesContact not found");

            var pmo = await _context.TblEmployee
               .FirstOrDefaultAsync(d => d.Name == projDto.PMO);

            if (pmo == null)
                throw new KeyNotFoundException("PMO not found");

            project.ClientId = client.Id;
            project.ProjectName = projDto.ProjectName;
            project.TechnicalProjectManager = technicalProjectManager.Id;
            project.SalesContact = salesContact.Id;
            project.PMO = pmo.Id;
            project.SOWSubmittedDate = projDto.SOWSubmittedDate;
            project.SOWSignedDate = projDto.SOWSignedDate;
            project.SOWValidTill = projDto.SOWValidTill;
            project.SOWLastExtendedDate = projDto.SOWLastExtendedDate;
            project.IsActive = projDto.IsActive;
            project.CreatedBy = projDto.CreatedBy;
            project.CreatedDate = projDto.CreatedDate;
            project.UpdatedBy = projDto.UpdatedBy;
            project.UpdatedDate = projDto.UpdatedDate;

            _context.Entry(project).State = EntityState.Modified;

            if (projDto.Technology != null && projDto.Technology.Any())
            {
                // Remove old technologies
                var projectTechnologies = await _context.TblProjectTechnology
                    .Where(et => et.ProjectId == projDto.Id)
                    .ToListAsync();
                _context.TblProjectTechnology.RemoveRange(projectTechnologies);

                // Add updated technologies
                foreach (var technologyId in projDto.Technology)
                {
                    var technology = await _context.TblTechnology.FirstOrDefaultAsync(t => t.Id == technologyId);

                    if (technology == null)
                    {
                        throw new KeyNotFoundException($"Technology with ID {technologyId} not found.");
                    }

                    var projectTechnology = new ProjectTechnology
                    {
                        ProjectId = project.Id,
                        TechnologyId = technologyId.ToString(),
                    };

                    await _context.TblProjectTechnology.AddAsync(projectTechnology);
                }
            }

            await _context.SaveChangesAsync();

            return projDto;
        }

        public async Task<bool> Delete(string id)
        {
            var existingData = await _repository.Get(id);
            if (existingData == null)
            {
                throw new ArgumentException($"with ID {id} not found.");
            }

            existingData.IsActive = false; // Soft delete
            await _repository.Update(existingData); // Save changes
            return true;
        }
    }
}
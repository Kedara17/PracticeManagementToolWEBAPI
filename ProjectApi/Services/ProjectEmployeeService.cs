using DataServices.Data;
using DataServices.Models;
using DataServices.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ProjectApi.Services
{
    public class ProjectEmployeeService : IProjectEmployeeService
    {
        private readonly IRepository<ProjectEmployee> _repository;
        private readonly DataBaseContext _context;

        public ProjectEmployeeService(IRepository<ProjectEmployee> repository, DataBaseContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<IEnumerable<ProjectEmployeeDTO>> GetAll()
        {
            var projectEmployee = await _context.TblProjectEmployee
                .Include(p => p.Project)
                .Include(p => p.Employee)
                .ToListAsync();

            var projectEmployeeDtos = new List<ProjectEmployeeDTO>();

            foreach (var item in projectEmployee)
            {
                projectEmployeeDtos.Add(new ProjectEmployeeDTO()
                {
                    Id = item.Id,
                    Project = item.Project?.ProjectName,
                    Employee = item.Employee?.Name,
                    StartDate = item.StartDate,
                    EndDate = item.EndDate,
                    IsActive = item.IsActive,
                    CreatedBy = item.CreatedBy,
                    CreatedDate = item.CreatedDate,
                    UpdatedBy = item.UpdatedBy,
                    UpdatedDate = item.UpdatedDate
                });
            }
            return projectEmployeeDtos;
        }

        public async Task<ProjectEmployeeDTO> Get(string id)
        {
            var projectEmployee = await _context.TblProjectEmployee
                .Include(p => p.Project)
                .Include(p => p.Employee)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (projectEmployee == null) return null;

            return new ProjectEmployeeDTO
            {
                Id = projectEmployee.Id,
                Project = projectEmployee.Project?.ProjectName,
                Employee = projectEmployee.Employee?.Name,
                StartDate = projectEmployee.StartDate,
                EndDate = projectEmployee.EndDate,
                IsActive = projectEmployee.IsActive,
                CreatedBy = projectEmployee.CreatedBy,
                CreatedDate = projectEmployee.CreatedDate,
                UpdatedBy = projectEmployee.UpdatedBy,
                UpdatedDate = projectEmployee.UpdatedDate
            };
        }

        public async Task<ProjectEmployeeDTO> Add(ProjectEmployeeDTO _object)
        {
          /*  var project = await _context.TblProject
               .FirstOrDefaultAsync(d => d.ProjectName == _object.Project);

            if (project == null)
                throw new KeyNotFoundException("Project not found");

            var employee = await _context.TblEmployee
               .FirstOrDefaultAsync(d => d.Name == _object.Employee);

            if (employee == null)
                throw new KeyNotFoundException("Employee not found");*/

            var projectEmployee = new ProjectEmployee
            {
                ProjectId = _object.Project,
                EmployeeId = _object.Employee,
                StartDate = _object.StartDate,
                EndDate = _object.EndDate,
                IsActive = _object.IsActive,
                CreatedBy = _object.CreatedBy,
                CreatedDate = _object.CreatedDate,
                UpdatedBy = _object.UpdatedBy,
                UpdatedDate = _object.UpdatedDate
            };

            _context.TblProjectEmployee.Add(projectEmployee);
            await _context.SaveChangesAsync();

            _object.Id = projectEmployee.Id;
            return _object;
        }

        public async Task<ProjectEmployeeDTO> Update(ProjectEmployeeDTO _object)
        {
            var projectEmployee = await _context.TblProjectEmployee.FindAsync(_object.Id);

            if (projectEmployee == null)
                throw new KeyNotFoundException("ProjectEmployee not found");

           /* var project = await _context.TblProject
              .FirstOrDefaultAsync(d => d.ProjectName == _object.Project);

            if (project == null)
                throw new KeyNotFoundException("Project not found");

            var employee = await _context.TblEmployee
                .FirstOrDefaultAsync(d => d.Name == _object.Employee);

            if (employee == null)
                throw new KeyNotFoundException("employee not found");*/


            projectEmployee.ProjectId = _object.Project;
            projectEmployee.EmployeeId = _object.Employee;
            projectEmployee.IsActive = _object.IsActive;
            projectEmployee.CreatedBy = _object.CreatedBy;
            projectEmployee.CreatedDate = _object.CreatedDate;
            projectEmployee.UpdatedBy = _object.UpdatedBy;
            projectEmployee.UpdatedDate = _object.UpdatedDate;

            _context.Entry(projectEmployee).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return _object;
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

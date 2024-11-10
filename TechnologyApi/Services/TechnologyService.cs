using DataServices.Data;
using DataServices.Models;
using DataServices.Models.ViewModels;
using DataServices.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TechnologyApi.Services
{
    public class TechnologyService : ITechnologyService
    {
        private readonly DataBaseContext _context;
        private readonly IRepository<Technology> _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<TechnologyService> _logger;

        public TechnologyService(DataBaseContext context, IRepository<Technology> repository, IHttpContextAccessor httpContextAccessor, ILogger<TechnologyService> logger)
        {
            _context = context;
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<IEnumerable<Technology>> GetAll()
        {
            _logger.LogInformation("Fetching all Technologies");
            var technologies = await _context.TblTechnology.Include(t => t.Department).ToListAsync();
            return technologies;
        }

        public async Task<Technology> Get(string id)
        {
            var technology = await _context.TblTechnology
                .Include(t => t.Department)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (technology == null)
                return null;
            return technology;            
        }

        public async Task<TechnologyViewModel> Add(TechnologyViewModel technologyDto)
        {
            var technology = new Technology();
            var existingTechnology = await _context.TblTechnology
                .FirstOrDefaultAsync(t => t.Id == technologyDto.Id);

            if (existingTechnology != null)
                throw new ArgumentException("A technology with the same name already exists.");

            if (!string.IsNullOrWhiteSpace(technologyDto.Department))
            {
                var departmentExists = await _context.TblDepartment
                    .AnyAsync(d => d.Id == technologyDto.Department);
                if (!departmentExists)
                    throw new ArgumentException("The specified department does not exist.");

                technology.DepartmentId = technologyDto.Department;
            }
            else
            {
                technology.DepartmentId = null; // Allow null if department is not specified
            }

            var employeeName = _httpContextAccessor.HttpContext?.User?.FindFirst("EmployeeName")?.Value;

            technology.Name = technologyDto.Name;
            technology.IsActive = true;
            technology.CreatedBy = employeeName;
            technology.CreatedDate = DateTime.Now;

            _context.TblTechnology.Add(technology);
            await _context.SaveChangesAsync();

            technologyDto.Id = technology.Id;
            return technologyDto;
        }

        public async Task<bool> Update(string id,TechnologyViewModel technologyDto)
        {
            
            var userName = _httpContextAccessor.HttpContext?.User?.FindFirst("EmployeeName")?.Value;

            var technology = await _context.TblTechnology.FindAsync(id);

            if (technology == null)
                throw new KeyNotFoundException("Technology not found");

            
            if (!string.IsNullOrWhiteSpace(technologyDto.Department))
            {
                
                var departmentExists = await _context.TblDepartment
                    .AnyAsync(d => d.Id == technologyDto.Department);
                if (!departmentExists)
                    throw new ArgumentException("The specified department does not exist.");

                technology.DepartmentId = technologyDto.Department;
            }
            else
            {
                technology.DepartmentId = null; // Allow null if department is not specified
            }

            technology.Name = technologyDto.Name;

            if (technology.IsActive != technologyDto.IsActive)
            {
                technology.IsActive = technologyDto.IsActive;
                _logger.LogInformation("Department {Id} state changed to {IsActive}", technologyDto.Id, technologyDto.IsActive);
            }
            technology.UpdatedBy = userName;
            technology.UpdatedDate = DateTime.Now;

            _context.Entry(technology).State = EntityState.Modified;
            int rowsAffected = await _context.SaveChangesAsync();

            return rowsAffected > 0;
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
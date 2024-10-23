using DataServices.Data;
using DataServices.Models;
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

        public TechnologyService(DataBaseContext context, IRepository<Technology> repository, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<TechnologyDTO>> GetAll()
        {
            var technologies = await _context.TblTechnology.Include(t => t.Department).ToListAsync();
            var techDtos = new List<TechnologyDTO>();

            foreach (var tech in technologies)
            {
                techDtos.Add(new TechnologyDTO
                {
                    Id = tech.Id,
                    Name = tech.Name,
                    Department = tech.Department?.Name,
                    IsActive = tech.IsActive,
                    CreatedBy = tech.CreatedBy,
                    CreatedDate = tech.CreatedDate,
                    UpdatedBy = tech.UpdatedBy,
                    UpdatedDate = tech.UpdatedDate
                });
            }

            return techDtos;
        }

        public async Task<TechnologyDTO> Get(string id)
        {
            var technology = await _context.TblTechnology
                .Include(t => t.Department)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (technology == null)
                return null;

            return new TechnologyDTO
            {
                Id = technology.Id,
                Name = technology.Name,
                Department = technology.Department?.Name,
                IsActive = technology.IsActive,
                CreatedBy = technology.CreatedBy,
                CreatedDate = technology.CreatedDate,
                UpdatedBy = technology.UpdatedBy,
                UpdatedDate = technology.UpdatedDate
            };
        }

        public async Task<TechnologyDTO> Add(TechnologyDTO technologyDto)
        {
            var technology = new Technology();
            // Check if the technology name already exists
            var existingTechnology = await _context.TblTechnology
                .FirstOrDefaultAsync(t => t.Name == technologyDto.Name);

            if (existingTechnology != null)
                throw new ArgumentException("A technology with the same name already exists.");
            
            // Check if a department name is provided
            if (!string.IsNullOrWhiteSpace(technologyDto.Department))
            {
                // Look for the department in the database
                var department = await _context.TblDepartment
                    .FirstOrDefaultAsync(d => d.Name == technologyDto.Department);

                // If department is not found, throw an exception and provide valid department names
                if (department == null)
                {
                    throw new ArgumentException($"Invalid department name. Please enter a valid department name.");
                }

                technology.DepartmentId = department.Id;
            }
            else
            {
                // If no department is provided, allow null for the DepartmentId
                technology.DepartmentId = null;
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

        public async Task<TechnologyDTO> Update(TechnologyDTO technologyDto)
        {
            var userName = _httpContextAccessor.HttpContext?.User?.FindFirst("EmployeeName")?.Value;

            // Check if the technology name already exists
            var existingTechnology = await _context.TblTechnology
                .FirstOrDefaultAsync(t => t.Name == technologyDto.Name);
            if (existingTechnology != null)
                throw new ArgumentException("A technology with the same name already exists.");

            var technology = await _context.TblTechnology.FindAsync(technologyDto.Id);

            if (technology == null)
                throw new KeyNotFoundException("Technology not found");

            // Check if a department name is provided
            if (!string.IsNullOrWhiteSpace(technologyDto.Department))
            {
                // Look for the department in the database
                var department = await _context.TblDepartment
                    .FirstOrDefaultAsync(d => d.Name == technologyDto.Department);

                // If department is not found, throw an exception
                if (department == null)
                {
                    throw new ArgumentException("Invalid department name. Please enter a valid department name.");
                }

                technology.DepartmentId = department.Id; // Update the DepartmentId
            }
            else
            {
                // Allow DepartmentId to be null if no department name is provided
                technology.DepartmentId = null;
            }

            technology.Name = technologyDto.Name;
            technology.UpdatedBy = userName;
            technology.UpdatedDate = DateTime.Now;

            _context.Entry(technology).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return technologyDto;
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
        public async Task<TechnologyDTO> GetByName(string name)
        {
            return await _context.TblTechnology.FirstOrDefaultAsync(d => d.Name == name);
        }
    }
}
using DataServices.Data;
using DataServices.Models;
using DataServices.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DepartmentApi.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IRepository<Department> _repository;
        private readonly DataBaseContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DepartmentService(IRepository<Department> repository, DataBaseContext context, IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<DepartmentDTO>> GetAll()
        {
            var departments = await _context.TblDepartment.ToListAsync();

            var departmentDTOs = new List<DepartmentDTO>();

            foreach (var d in departments)
            {
                departmentDTOs.Add(new DepartmentDTO
                {
                    Id = d.Id,
                    Name = d.Name,
                    IsActive = d.IsActive,
                    CreatedBy = d.CreatedBy,
                    CreatedDate = d.CreatedDate,
                    UpdatedBy = d.UpdatedBy,
                    UpdatedDate = d.UpdatedDate
                });
            }

            return departmentDTOs;
        }

        public async Task<DepartmentDTO> Get(string id)
        {
            var department = await _context.TblDepartment
                .FirstOrDefaultAsync(t => t.Id == id);

            if (department == null)
                return null;

            return new DepartmentDTO
            {
                Id = department.Id,
                Name = department.Name,
                IsActive = department.IsActive,
                CreatedBy = department.CreatedBy,
                CreatedDate = department.CreatedDate,
                UpdatedBy = department.UpdatedBy,
                UpdatedDate = department.UpdatedDate
            };
        }

        public async Task<DepartmentDTO> Add(DepartmentDTO _object)
        {
            // Check if the Department name already exists
            var existingDepartment = await _context.TblDepartment
                .FirstOrDefaultAsync(t => t.Name == _object.Name);

            if (existingDepartment != null)
                throw new ArgumentException("A department with the same name already exists.");

            var employeeName = _httpContextAccessor.HttpContext?.User?.FindFirst("EmployeeName")?.Value;
            var department = new Department
            {
                Name = _object.Name,
                IsActive = true,
                CreatedBy = employeeName,
                CreatedDate = DateTime.Now
            };

            _context.TblDepartment.Add(department);
            await _context.SaveChangesAsync();

            _object.Id = department.Id;
            return _object;
        }

        public async Task<DepartmentDTO> Update(DepartmentDTO _object)
        {

            var userName = _httpContextAccessor.HttpContext?.User?.FindFirst("EmployeeName")?.Value;
            var department = await _context.TblDepartment.FindAsync(_object.Id);

            if (department == null)
                throw new KeyNotFoundException("Department not found");

            department.Name = _object.Name;
            department.UpdatedBy = userName;
            department.UpdatedDate = DateTime.Now;

            _context.Entry(department).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return _object;
        }

        public async Task<bool> Delete(string id)
        {
            // Check if the technology exists
            var existingData = await _repository.Get(id);
            if (existingData == null)
            {
                throw new ArgumentException($"with ID {id} not found.");
            }

            // Call repository to delete the Department
            existingData.IsActive = false; // Soft delete
            await _repository.Update(existingData); // Save changes
            return true;
        }

        public async Task<DepartmentDTO> GetByName(string name)
        {
            return await _context.TblDepartment.FirstOrDefaultAsync(d => d.Name == name);
        }

    }
}





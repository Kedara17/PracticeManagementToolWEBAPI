using DataServices.Data;
using DataServices.Models;
using DataServices.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DesignationApi.Services
{
    public class DesignationService : IDesignationService
    {
        private readonly IRepository<Designation> _repository;
        private readonly DataBaseContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DesignationService(IRepository<Designation> repository, DataBaseContext context, IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<DesignationDTO>> GetAll()
        {
            var designations = await _context.TblDesignation.ToListAsync();

            var designationDTOs = new List<DesignationDTO>();

            foreach (var d in designations)
            {
                designationDTOs.Add(new DesignationDTO
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

            return designationDTOs; 
        }
        public async Task<DesignationDTO> Get(string id)
        {
            var designation = await _context.TblDesignation
                .FirstOrDefaultAsync(t => t.Id == id);

            if (designation == null)
                return null;

            return new DesignationDTO
            {
                Id = designation.Id,
                Name = designation.Name,
                IsActive = designation.IsActive,
                CreatedBy = designation.CreatedBy,
                CreatedDate = designation.CreatedDate,
                UpdatedBy = designation.UpdatedBy,
                UpdatedDate = designation.UpdatedDate
            };
        }

        public async Task<DesignationDTO> Add(DesignationDTO _object)
        {
            // Check if the Designation name already exists
            var existingDesignation = await _context.TblDesignation
                .FirstOrDefaultAsync(t => t.Name == _object.Name);

            if (existingDesignation != null)
                throw new ArgumentException("A designation with the same name already exists.");

            var employeeName = _httpContextAccessor.HttpContext?.User?.FindFirst("EmployeeName")?.Value;
            var designation = new Designation
            {
                Name = _object.Name,
                IsActive = true,
                CreatedBy = employeeName,
                CreatedDate = DateTime.Now
            };

            _context.TblDesignation.Add(designation);
            await _context.SaveChangesAsync();

            _object.Id = designation.Id;
            return _object;
        }

        public async Task<DesignationDTO> Update(DesignationDTO _object)
        {
            var employeeName = _httpContextAccessor.HttpContext?.User?.FindFirst("EmployeeName")?.Value;
            var designation = await _context.TblDesignation.FindAsync(_object.Id);

            if (designation == null)
                throw new KeyNotFoundException("Designation not found");

            designation.Name = _object.Name;
            designation.UpdatedBy = employeeName;
            designation.UpdatedDate = DateTime.Now;

            _context.Entry(designation).State = EntityState.Modified;
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
            //return await _repository.Delete(id);
            existingData.IsActive = false; // Soft delete
            await _repository.Update(existingData); // Save changes
            return true;
        }
        public async Task<DesignationDTO> GetByName(string name)
        {
            return await _context.TblDesignation.FirstOrDefaultAsync(d => d.Name == name);
        }
    }
}

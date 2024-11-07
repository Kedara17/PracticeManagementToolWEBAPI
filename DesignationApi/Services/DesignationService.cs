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
        private readonly ILogger<DesignationService> _logger;

        public DesignationService(IRepository<Designation> repository, DataBaseContext context, IHttpContextAccessor httpContextAccessor, ILogger<DesignationService> logger)
        {
            _repository = repository;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<IEnumerable<DesignationDTO>> GetAll()
        {
            _logger.LogInformation("Fetching all designations");
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
            _logger.LogInformation("Fetching designation with id: {Id}", id);
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
            _logger.LogInformation("Adding a new designation with name: {Name}", _object.Name);
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
            _logger.LogInformation("Updating designation with id: {Id}", _object.Id);
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
            var designation = await _context.TblDesignation.FindAsync(id);
            if (designation == null)
            {
                throw new KeyNotFoundException("Designation not found");
            }

            // Toggle the IsActive status
            designation.IsActive = !designation.IsActive;

            // Save the changes
            _context.Entry(designation).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return designation.IsActive;
        }

        public async Task<DesignationDTO> GetByName(string name)
        {
            _logger.LogInformation("Fetching designation with name: {Name}", name);
            return await _context.TblDesignation.FirstOrDefaultAsync(d => d.Name == name);
        }
    }
}

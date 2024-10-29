//Services




using Microsoft.EntityFrameworkCore;
using DataServices.Data;
using DataServices.Models;
using DataServices.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CertificationsApi.Services
{
    public class CertificationsService : ICertificationsService
    {
        private readonly IRepository<Certifications> _repository;
        private readonly DataBaseContext _context;

        public CertificationsService(IRepository<Certifications> repository, DataBaseContext context)
        {
            _repository = repository;
            _context = context;
        }

        // Get all Certifications
        public async Task<IEnumerable<CertificationsDTO>> GetAll()
        {
            var certifications = await _context.TblCertifications.Include(t => t.Employee).ToListAsync();
            var certificationsDTO = new List<CertificationsDTO>();

            foreach (var c in certifications)
            {
                certificationsDTO.Add(new CertificationsDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    EmployeeId = c.Employee?.Name, // Use EmployeeId
                    ExamDate = c.ExamDate,
                    ValidTill = c.ValidTill,
                    Status = c.Status,
                    Comments = c.Comments,
                    IsActive = c.IsActive,
                    CreatedBy = c.CreatedBy,
                    CreatedDate = c.CreatedDate,
                    UpdatedBy = c.UpdatedBy,
                    UpdatedDate = c.UpdatedDate
                });
            }

            return certificationsDTO;
        }

        // Get Certification by ID
        public async Task<CertificationsDTO> Get(string id)
        {
            var certification = await _context.TblCertifications
                .Include(t => t.Employee) // Include Employee to get Employee details
                .FirstOrDefaultAsync(t => t.Id == id);

            if (certification == null)
                return null;

            return new CertificationsDTO
            {
                Id = certification.Id,
                Name = certification.Name,
                EmployeeId = certification.Employee?.Name, // Use EmployeeId
                ExamDate = certification.ExamDate,
                ValidTill = certification.ValidTill,
                Status = certification.Status,
                Comments = certification.Comments,
                IsActive = certification.IsActive,
                CreatedBy = certification.CreatedBy,
                CreatedDate = certification.CreatedDate,
                UpdatedBy = certification.UpdatedBy,
                UpdatedDate = certification.UpdatedDate
            };
        }

        // Add new Certification
        public async Task<CertificationsDTO> Add(CertificationsDTO certificationsDto)
        {
            var employee = await _context.TblEmployee.FirstOrDefaultAsync(e => e.Id == certificationsDto.EmployeeId);

            if (employee == null)
                throw new KeyNotFoundException("Employee not found");

            var certification = new Certifications
            {
                Name = certificationsDto.Name,
                EmployeeId = employee.Id,  // Use EmployeeId
                ExamDate = certificationsDto.ExamDate,
                ValidTill = certificationsDto.ValidTill,
                Status = certificationsDto.Status,
                Comments = certificationsDto.Comments,
                IsActive = certificationsDto.IsActive,
                CreatedBy = certificationsDto.CreatedBy,
                CreatedDate = certificationsDto.CreatedDate,
                UpdatedBy = certificationsDto.UpdatedBy,
                UpdatedDate = certificationsDto.UpdatedDate
            };

            _context.TblCertifications.Add(certification);
            await _context.SaveChangesAsync();

            certificationsDto.Id = certification.Id;
            return certificationsDto;
        }

        // Update Certification
        public async Task<CertificationsDTO> Update(CertificationsDTO certificationDto)
        {
            var certification = await _context.TblCertifications.FindAsync(certificationDto.Id);

            if (certification == null)
                throw new KeyNotFoundException("Certification not found");

            var employee = await _context.TblEmployee.FirstOrDefaultAsync(e => e.Id == certificationDto.EmployeeId);

            if (employee == null)
                throw new KeyNotFoundException("Employee not found");

            certification.Name = certificationDto.Name;
            certification.EmployeeId = employee.Id;  // Use EmployeeId
            certification.ExamDate = certificationDto.ExamDate;
            certification.ValidTill = certificationDto.ValidTill;
            certification.Status = certificationDto.Status;
            certification.Comments = certificationDto.Comments;
            certification.IsActive = certificationDto.IsActive;
            certification.UpdatedBy = certificationDto.UpdatedBy;
            certification.UpdatedDate = certificationDto.UpdatedDate;

            _context.Entry(certification).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return certificationDto;
        }

        // Soft delete
        public async Task<bool> Delete(string id)
        {
            // Retrieve the certification entity by ID
            var certification = await _context.TblCertifications.FindAsync(id);

            if (certification == null)
            {
                // Optional: Either throw an exception or return false based on your use case
                throw new KeyNotFoundException($"Certification with ID {id} not found.");
                // OR return false if you prefer not to throw an exception
                // return false;
            }

            // Soft delete by marking the entity as inactive
            certification.IsActive = false;

            // Save the changes in the context
            await _context.SaveChangesAsync();

            // Return success
            return true;
        }

    }
}

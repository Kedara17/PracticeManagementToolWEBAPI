using DataServices.Data;
using DataServices.Models;
using DataServices.Repositories;
using Microsoft.EntityFrameworkCore;

namespace SOWApi.Services
{
    public class SOWStatusService : ISOWStatusService
    {
        private readonly IRepository<SOWStatus> _repository;
        private readonly DataBaseContext _context;

        public SOWStatusService(IRepository<SOWStatus> repository, DataBaseContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<IEnumerable<SOWStatusDTO>> GetAll()
        {
            var sowStatuses = await _context.TblSOWStatus.ToListAsync();

            var sowStatusDTOs = new List<SOWStatusDTO>();

            foreach (var d in sowStatuses)
            {
                sowStatusDTOs.Add(new SOWStatusDTO
                {
                    Id = d.Id,
                    Status = d.Status,
                    IsActive = d.IsActive,
                    CreatedBy = d.CreatedBy,
                    CreatedDate = d.CreatedDate,
                    UpdatedBy = d.UpdatedBy,
                    UpdatedDate = d.UpdatedDate
                });
            }

            return sowStatusDTOs;
        }

        public async Task<SOWStatusDTO> Get(string id)
        {
            var sowStatuses = await _context.TblSOWStatus
                .FirstOrDefaultAsync(t => t.Id == id);

            if (sowStatuses == null)
                return null;

            return new SOWStatusDTO
            {
                Id = sowStatuses.Id,
                Status = sowStatuses.Status,
                IsActive = sowStatuses.IsActive,
                CreatedBy = sowStatuses.CreatedBy,
                CreatedDate = sowStatuses.CreatedDate,
                UpdatedBy = sowStatuses.UpdatedBy,
                UpdatedDate = sowStatuses.UpdatedDate
            };
        }

        public async Task<SOWStatusDTO> Add(SOWStatusDTO sowstatusDto)
        {
            // Check if the Status already exists
            var existingStatus = await _context.TblSOWStatus
                .FirstOrDefaultAsync(t => t.Status == sowstatusDto.Status);

            if (existingStatus != null)
                throw new ArgumentException("A Status with the same name already exists.");

            var sowStatus = new SOWStatus
            {
                Status = sowstatusDto.Status,
                IsActive = true,
                CreatedBy = "SYSTEM",
                CreatedDate = DateTime.Now,
                UpdatedBy = sowstatusDto.UpdatedBy,
                UpdatedDate = sowstatusDto.UpdatedDate
            };

            _context.TblSOWStatus.Add(sowStatus);
            await _context.SaveChangesAsync();

            sowstatusDto.Id = sowStatus.Id;
            return sowstatusDto;
        }

        public async Task<SOWStatusDTO> Update(SOWStatusDTO sowstatusDto)
        {
            // Check if the Status already exists
            var existingStatus = await _context.TblSOWStatus
                .FirstOrDefaultAsync(t => t.Status == sowstatusDto.Status);

            if (existingStatus != null)
                throw new ArgumentException("A Status with the same name already exists.");

            var sowstatus = await _context.TblSOWStatus.FindAsync(sowstatusDto.Id);

            if (sowstatus == null)
                throw new KeyNotFoundException("SOWStatus not found");

            sowstatus.Status = sowstatusDto.Status;
            sowstatus.IsActive = sowstatusDto.IsActive;
            sowstatus.UpdatedBy = sowstatusDto.UpdatedBy;
            sowstatus.UpdatedDate = sowstatusDto.UpdatedDate;

            _context.Entry(sowstatus).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return sowstatusDto;

        }

        public async Task<bool> Delete(string id)
        {
            var existingsowstatus = await _repository.Get(id);
            if (existingsowstatus == null)
            {
                throw new ArgumentException($"SowStatus with ID {id} not found.");
            }
            existingsowstatus.IsActive = false; // Soft delete
            await _repository.Update(existingsowstatus); // Save changes
            return true;
        }
    }
}

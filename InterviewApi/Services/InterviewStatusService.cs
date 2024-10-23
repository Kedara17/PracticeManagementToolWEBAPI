using DataServices.Data;
using DataServices.Models;
using DataServices.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;

namespace InterviewApi.Services
{
    public class InterviewStatusService : IInterviewStatusService
    {
        private readonly IRepository<InterviewStatus> _repository;
        private readonly DataBaseContext _context;

        public InterviewStatusService(IRepository<InterviewStatus> repository, DataBaseContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<IEnumerable<InterviewStatusDTO>> GetAll()
        {
            var interviews = await _context.TblInterviewStatus.ToListAsync();
            if (interviews == null) return null;

            var interviewsDto = new List<InterviewStatusDTO>();
            foreach (var interview in interviews)
            {
                interviewsDto.Add(new InterviewStatusDTO()
                {
                    Id = interview.Id,
                    Status = interview.Status,
                    IsActive = interview.IsActive,
                    CreatedBy = interview.CreatedBy,
                    CreatedDate = interview.CreatedDate,
                    UpdatedBy = interview.UpdatedBy,
                    UpdatedDate = interview.UpdatedDate
                });
            }
            return interviewsDto;
        }

        public async Task<InterviewStatusDTO> Get(string id)
        {
            var interviews = await _context.TblInterviewStatus.FirstOrDefaultAsync(t => t.Id == id); ;
            if (interviews == null) return null;

            return new InterviewStatusDTO
            {
                Id = interviews.Id,
                Status = interviews.Status,
                IsActive = interviews.IsActive,
                CreatedBy = interviews.CreatedBy,
                CreatedDate = interviews.CreatedDate,
                UpdatedBy = interviews.UpdatedBy,
                UpdatedDate = interviews.UpdatedDate
            };
        }

        public async Task<InterviewStatusDTO> Add(InterviewStatusDTO _object)
        {
            // Check if the interview Status already exists
            var existingStatus = await _context.TblInterviewStatus
                .FirstOrDefaultAsync(t => t.Status == _object.Status);

            if (existingStatus != null)
                throw new ArgumentException("A InterviewStatus with the same name already exists.");

            var interviewStatus = new InterviewStatus
            {
                Status = _object.Status,
                IsActive = _object.IsActive,
                CreatedBy = _object.CreatedBy,
                CreatedDate = _object.CreatedDate,
                UpdatedBy = _object.UpdatedBy,
                UpdatedDate = _object.UpdatedDate
            };

            _context.TblInterviewStatus.Add(interviewStatus);
            await _context.SaveChangesAsync();

            _object.Id = interviewStatus.Id;
            return _object;
        }

        public async Task<InterviewStatusDTO> Update(InterviewStatusDTO _object)
        {
            // Check if the interview Status already exists
            var existingStatus = await _context.TblInterviewStatus
                .FirstOrDefaultAsync(t => t.Status == _object.Status);

            if (existingStatus != null)
                throw new ArgumentException("A InterviewStatus with the same name already exists.");

            var interviewStatus = await _context.TblInterviewStatus.FindAsync(_object.Id);

            if (interviewStatus == null)
                throw new KeyNotFoundException("InterviewStatus not found");

            interviewStatus.Status = _object.Status;
            interviewStatus.IsActive = _object.IsActive;
            interviewStatus.CreatedBy = _object.CreatedBy;
            interviewStatus.CreatedDate = _object.CreatedDate;
            interviewStatus.UpdatedBy = _object.UpdatedBy;
            interviewStatus.UpdatedDate = _object.UpdatedDate;

            _context.Entry(interviewStatus).State = EntityState.Modified;
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
using DataServices.Data;
using DataServices.Models;
using DataServices.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace InterviewApi.Services
{
    public class InterviewService : IInterviewService
    {
        private readonly IRepository<Interviews> _repository;
        private readonly ILogger<InterviewService> _logger;
        private readonly DataBaseContext _context;

        public InterviewService(IRepository<Interviews> repository, DataBaseContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<IEnumerable<InterviewsDTO>> GetAll()
        {
            var interviews = await _context.TblInterviews
                 .Include(c => c.SOWRequirement)
                 .Include(t => t.Status)
                 .Include(s => s.Employee)
                 .ToListAsync();

            var interviewsDto = new List<InterviewsDTO>();
            foreach (var item in interviews)
            {
                interviewsDto.Add(new InterviewsDTO
                {
                    Id = item.Id,
                    SOWRequirement = item.SOWRequirement?.TeamSize.ToString(),
                    Name = item.Name,
                    InterviewDate = item.InterviewDate,
                    YearsOfExperience = item.YearsOfExperience,
                    Status = item.Status?.Status,
                    On_Boarding = item.On_Boarding,
                    Recruiter = item.Employee?.Name,
                    IsActive = item.IsActive,
                    CreatedBy = item.CreatedBy,
                    CreatedDate = item.CreatedDate,
                    UpdatedBy = item.UpdatedBy,
                    UpdatedDate = item.UpdatedDate
                });
            }
            return interviewsDto;
        }

        public async Task<InterviewsDTO> Get(string id)
        {
            var interviews = await _context.TblInterviews
                 .Include(c => c.SOWRequirement)
                 .Include(t => t.Status)
                 .Include(s => s.Employee)
                 .FirstOrDefaultAsync(t => t.Id == id);
            if (interviews == null) return null;

            return new InterviewsDTO
            {
                Id = interviews.Id,
                SOWRequirement = interviews.SOWRequirement?.TeamSize.ToString(),
                Name = interviews.Name,
                InterviewDate = interviews.InterviewDate,
                YearsOfExperience = interviews.YearsOfExperience,
                Status = interviews.Status?.Status,
                On_Boarding = interviews.On_Boarding,
                Recruiter = interviews.Employee?.Name,
                IsActive = interviews.IsActive,
                CreatedBy = interviews.CreatedBy,
                CreatedDate = interviews.CreatedDate,
                UpdatedBy = interviews.UpdatedBy,
                UpdatedDate = interviews.UpdatedDate
            };
        }

        public async Task<InterviewsDTO> Add(InterviewsDTO _object)
        {
           /* var sowRequirement = await _context.TblSOWRequirement
               .FirstOrDefaultAsync(d => d.TeamSize.ToString() == _object.SOWRequirement);

            if (sowRequirement == null)
                throw new KeyNotFoundException("SOWRequirement not found");

            var status = await _context.TblInterviewStatus
               .FirstOrDefaultAsync(d => d.Status == _object.Status);

            if (status == null)
                throw new KeyNotFoundException("status not found");

            var recruiter = await _context.TblEmployee
               .FirstOrDefaultAsync(d => d.Name == _object.Recruiter);

            if (recruiter == null)
                throw new KeyNotFoundException("SalesContact not found");*/


            var interviews = new Interviews
            {
                SOWRequirementId = _object.SOWRequirement,
                Name = _object.Name,
                InterviewDate = _object.InterviewDate,
                YearsOfExperience = _object.YearsOfExperience,
                StatusId = _object.Status,
                On_Boarding = _object.On_Boarding,
                Recruiter = _object.Recruiter,
                IsActive = _object.IsActive,
                CreatedBy = _object.CreatedBy,
                CreatedDate = _object.CreatedDate,
                UpdatedBy = _object.UpdatedBy,
                UpdatedDate = _object.UpdatedDate
            };

            _context.TblInterviews.Add(interviews);
            await _context.SaveChangesAsync();

            _object.Id = interviews.Id;
            return _object;
        }

        public async Task<InterviewsDTO> Update(InterviewsDTO _object)
        {
            var interview = await _context.TblInterviews.FindAsync(_object.Id);

            if (interview == null)
                throw new KeyNotFoundException("Interview not found");

           /* var sowRequirement = await _context.TblSOWRequirement
               .FirstOrDefaultAsync(d => d.TeamSize.ToString() == _object.SOWRequirement);

            if (sowRequirement == null)
                throw new KeyNotFoundException("SOWRequirement not found");

            var status = await _context.TblInterviewStatus
               .FirstOrDefaultAsync(d => d.Status == _object.Status);

            if (status == null)
                throw new KeyNotFoundException("status not found");

            var recruiter = await _context.TblEmployee
               .FirstOrDefaultAsync(d => d.Name == _object.Recruiter);

            if (recruiter == null)
                throw new KeyNotFoundException("SalesContact not found");*/

            interview.SOWRequirementId = _object.SOWRequirement;
            interview.Name = _object.Name;
            interview.InterviewDate = _object.InterviewDate;
            interview.YearsOfExperience = _object.YearsOfExperience;
            interview.StatusId = _object.Status;
            interview.On_Boarding = _object.On_Boarding;
            interview.Recruiter = _object.Recruiter;
            interview.IsActive = _object.IsActive;
            interview.CreatedBy = _object.CreatedBy;
            interview.CreatedDate = _object.CreatedDate;
            interview.UpdatedBy = _object.UpdatedBy;
            interview.UpdatedDate = _object.UpdatedDate;

            _context.Entry(interview).State = EntityState.Modified;
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
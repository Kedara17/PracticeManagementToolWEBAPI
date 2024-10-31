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


            var interviews = new Interviews();

            if (!string.IsNullOrWhiteSpace(_object.SOWRequirement))
            {
                var sowRequirementExists = await _context.TblSOWRequirement
                    .AnyAsync(d => d.Id == _object.SOWRequirement);
                if (!sowRequirementExists)
                    throw new ArgumentException("The specified sowRequirement does not exist.");

                interviews.SOWRequirementId = _object.SOWRequirement;
            }
            else
            {
                interviews.SOWRequirementId = null;
            }
            //_________________________________________________
            if (!string.IsNullOrWhiteSpace(_object.Status))
            {
                var statusExists = await _context.TblInterviewStatus
                    .AnyAsync(d => d.Id == _object.Status);
                if (!statusExists)
                    throw new ArgumentException("The specified status does not exist.");

                interviews.StatusId = _object.Status;
            }
            else
            {
                interviews.StatusId = null;
            }
            //_________________________________________________
            if (!string.IsNullOrWhiteSpace(_object.Recruiter))
            {
                var recruiterExists = await _context.TblEmployee
                    .AnyAsync(d => d.Id == _object.Recruiter);
                if (!recruiterExists)
                    throw new ArgumentException("The specified recruiter does not exist.");

                interviews.Recruiter = _object.Recruiter;
            }
            else
            {
                interviews.Recruiter = null;
            }
            //---------------------------------------------------------
            if (_object.InterviewDate.HasValue)
            {
                interviews.InterviewDate = _object.InterviewDate;
            }
            else
            {
                interviews.InterviewDate = null;
            }
            //__________________________________________________________
            if (_object.YearsOfExperience.HasValue && _object.YearsOfExperience > 0)
            {
                interviews.YearsOfExperience = _object.YearsOfExperience.Value;
            }
            else
            {
                interviews.YearsOfExperience = null;
            }
            //_____________________________________________________________
            if (_object.On_Boarding.HasValue)
            {
                interviews.On_Boarding = _object.On_Boarding;
            }
            else
            {
                interviews.On_Boarding = null;
            }

            /*interviews.SOWRequirementId = _object.SOWRequirement;*/
            interviews.Name = _object.Name;
            interviews.InterviewDate = _object.InterviewDate;
            interviews.YearsOfExperience = _object.YearsOfExperience;
            /*interviews.StatusId = _object.Status;*/
            interviews.On_Boarding = _object.On_Boarding;
            /*interviews.Recruiter = _object.Recruiter;*/
            interviews.IsActive = _object.IsActive;
            interviews.CreatedBy = _object.CreatedBy;
            interviews.CreatedDate = _object.CreatedDate;
            interviews.UpdatedBy = _object.UpdatedBy;
            interviews.UpdatedDate = _object.UpdatedDate;           

            _context.TblInterviews.Add(interviews);
            await _context.SaveChangesAsync();

            _object.Id = interviews.Id;
            return _object;
        }

        public async Task<InterviewsDTO> Update(InterviewsDTO _object)
        {
            var interviews = new Interviews();
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

            if (!string.IsNullOrWhiteSpace(_object.SOWRequirement))
            {
                var sowRequirementExists = await _context.TblSOWRequirement
                    .AnyAsync(d => d.Id == _object.SOWRequirement);
                if (!sowRequirementExists)
                    throw new ArgumentException("The specified sowRequirement does not exist.");

                interviews.SOWRequirementId = _object.SOWRequirement;
            }
            else
            {
                interviews.SOWRequirementId = null;
            }
            //_________________________________________________
            if (!string.IsNullOrWhiteSpace(_object.Status))
            {
                var statusExists = await _context.TblInterviewStatus
                    .AnyAsync(d => d.Id == _object.Status);
                if (!statusExists)
                    throw new ArgumentException("The specified status does not exist.");

                interviews.StatusId = _object.Status;
            }
            else
            {
                interviews.StatusId = null;
            }
            //_________________________________________________
            if (!string.IsNullOrWhiteSpace(_object.Recruiter))
            {
                var recruiterExists = await _context.TblEmployee
                    .AnyAsync(d => d.Id == _object.Recruiter);
                if (!recruiterExists)
                    throw new ArgumentException("The specified recruiter does not exist.");

                interviews.Recruiter = _object.Recruiter;
            }
            else
            {
                interviews.Recruiter = null;
            }
            //---------------------------------------------------------
            if (_object.InterviewDate.HasValue)
            {
                interviews.InterviewDate = _object.InterviewDate;
            }
            else
            {
                interviews.InterviewDate = null;
            }
            //__________________________________________________________
            if (_object.YearsOfExperience.HasValue && _object.YearsOfExperience > 0)
            {
                interviews.YearsOfExperience = _object.YearsOfExperience.Value;
            }
            else
            {
                interviews.YearsOfExperience = null;
            }
            //_____________________________________________________________
            if (_object.On_Boarding.HasValue)
            {
                interviews.On_Boarding = _object.On_Boarding;
            }
            else
            {
                interviews.On_Boarding = null;
            }

            /*interviews.SOWRequirementId = _object.SOWRequirement;*/
            interviews.Name = _object.Name;
            interviews.InterviewDate = _object.InterviewDate;
            interviews.YearsOfExperience = _object.YearsOfExperience;
            /*interviews.StatusId = _object.Status;*/
            interviews.On_Boarding = _object.On_Boarding;
            /*interviews.Recruiter = _object.Recruiter;*/
            interviews.IsActive = _object.IsActive;
            interviews.CreatedBy = _object.CreatedBy;
            interviews.CreatedDate = _object.CreatedDate;
            interviews.UpdatedBy = _object.UpdatedBy;
            interviews.UpdatedDate = _object.UpdatedDate;

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
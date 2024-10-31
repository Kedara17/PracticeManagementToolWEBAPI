using DataServices.Data;
using DataServices.Models;
using DataServices.Repositories;
using Microsoft.EntityFrameworkCore;

namespace NewLeadApi.Services
{
    public class NewLeadEnquiryFollowupService : INewLeadEnquiryFollowupService
    {
        private readonly IRepository<NewLeadEnquiryFollowup> _repository;
        private readonly DataBaseContext _context;

        public NewLeadEnquiryFollowupService(IRepository<NewLeadEnquiryFollowup> repository, DataBaseContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<IEnumerable<NewLeadEnquiryFollowupDTO>> GetAll()
        {
            var followups = await _context.TblNewLeadEnquireFollowup
                .Include(ne => ne.NewLeadEnquiry)
                .Include(ne => ne.Employee)
                .ToListAsync();
            var dto = followups.Select(f => new NewLeadEnquiryFollowupDTO
            {
                Id = f.Id,
                NewLeadEnquiryID = f.NewLeadEnquiryID.ToString(),
                AssignTo = f.AssignTo.ToString(),
                NewFollowupDate = f.NewFollowupDate,
                Comments = f.Comments,
                IsActive = f.IsActive,
                CreatedBy = f.CreatedBy,
                CreatedDate = f.CreatedDate,
                UpdatedBy = f.UpdatedBy,
                UpdatedDate = f.UpdatedDate
            }).ToList();
            return dto;
        }

        public async Task<NewLeadEnquiryFollowupDTO> Get(string id)
        {
            var followup = await _context.TblNewLeadEnquireFollowup
                .Include(ne => ne.NewLeadEnquiry)
                .Include(ne => ne.Employee)
                .FirstOrDefaultAsync(ne => ne.Id == id);

            if (followup == null) return null;

            return new NewLeadEnquiryFollowupDTO
            {
                Id = followup.Id,
                NewLeadEnquiryID = followup.NewLeadEnquiryID.ToString(),
                AssignTo = followup.AssignTo.ToString(),
                NewFollowupDate = followup.NewFollowupDate,
                Comments = followup.Comments,
                IsActive = followup.IsActive,
                CreatedBy = followup.CreatedBy,
                CreatedDate = followup.CreatedDate,
                UpdatedBy = followup.UpdatedBy,
                UpdatedDate = followup.UpdatedDate
            };
        }

        public async Task<NewLeadEnquiryFollowupDTO> Add(NewLeadEnquiryFollowupDTO dto)
        {
            var newFollowup = new NewLeadEnquiryFollowup();

            var newLeadEnquiryID = await _context.TblNewLeadEnquiry
             .FirstOrDefaultAsync(d => d.Id == dto.NewLeadEnquiryID);
            if (newLeadEnquiryID == null)
                throw new KeyNotFoundException("NewLeadEnquiryID not found");

            var assignTo = await _context.TblEmployee
               .FirstOrDefaultAsync(d => d.Id == dto.AssignTo);
            if (assignTo == null)
                throw new KeyNotFoundException("AssignTo not found");

            newFollowup.Id = dto.Id;
            newFollowup.NewLeadEnquiryID = dto.NewLeadEnquiryID;
            newFollowup.AssignTo = dto.AssignTo;
            newFollowup.NewFollowupDate = dto.NewFollowupDate;
            newFollowup.Comments = dto.Comments;
            newFollowup.IsActive = dto.IsActive;
            newFollowup.CreatedBy = dto.CreatedBy;
            newFollowup.CreatedDate = dto.CreatedDate;
            newFollowup.UpdatedBy = dto.UpdatedBy;
            newFollowup.UpdatedDate = dto.UpdatedDate;

            dto.Id = newFollowup.Id;
            await _context.TblNewLeadEnquireFollowup.AddAsync(newFollowup);
            await _context.SaveChangesAsync();
            return dto;
        }
        
        public async Task<NewLeadEnquiryFollowupDTO> Update(NewLeadEnquiryFollowupDTO dto)
        {
            // Check if the follow-up ID exists
            var followup = await _context.TblNewLeadEnquireFollowup.FindAsync(dto.Id);
            if (followup == null)
                throw new KeyNotFoundException("Followup not found.");

            // Validate NewLeadEnquiryID exists
            var newLeadEnquiry = await _context.TblNewLeadEnquiry
                .FindAsync(dto.NewLeadEnquiryID);
            if (newLeadEnquiry == null)
                throw new KeyNotFoundException("NewLeadEnquiryID not found.");

            // Validate AssignTo exists in the Employee table
            var assignTo = await _context.TblEmployee
                .FindAsync(dto.AssignTo);
            if (assignTo == null)
                throw new KeyNotFoundException("AssignTo not found.");

            // Update followup fields
            followup.Id = dto.Id;
            followup.NewLeadEnquiryID = dto.NewLeadEnquiryID;
            followup.AssignTo = dto.AssignTo;
            followup.NewFollowupDate = dto.NewFollowupDate;
            followup.Comments = dto.Comments;
            followup.IsActive = dto.IsActive;
            followup.CreatedBy = dto.CreatedBy;
            followup.CreatedDate = dto.CreatedDate;
            followup.UpdatedBy = dto.UpdatedBy;
            followup.UpdatedDate = dto.UpdatedDate;

            // Set the entity state to modified
            _context.Entry(followup).State = EntityState.Modified;

            // Save changes to the database
            await _context.SaveChangesAsync();

            return dto;
        }

        public async Task<bool> Delete(string id)
        {
            var followup = await _context.TblNewLeadEnquireFollowup.FindAsync(id);
            if (followup == null)
            {
                throw new KeyNotFoundException($"FollowUp with ID {id} not found.");
            }
            followup.IsActive = false; // Soft delete
            _context.TblNewLeadEnquireFollowup.Update(followup); // Update the record
            await _context.SaveChangesAsync(); // Ensure to save changes
            return true;
        }
    }

}
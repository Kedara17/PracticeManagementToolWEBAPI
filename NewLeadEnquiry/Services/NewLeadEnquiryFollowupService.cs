using DataServices.Data;
using DataServices.Models;
using DataServices.Repositories;

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

        // Retrieve all followups
        public async Task<IEnumerable<NewLeadEnquiryFollowupDTO>> GetAll()
        {
            var followups = await _repository.GetAll();
            return followups.Select(f => new NewLeadEnquiryFollowupDTO
            {
                NewLeadEnquiryID = f.NewLeadEnquiryID.ToString(),
                AssignTo = f.AssignTo.ToString(),
                NewFollowupDate = f.NewFollowupDate,
                Comments = f.Comments,
                IsActive = f.IsActive,
                CreatedBy = f.CreatedBy,
                CreatedDate = f.CreatedDate,
                UpdatedBy = f.UpdatedBy,
                UpdatedDate = f.UpdatedDate
            });
        }

        // Get a specific followup by id
        public async Task<NewLeadEnquiryFollowupDTO> Get(string id)
        {
            var followup = await _repository.Get(id);
            if (followup == null) return null;

            return new NewLeadEnquiryFollowupDTO
            {
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

        // Add a new followup
        public async Task<NewLeadEnquiryFollowupDTO> Add(NewLeadEnquiryFollowupDTO dto)
        {
            var newFollowup = new NewLeadEnquiryFollowup
            {
                NewLeadEnquiryID = dto.NewLeadEnquiryID,
                AssignTo = dto.AssignTo,
                NewFollowupDate = dto.NewFollowupDate,
                Comments = dto.Comments,
                IsActive = dto.IsActive,
                CreatedBy = dto.CreatedBy,
                CreatedDate = dto.CreatedDate,
                UpdatedBy = dto.UpdatedBy,
                UpdatedDate = dto.UpdatedDate
            };

            await _repository.Create(newFollowup);
            return dto;
        }

        // Update an existing followup
        public async Task<NewLeadEnquiryFollowupDTO> Update(NewLeadEnquiryFollowupDTO dto)
        {
            var followup = await _repository.Get(dto.Id); // Use the primary key or unique identifier for retrieval
            if (followup == null) throw new KeyNotFoundException("Followup not found.");

            followup.NewLeadEnquiryID = dto.NewLeadEnquiryID; // Update foreign key
            followup.AssignTo = dto.AssignTo;
            followup.NewFollowupDate = dto.NewFollowupDate;
            followup.Comments = dto.Comments;
            followup.IsActive = dto.IsActive;
            followup.CreatedBy = dto.CreatedBy;
            followup.CreatedDate = dto.CreatedDate;
            followup.UpdatedBy = dto.UpdatedBy;
            followup.UpdatedDate = dto.UpdatedDate;

            await _repository.Update(followup);
            return dto;
        }

        // Delete a followup by id
        public async Task<bool> Delete(string id)
        { 
            var followup = await _repository.Get(id);
            if (followup == null)
            {
                throw new KeyNotFoundException($"FollowUp with ID {id} not found.");
            }

            followup.IsActive = false; // Soft delete
            await _repository.Update(followup); // Update the record

            return true;
        }
    }
}

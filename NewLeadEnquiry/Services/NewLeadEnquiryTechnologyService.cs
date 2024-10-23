using DataServices.Data;
using DataServices.Models;
using DataServices.Repositories;
using Microsoft.EntityFrameworkCore;
using NewLeadApi.Services;

namespace LeadEnquiryApi.Services
{
    public class NewLeadEnquiryTechnologyService : INewLeadEnquiryTechnologyService
    {
        private readonly DataBaseContext _context;
        private readonly IRepository<NewLeadEnquiryTechnology> _repository;

        public NewLeadEnquiryTechnologyService(DataBaseContext context, IRepository<NewLeadEnquiryTechnology> repository)
        {
            _context = context;
            _repository = repository;
        }

        public async Task<IEnumerable<NewLeadEnquiryTechnologyDTO>> GetAll()
        {
            var enquiries = await _context.TblNewLeadEnquiryTechnology
                .Include(ne => ne.NewLeadEnquiry)
                .Include(ne => ne.Technology)
                .ToListAsync();

            var enquiryDtos = new List<NewLeadEnquiryTechnologyDTO>();

            foreach (var ne in enquiries)
            {
                enquiryDtos.Add(new NewLeadEnquiryTechnologyDTO()
                {
                    Id = ne.Id,
                    NewLeadEnquiryID = ne.NewLeadEnquiry?.Id,
                    TechnologyID = ne.Technology?.Name,
                    IsActive = ne.IsActive,
                    CreatedDate = DateTime.Now,
                    CreatedBy = ne.CreatedBy,
                    UpdatedDate = DateTime.Now,
                    UpdatedBy = ne.UpdatedBy
                });

            }

            return enquiryDtos;
        }

        public async Task<NewLeadEnquiryTechnologyDTO> Get(string id)
        {
            var enquiryTechnology = await _context.TblNewLeadEnquiryTechnology
                .Include(ne => ne.NewLeadEnquiry)
                .Include(ne => ne.Technology)
                .FirstOrDefaultAsync(ne => ne.Id == id);

            if (enquiryTechnology == null) return null;

            return new NewLeadEnquiryTechnologyDTO
            {
                Id = enquiryTechnology.Id,
                NewLeadEnquiryID = enquiryTechnology.NewLeadEnquiryID,
                TechnologyID = enquiryTechnology.TechnologyID,
                IsActive = enquiryTechnology.IsActive,
                CreatedBy = enquiryTechnology.CreatedBy,
                CreatedDate = enquiryTechnology.CreatedDate,
                UpdatedBy = enquiryTechnology.UpdatedBy,
                UpdatedDate = enquiryTechnology.UpdatedDate
            };
        }

        public async Task<NewLeadEnquiryTechnologyDTO> Add(NewLeadEnquiryTechnologyDTO dto)
        {

            var newLeadEnquiry = await _context.TblNewLeadEnquiry
                .FirstOrDefaultAsync(ne => ne.Id == dto.NewLeadEnquiryID);
            if (newLeadEnquiry == null)
                throw new KeyNotFoundException("New Lead Enquiry not found");

            var technology = await _context.TblTechnology
                .FirstOrDefaultAsync(ne => ne.Name == dto.TechnologyID);
                //.FirstOrDefaultAsync(ne => dto.TechnologyID.Contains(ne.Id));
            if (technology == null)
                throw new KeyNotFoundException("Technology not found");

            var newleadenquiryTechnology = new NewLeadEnquiryTechnology
            {
                NewLeadEnquiryID = newLeadEnquiry.Id,
                TechnologyID = technology.Id,
                IsActive = true,
                CreatedBy = dto.CreatedBy,
                CreatedDate = DateTime.Now,
                UpdatedBy = dto.UpdatedBy,
                UpdatedDate = DateTime.Now
            };
            _context.TblNewLeadEnquiryTechnology.Add(newleadenquiryTechnology);
            await _context.SaveChangesAsync();

            dto.Id = newleadenquiryTechnology.Id;
            return dto;
        }

        public async Task<NewLeadEnquiryTechnologyDTO> Update(NewLeadEnquiryTechnologyDTO dto)
        {
            var newleadenquiryTechnology = await _context.TblNewLeadEnquiryTechnology.FindAsync(dto.Id);

            if (newleadenquiryTechnology == null)
                throw new KeyNotFoundException("New Lead Enquiry Technology entry not found");

            var newLeadEnquiry = await _context.TblNewLeadEnquiry
                .FirstOrDefaultAsync(ne => ne.Id == dto.NewLeadEnquiryID);
            if (newLeadEnquiry == null)
                throw new KeyNotFoundException("New Lead Enquiry not found");

            var technology = await _context.TblTechnology
                //.FirstOrDefaultAsync(ne => ne.Id == dto.TechnologyID);
                .FirstOrDefaultAsync(ne => dto.TechnologyID.Contains(ne.Id));
            if (technology == null)
                throw new KeyNotFoundException("Technology not found");

            // Update the NewLeadEnquiryTechnology properties
            newleadenquiryTechnology.NewLeadEnquiryID = dto.NewLeadEnquiryID;
            newleadenquiryTechnology.TechnologyID = dto.TechnologyID;
            newleadenquiryTechnology.IsActive = dto.IsActive;
            newleadenquiryTechnology.CreatedBy = dto.CreatedBy;
            newleadenquiryTechnology.CreatedDate = dto.CreatedDate;
            newleadenquiryTechnology.UpdatedBy = dto.UpdatedBy;
            newleadenquiryTechnology.UpdatedDate = dto.UpdatedDate;

            _context.Entry(newleadenquiryTechnology).State = EntityState.Modified;

            // Save changes
            await _context.SaveChangesAsync();

            return dto;
        }

        public async Task<bool> Delete(string id)
        {
            var existingData = await _repository.Get(id);
            if (existingData == null)
            {
                throw new ArgumentException($"with ID {id} not found.");
            }
            existingData.IsActive = false; //Soft delete
            await _repository.Update(existingData); // Save changes
            return true;
        }
    }
}

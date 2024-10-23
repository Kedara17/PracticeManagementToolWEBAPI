using DataServices.Data;
using DataServices.Models;
using DataServices.Repositories;
using Microsoft.EntityFrameworkCore;

namespace NewLeadApi.Services
{
    public class NewLeadEnquiryService : INewLeadEnquiryService
    {
        private readonly IRepository<NewLeadEnquiry> _repository;
        private readonly DataBaseContext _context;

        public NewLeadEnquiryService(IRepository<NewLeadEnquiry> repository, DataBaseContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<IEnumerable<NewLeadEnquiryDTO>> GetAll()
        {
            var enquiries = await _context.TblNewLeadEnquiry.ToListAsync();

            var dto = enquiries.Select(enquiry => new NewLeadEnquiryDTO
            {
                Id = enquiry.Id,
                CompanyName = enquiry.CompanyName,
                CompanyRepresentative = enquiry.CompanyRepresentative,
                RepresentativeDesignation = enquiry.RepresentativeDesignation,
                Requirement = enquiry.Requirement,
                EnquiryDate = enquiry.EnquiryDate,
                EmployeeID = enquiry.EmployeeID,
                AssignTo = enquiry.AssignTo,
                Status = enquiry.Status,
                Comments = enquiry.Comments,
                IsActive = enquiry.IsActive,
                UpdatedBy = enquiry.UpdatedBy,
                UpdatedDate = enquiry.UpdatedDate,
                Profile = enquiry.Profile
            }).ToList();
            return dto;
        }

        public async Task<NewLeadEnquiryDTO> Get(string id)
        {
            var newLeadEnquiry = await _repository.Get(id);

            if (newLeadEnquiry == null) return null;

            return new NewLeadEnquiryDTO
            {
                Id = newLeadEnquiry.Id,
                CompanyName = newLeadEnquiry.CompanyName,
                CompanyRepresentative = newLeadEnquiry.CompanyRepresentative,
                RepresentativeDesignation = newLeadEnquiry.RepresentativeDesignation,
                Requirement = newLeadEnquiry.Requirement,
                EnquiryDate = newLeadEnquiry.EnquiryDate,
                EmployeeID = newLeadEnquiry.EmployeeID,
                AssignTo = newLeadEnquiry.AssignTo,
                Status = newLeadEnquiry.Status,
                Comments = newLeadEnquiry.Comments,
                IsActive = newLeadEnquiry.IsActive,
                UpdatedBy = newLeadEnquiry.UpdatedBy,
                UpdatedDate = newLeadEnquiry.UpdatedDate,
                Profile = newLeadEnquiry.Profile
            };
        }

        public async Task<NewLeadEnquiryDTO> Add(NewLeadEnquiryDTO dto)
        {
            var newLeadEnquiry = new NewLeadEnquiry
            {
                CompanyName = dto.CompanyName,
                CompanyRepresentative = dto.CompanyRepresentative,
                RepresentativeDesignation = dto.RepresentativeDesignation,
                Requirement = dto.Requirement,
                EnquiryDate = dto.EnquiryDate,
                EmployeeID = dto.EmployeeID,
                AssignTo = dto.AssignTo,
                Status = dto.Status,
                Comments = dto.Comments,
                IsActive = true, // Assuming new enquiries are active by default
                CreatedBy = dto.CreatedBy,
                CreatedDate = DateTime.UtcNow,
                UpdatedBy = dto.UpdatedBy,
                UpdatedDate = DateTime.UtcNow,
                Profile = dto.Profile
            };

            _context.TblNewLeadEnquiry.Add(newLeadEnquiry);
            await _context.SaveChangesAsync();
            dto.Id = newLeadEnquiry.Id;

            // Handle technologies
            if (dto.TechnologyID != null && dto.TechnologyID.Any())
            {
                foreach (var technologyId in dto.TechnologyID)
                {
                    var newLeadEnquiryTechnology = new NewLeadEnquiryTechnology
                    {
                        NewLeadEnquiryID = newLeadEnquiry.Id,
                        TechnologyID = technologyId.ToString(),
                    };

                    await _context.TblNewLeadEnquiryTechnology.AddAsync(newLeadEnquiryTechnology);
                }
                await _context.SaveChangesAsync();
            }

            return dto;
        }

        public async Task<string> UploadFileAsync(NewLeadEnquiryProfileDTO newLeadEnquiryProfile)
        {
            string filePath = "";
            try
            {
                if (newLeadEnquiryProfile.Profile.Length > 0)
                {
                    var file = newLeadEnquiryProfile.Profile;
                    filePath = Path.GetFullPath($"C:\\Users\\mshaik5\\Desktop\\UploadProfiles\\{file.FileName}");

                    // Save the file
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Update the enquiry's profile if ID is provided
                    if (!string.IsNullOrEmpty(newLeadEnquiryProfile.Id))
                    {
                        var newLeadEnquiry = await Get(newLeadEnquiryProfile.Id);

                        if (newLeadEnquiry != null)
                        {
                            newLeadEnquiry.Profile = file.FileName;
                            await Update(newLeadEnquiry);
                        }
                    }
                    else
                    {
                        return file.FileName;
                    }
                }
                else
                {
                    throw new Exception("The uploaded file is empty.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while uploading the file: " + ex.Message);
            }

            return filePath;
        }

        public async Task<NewLeadEnquiryDTO> Update(NewLeadEnquiryDTO dto)
        {
            var enquiry = await _context.TblNewLeadEnquiry.FindAsync(dto.Id);
            if (enquiry == null)
            {
                throw new KeyNotFoundException($"Lead Enquiry not found for ID: {dto.Id}");
            }

            enquiry.CompanyName = dto.CompanyName;
            enquiry.CompanyRepresentative = dto.CompanyRepresentative;
            enquiry.RepresentativeDesignation = dto.RepresentativeDesignation;
            enquiry.Requirement = dto.Requirement;
            enquiry.EnquiryDate = dto.EnquiryDate;
            enquiry.Status = dto.Status;
            enquiry.Comments = dto.Comments;
            enquiry.IsActive = dto.IsActive;
            enquiry.UpdatedBy = dto.UpdatedBy;
            enquiry.UpdatedDate = DateTime.UtcNow;

            _context.Entry(enquiry).State = EntityState.Modified;

            // Update technologies
            if (dto.TechnologyID != null && dto.TechnologyID.Any())
            {
                // Remove old technologies
                var existingTechnologies = await _context.TblNewLeadEnquiryTechnology
                    .Where(ne =>ne.NewLeadEnquiryID == dto.Id)
                    .ToListAsync();
                _context.TblNewLeadEnquiryTechnology.RemoveRange(existingTechnologies);

                // Add new technologies
                foreach (var technology in dto.TechnologyID)
                {
                    var newLeadEnquiryTechnology = new NewLeadEnquiryTechnology
                    {
                        NewLeadEnquiryID = dto.Id,
                        TechnologyID = technology.ToString(),
                    };
                    await _context.TblNewLeadEnquiryTechnology.AddAsync(newLeadEnquiryTechnology);
                }
            }

            await _context.SaveChangesAsync();

            return dto;
        }

        public async Task<bool> Delete(string id)
        {
            var enquiry = await _repository.Get(id);
            if (enquiry == null)
            {
                throw new ArgumentException($"Lead Enquiry with ID {id} not found.");
            }

            enquiry.IsActive = false; // Soft delete
            await _repository.Update(enquiry);

            return true;
        }
    }
}

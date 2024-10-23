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
            var enquiries = await _context.TblNewLeadEnquiry
            .Include(ne => ne.Employee)
            .Include(ne => ne.AssignedEmployee)
            .ToListAsync();

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
            var newLeadEnquiry = await _context.TblNewLeadEnquiry
            .Include(ne => ne.Employee)
            .Include(ne => ne.AssignedEmployee)
            .FirstOrDefaultAsync(ne => ne.Id == id);

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
            var newLeadEnquiry = new NewLeadEnquiry();

            var employeeId = await _context.TblEmployee
              .FirstOrDefaultAsync(d => d.Id == dto.EmployeeID);
            if (employeeId == null)
                throw new KeyNotFoundException("Employee not found");

            var assignTo = await _context.TblEmployee
               .FirstOrDefaultAsync(d => d.Id == dto.AssignTo);
            if (assignTo == null)
                throw new KeyNotFoundException("AssignTo not found");

            newLeadEnquiry.CompanyName = dto.CompanyName;
            newLeadEnquiry.CompanyRepresentative = dto.CompanyRepresentative;
            newLeadEnquiry.RepresentativeDesignation = dto.RepresentativeDesignation;
            newLeadEnquiry.Requirement = dto.Requirement;
            newLeadEnquiry.EnquiryDate = dto.EnquiryDate;
            newLeadEnquiry.EmployeeID = dto.EmployeeID;
            newLeadEnquiry.AssignTo = dto.AssignTo;
            newLeadEnquiry.Status = dto.Status;
            newLeadEnquiry.Comments = dto.Comments;
            newLeadEnquiry.IsActive = true; // Assuming new enquiries are active by default
            newLeadEnquiry.CreatedBy = dto.CreatedBy;
            newLeadEnquiry.CreatedDate = DateTime.UtcNow;
            newLeadEnquiry.UpdatedBy = dto.UpdatedBy;
            newLeadEnquiry.UpdatedDate = DateTime.UtcNow;
            newLeadEnquiry.Profile = dto.Profile;

            // Set the Profile property if a file is uploaded
            if (!string.IsNullOrEmpty(dto.Profile))
            {
                newLeadEnquiry.Profile = dto.Profile;
            }

            _context.TblNewLeadEnquiry.Add(newLeadEnquiry);
            await _context.SaveChangesAsync();
            dto.Id = newLeadEnquiry.Id;

            // Handle technologies
            if (dto.Technology != null && dto.Technology.Any())
            {
                foreach (var technologyId in dto.Technology)
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
                    filePath = Path.GetFullPath($"C:\\Users\\skolli5\\Desktop\\UpdatedProfiles\\{file.FileName}");
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
            var newLeadEnquiry = await _context.TblNewLeadEnquiry.FindAsync(dto.Id);
            if (newLeadEnquiry == null)
            {
                throw new KeyNotFoundException($"Lead Enquiry not found for ID: {dto.Id}");
            }

            var employeeId = await _context.TblEmployee.FindAsync(dto.EmployeeID);
            if (employeeId == null)
                throw new KeyNotFoundException("EmployeeID not found");

            var assignTo = await _context.TblEmployee
                .FirstOrDefaultAsync(d => d.Id == dto.AssignTo);
            if (assignTo == null)
                throw new KeyNotFoundException("AssignTo not found");

            newLeadEnquiry.CompanyName = dto.CompanyName;
            newLeadEnquiry.CompanyRepresentative = dto.CompanyRepresentative;
            newLeadEnquiry.RepresentativeDesignation = dto.RepresentativeDesignation;
            newLeadEnquiry.Requirement = dto.Requirement;
            newLeadEnquiry.EnquiryDate = dto.EnquiryDate;
            newLeadEnquiry.Status = dto.Status;
            newLeadEnquiry.Comments = dto.Comments;
            newLeadEnquiry.IsActive = dto.IsActive;
            newLeadEnquiry.UpdatedBy = dto.UpdatedBy;
            newLeadEnquiry.UpdatedDate = DateTime.UtcNow;

            // Set the Profile property if a file is uploaded
            if (!string.IsNullOrEmpty(dto.Profile))
            {
                newLeadEnquiry.Profile = dto.Profile;
            }

            _context.Entry(newLeadEnquiry).State = EntityState.Modified;

            // Update technologies
            if (dto.Technology != null && dto.Technology.Any())
            {
                // Remove old technologies
                var existingTechnologies = await _context.TblNewLeadEnquiryTechnology
                    .Where(ne => ne.NewLeadEnquiryID == dto.Id)
                    .ToListAsync();
                _context.TblNewLeadEnquiryTechnology.RemoveRange(existingTechnologies);

                // Add new technologies
                foreach (var technologyId in dto.Technology)
                {
                    var newLeadEnquiryTechnology = new NewLeadEnquiryTechnology
                    {
                        NewLeadEnquiryID = dto.Id,
                        TechnologyID = technologyId.ToString(),
                    };
                    await _context.TblNewLeadEnquiryTechnology.AddAsync(newLeadEnquiryTechnology);
                }
            }

            await _context.SaveChangesAsync();

            return dto;
        }

        public async Task<bool> Delete(string id)
        {
            var existingData = await _repository.Get(id);
            if (existingData == null)
            {
                throw new ArgumentException($"Lead Enquiry with ID {id} not found.");
            }

            existingData.IsActive = false; // Soft delete
            await _repository.Update(existingData);
            return true;
        }
    }
}

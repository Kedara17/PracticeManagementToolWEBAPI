using DataServices.Data;
using DataServices.Models;
using DataServices.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Runtime.InteropServices;

namespace NewLeadApi.Services
{
    public class NewLeadEnquiryService : INewLeadEnquiryService
    {
        private readonly IRepository<NewLeadEnquiry> _repository;
        private readonly DataBaseContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public NewLeadEnquiryService(IRepository<NewLeadEnquiry> repository, DataBaseContext context, IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
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
                CreatedBy = enquiry.CreatedBy,
                CreatedDate = enquiry.CreatedDate,
                UpdatedBy = enquiry.UpdatedBy,
                UpdatedDate = enquiry.UpdatedDate,
                FileName = enquiry.FileName,
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

            var document = await _context.TblNewLeadEnquiryDocuments
        .FirstOrDefaultAsync(doc => doc.NewLeadEnquiryID == newLeadEnquiry.Id);

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
                CreatedBy = newLeadEnquiry.CreatedBy,
                CreatedDate = newLeadEnquiry.CreatedDate,
                UpdatedBy = newLeadEnquiry.UpdatedBy,
                UpdatedDate = newLeadEnquiry.UpdatedDate,
                FileName = document?.FileName,
            };
        }

        public async Task<NewLeadEnquiryDTO> Add(NewLeadEnquiryDTO dto)
        {
            //var leadEnquiry = _httpContextAccessor.HttpContext?.User?.Identity?.Name;

            var newLeadEnquiry = new NewLeadEnquiry();

            // Check if the Enquiry name already exists
            //     var existingEnquiry = await _context.TblNewLeadEnquiry
            //    .FirstOrDefaultAsync(t => t.CompanyName == dto.CompanyName);

            //if (existingEnquiry != null)
            //    throw new ArgumentException("A enquiry with the same name already exists.");

            newLeadEnquiry.CompanyName = dto.CompanyName;
            newLeadEnquiry.CompanyRepresentative = dto.CompanyRepresentative;
            newLeadEnquiry.RepresentativeDesignation = dto.RepresentativeDesignation;
            newLeadEnquiry.Requirement = dto.Requirement;
            newLeadEnquiry.EnquiryDate = dto.EnquiryDate;
            newLeadEnquiry.EmployeeID = dto.EmployeeID;
            newLeadEnquiry.AssignTo = dto.AssignTo;
            newLeadEnquiry.Status = dto.Status;
            newLeadEnquiry.Comments = dto.Comments;
            newLeadEnquiry.IsActive = true; 
            newLeadEnquiry.CreatedBy = dto.CreatedBy;
            newLeadEnquiry.CreatedDate = DateTime.Now;
            newLeadEnquiry.FileName = dto.FileName;

            await _context.TblNewLeadEnquiry.AddAsync(newLeadEnquiry);
            await _context.SaveChangesAsync();

            // Set the Profile property if a file is uploaded
            if (!string.IsNullOrEmpty(dto.FileName))
            {          
                var existingDocument = await _context.TblNewLeadEnquiryDocuments
           .FirstOrDefaultAsync(doc => doc.NewLeadEnquiryID == newLeadEnquiry.Id);

                if (existingDocument == null)
                {
                    // Add new document if it doesn't exist
                    await _context.TblNewLeadEnquiryDocuments.AddAsync(new NewLeadEnquiryDocuments
                    {
                        NewLeadEnquiryID = newLeadEnquiry.Id,
                        FileName = dto.FileName
                    });
                }
                else
                {
                    // Update existing document
                    existingDocument.FileName = dto.FileName;
                    _context.Entry(existingDocument).State = EntityState.Modified;
                }
            }

            // Handle technologies
            if (dto.Technology != null && dto.Technology.Any())
            {              
                var technologies = dto.Technology.Select(techId => new NewLeadEnquiryTechnology
                {
                    NewLeadEnquiryID = newLeadEnquiry.Id,
                    TechnologyID = techId.ToString()
                });
                await _context.TblNewLeadEnquiryTechnology.AddRangeAsync(technologies);
            }

            await _context.SaveChangesAsync();
            dto.Id = newLeadEnquiry.Id;

            return dto;
        }

        public async Task<string> UploadFileAsync(NewLeadEnquiryFileNameDTO newLeadEnquiryFileName)
        {
            // Generate the filename with the original document name and current date.
            var filename = Path.GetFileNameWithoutExtension(newLeadEnquiryFileName.FileName.FileName) + Path.GetExtension(newLeadEnquiryFileName.FileName.FileName);

            try
            {
                // Get the file extension
                var extension = Path.GetExtension(filename);

                // Define the upload directory path
                var filepath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads\\Resumes");

                // Create the directory if it does not exist
                if (!Directory.Exists(filepath))
                {
                    Directory.CreateDirectory(filepath);
                }

                // Combine the directory and filename for the full path
                var completepath = Path.Combine(filepath, filename);

                // Save the file
                using (var stream = new FileStream(completepath, FileMode.Create))
                {
                    await newLeadEnquiryFileName.FileName.CopyToAsync(stream);
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                Console.WriteLine(ex.Message);
            }
            return filename;
        }

        public async Task<FileContentResult> DownloadFileAsync(string filename)
        {
            // Construct the full file path
            var completePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads\\Resumes", filename);

            // Check if the file exists
            if (!System.IO.File.Exists(completePath))
            {
                return null; // Return null if file does not exist
            }

            // Determine the content type based on the file extension
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(completePath, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            // Read the file as a byte array
            var fileBytes = await System.IO.File.ReadAllBytesAsync(completePath);

            // Return the file as FileContentResult
            return new FileContentResult(fileBytes, contentType)
            {
                FileDownloadName = filename
            };
        }

        /* public async Task<string> UploadFileAsync(NewLeadEnquiryFileNameDTO newLeadEnquiryFileName)
         {
             string filePath = "";
             try
             {
                 if (newLeadEnquiryFileName.FileName.Length > 0)
                 {
                     var file = newLeadEnquiryFileName.FileName;
                     filePath = Path.GetFullPath($"C:\\Users\\skolli5\\UpdatedProfiles\\Resumes\\{file.FileName}");
                     // Save the file
                     using (var stream = System.IO.File.Create(filePath))
                     {
                         await file.CopyToAsync(stream);
                     }

                     // Update the enquiry's profile if ID is provided
                     if (!string.IsNullOrEmpty(newLeadEnquiryFileName.Id))
                     {
                         var newLeadEnquiry = await Get(newLeadEnquiryFileName.Id);

                         if (newLeadEnquiry != null)
                         {
                             newLeadEnquiry.FileName = file.FileName;
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
 */
        public async Task<NewLeadEnquiryDTO> Update(NewLeadEnquiryDTO dto)
        {

            //var userName = _httpContextAccessor.HttpContext?.User?.FindFirst("LeadEnquiry")?.Value;

            // Check if the Enquiry name already exists
           

            var newLeadEnquiry = await _context.TblNewLeadEnquiry.FindAsync(dto.Id);
            if (newLeadEnquiry == null)
            {
                throw new KeyNotFoundException($"Lead Enquiry not found for ID: {dto.Id}");
            }


            newLeadEnquiry.CompanyName = dto.CompanyName;
            newLeadEnquiry.CompanyRepresentative = dto.CompanyRepresentative;
            newLeadEnquiry.RepresentativeDesignation = dto.RepresentativeDesignation;
            newLeadEnquiry.Requirement = dto.Requirement;
            newLeadEnquiry.EnquiryDate = dto.EnquiryDate;
            newLeadEnquiry.Status = dto.Status;
            newLeadEnquiry.Comments = dto.Comments;
            newLeadEnquiry.IsActive = dto.IsActive;           
            newLeadEnquiry.UpdatedBy = dto.UpdatedBy;
            newLeadEnquiry.UpdatedDate = DateTime.Now;

            // Set the Profile property if a file is uploaded
            if (!string.IsNullOrEmpty(dto.FileName))
            {
                var existingDocument = await _context.TblNewLeadEnquiryDocuments
                    .FirstOrDefaultAsync(d => d.NewLeadEnquiryID == dto.Id);

                if (existingDocument != null)
                {
                    existingDocument.FileName = dto.FileName;
                    _context.Entry(existingDocument).State = EntityState.Modified;
                }
                else
                {
                    var newDocument = new NewLeadEnquiryDocuments
                    {
                        NewLeadEnquiryID = dto.Id,
                        FileName = dto.FileName
                    };
                    await _context.TblNewLeadEnquiryDocuments.AddAsync(newDocument);
                }
            }

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

using DataServices.Data;
using DataServices.Models;
using DataServices.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace POCAPI.Services
{
    public class POCService : IPOCService
    {
        private readonly IRepository<POC> _repository;
        private readonly DataBaseContext _context;

        public POCService(IRepository<POC> repository, DataBaseContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<IEnumerable<POCDTO>> GetAll()
        {
            var pocs = await _context.TblPOC.Include(e => e.Client).ToListAsync();
            var pocDtos = new List<POCDTO>();

            foreach (var poc in pocs)
            {
                pocDtos.Add(new POCDTO
                {
                    Id = poc.Id,
                    Title = poc.Title,
                    Client = poc.Client?.Name,
                    Status = poc.Status,
                    TargetDate = poc.TargetDate,
                    CompletedDate = poc.CompletedDate,
                    Document = poc.Document,
                    IsActive = poc.IsActive,
                    CreatedBy = poc.CreatedBy,
                    CreatedDate = poc.CreatedDate,
                    UpdatedBy = poc.UpdatedBy,
                    UpdatedDate = poc.UpdatedDate
                });
            }
            return pocDtos;
        }

        public async Task<POCDTO> Get(string id)
        {
            var poc = await _context.TblPOC
                .Include(e => e.Client)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (poc == null)
                return null;

            return new POCDTO
            {
                Id = poc.Id,
                Title = poc.Title,
                Client = poc.Client?.Name,
                Status = poc.Status,
                TargetDate = poc.TargetDate,
                CompletedDate = poc.CompletedDate,
                Document = poc.Document,
                IsActive = poc.IsActive,
                CreatedBy = poc.CreatedBy,
                CreatedDate = poc.CreatedDate,
                UpdatedBy = poc.UpdatedBy,
                UpdatedDate = poc.UpdatedDate
            };
        }

        public async Task<POCDTO> Add(POCDTO pocDto)
        {
            var poc = new POC();
            // Check if the POC already exists
            var existingPOC = await _context.TblPOC
                .FirstOrDefaultAsync(t => t.Title == pocDto.Title);
            if (existingPOC != null)
                throw new ArgumentException("A POC with the same name already exists.");

            /* var client = await _context.TblClient
                 .FirstOrDefaultAsync(d => d.Name == pocDto.Client);

             if (client == null)
                 throw new KeyNotFoundException("Client not found");*/

           
            //____________________________________________________________
            if (!string.IsNullOrWhiteSpace(pocDto.Status))
            {
                poc.Status = pocDto.Status;
            }
            else
            {
                poc.Status = null;
            }
            //____________________________________________________________
            if (pocDto.TargetDate.HasValue)
            {
                poc.TargetDate = pocDto.TargetDate;
            }
            else
            {
                poc.TargetDate = null;
            }
            //_____________________________________________________________
            if (pocDto.CompletedDate.HasValue)
            {
                poc.CompletedDate = pocDto.CompletedDate;
            }
            else
            {
                poc.CompletedDate = null;
            }
            //____________________________________________________________
            if (!string.IsNullOrWhiteSpace(pocDto.Document))
            {
                poc.Document = pocDto.Document;
            }
            else
            {
                poc.Document = null;
            }

            poc.Title = pocDto.Title;
            poc.ClientId = pocDto.Client;
            /* poc.Status = pocDto?.Status;
             poc.TargetDate = pocDto.TargetDate;
             poc.CompletedDate = pocDto.CompletedDate;
             poc.Document = pocDto.Document;*/
            poc.IsActive = pocDto.IsActive;
            poc.CreatedBy = pocDto.CreatedBy;
            poc.CreatedDate = pocDto.CreatedDate;
            poc.UpdatedBy = pocDto.UpdatedBy;
            poc.UpdatedDate = pocDto.UpdatedDate;       

            // Set the Profile property if a file is uploaded
            if (!string.IsNullOrEmpty(pocDto.Document))
            {
                poc.Document = pocDto.Document;
            }

            _context.TblPOC.Add(poc);
            await _context.SaveChangesAsync();

            pocDto.Id = poc.Id;
            return pocDto;
        }

        [HttpPost]
        public async Task<string> UploadFileAsync(POCDocumentDTO pocDto)
        {
            // Generate the filename with the original document name and current date.
            var filename = Path.GetFileNameWithoutExtension(pocDto.Document.FileName) + Path.GetExtension(pocDto.Document.FileName);

            try
            {
                // Get the file extension
                var extension = Path.GetExtension(filename);

                // Define the upload directory path
                var filepath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads\\Documents");

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
                    await pocDto.Document.CopyToAsync(stream);
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                Console.WriteLine(ex.Message);
            }
            return filename;
        }

        [HttpGet("download")]
        public async Task<FileContentResult> DownloadFileAsync(string filename)
        {
            // Construct the full file path
            var completePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads\\Documents", filename);

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
            
        /*       public async Task<string> UploadFileAsync(POCDocumentDTO pocdoc)
               {
                   string filePath = "";
                   try
                   {
                       // Check if the file is not empty
                       if (pocdoc.Document.Length > 0)
                       {
                           var file = pocdoc.Document;
                           filePath = Path.GetFullPath($"C:\\Users\\rneerukonda1\\Desktop\\UploadProfiles\\UPLOADEDFILES\\NewFile\\{file.FileName}");

                           // Save file to the specified path
                           using (var stream = System.IO.File.Create(filePath))
                           {
                               await file.CopyToAsync(stream);
                           }

                           if (!string.IsNullOrEmpty(pocdoc.Id))
                           {
                               var poc = await Get(pocdoc.Id);

                               if (poc != null)
                               {
                                   poc.Document = file.FileName;
                                   await Update(poc);
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
               }*/

        public async Task<POCDTO> Update(POCDTO pocDto)
        {
            var poc = await _context.TblPOC.FindAsync(pocDto.Id);
            // Check if the POC already exists
            var existingPOC = await _context.TblPOC
                .FirstOrDefaultAsync(t => t.Title == pocDto.Title);

            if (existingPOC != null)
                throw new ArgumentException("A POC with the same name already exists.");

            var pocData = await _context.TblPOC.FindAsync(pocDto.Id);

            if (pocData == null)
                throw new KeyNotFoundException("Poc not found");

            /*  var client = await _context.TblClient
                  .FirstOrDefaultAsync(d => d.Name == pocDto.Client);

              if (client == null)
                  throw new KeyNotFoundException("Author not found");*/

            //____________________________________________________________
            if (!string.IsNullOrWhiteSpace(pocDto.Status))
            {
                poc.Status = pocDto.Status;
            }
            else
            {
                poc.Status = null;
            }
            //____________________________________________________________
            if (pocDto.TargetDate.HasValue)
            {
                poc.TargetDate = pocDto.TargetDate;
            }
            else
            {
                poc.TargetDate = null;
            }
            //_____________________________________________________________
            if (pocDto.CompletedDate.HasValue)
            {
                poc.CompletedDate = pocDto.CompletedDate;
            }
            else
            {
                poc.CompletedDate = null;
            }
            //____________________________________________________________
            if (!string.IsNullOrWhiteSpace(pocDto.Document))
            {
                poc.Document = pocDto.Document;
            }
            else
            {
                poc.Document = null;
            }

            poc.Title = pocDto.Title;
            poc.ClientId = pocDto.Client;
            /* poc.Status = pocDto?.Status;
             poc.TargetDate = pocDto.TargetDate;
             poc.CompletedDate = pocDto.CompletedDate;
             poc.Document = pocDto.Document;*/
            poc.IsActive = pocDto.IsActive;
            poc.CreatedBy = pocDto.CreatedBy;
            poc.CreatedDate = pocDto.CreatedDate;
            poc.UpdatedBy = pocDto.UpdatedBy;
            poc.UpdatedDate = pocDto.UpdatedDate;

            // Set the Profile property if a file is uploaded
            if (!string.IsNullOrEmpty(pocDto.Document))
            {
                poc.Document = pocDto.Document;
            }

            _context.Entry(poc).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return pocDto;
        }

        public async Task<bool> Delete(string id)
        {
            // Check if the Blogs exists
            var existingData = await _repository.Get(id);
            if (existingData == null)
            {
                throw new ArgumentException($"Blogs with ID {id} not found.");
            }
            existingData.IsActive = false; // Soft delete
            await _repository.Update(existingData); // Save changes
            return true;
        }
    }
}

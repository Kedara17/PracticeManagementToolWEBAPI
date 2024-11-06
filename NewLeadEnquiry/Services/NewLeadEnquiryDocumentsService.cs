using DataServices.Data;
using DataServices.Models;
using DataServices.Repositories;
using Microsoft.EntityFrameworkCore;

namespace NewLeadApi.Services
{
    public class NewLeadEnquiryDocumentsService : INewLeadEnquiryDocumentsService
    {
        private readonly DataBaseContext _context;
        private readonly IRepository<NewLeadEnquiryDocuments> _repository;

        public NewLeadEnquiryDocumentsService(DataBaseContext context, IRepository<NewLeadEnquiryDocuments> repository)
        {
            _context = context;
            _repository = repository;
        }

        // Get all documents
        public async Task<IEnumerable<NewLeadEnquiryDocumentsDTO>> GetAll()
        {
            var documents = await _context.TblNewLeadEnquiryDocuments
                .Include(ne => ne.NewLeadEnquirys)
                .ToListAsync();

            var documentDtos = documents.Select(doc => new NewLeadEnquiryDocumentsDTO
            {
                Id = doc.Id, // Assuming Id is present in NewLeadEnquiryDocuments
                NewLeadEnquiryID = doc.NewLeadEnquiryID,
                FileName = doc.FileName,
                IsActive = doc.IsActive,
                CreatedBy = doc.CreatedBy,
                CreatedDate = doc.CreatedDate,
                UpdatedBy = doc.UpdatedBy,
                UpdatedDate = doc.UpdatedDate
            });

            return documentDtos;
        }

        // Get a document by ID
        public async Task<NewLeadEnquiryDocumentsDTO> Get(string id)
        {
            var document = await _context.TblNewLeadEnquiryDocuments
                .Include(ne => ne.NewLeadEnquirys)
                .FirstOrDefaultAsync(doc => doc.Id == id);

            if (document == null) return null;

            return new NewLeadEnquiryDocumentsDTO
            {
                Id = document.Id,
                NewLeadEnquiryID = document.NewLeadEnquiryID,
                FileName = document.FileName,
                IsActive = document.IsActive,
                CreatedBy = document.CreatedBy,
                CreatedDate = document.CreatedDate,
                UpdatedBy = document.UpdatedBy,
                UpdatedDate = document.UpdatedDate
            };
        }

        // Add a new document
        public async Task<NewLeadEnquiryDocumentsDTO> Add(NewLeadEnquiryDocumentsDTO dto)
        {
            var newLeadEnquiry = await _context.TblNewLeadEnquiry
                .FirstOrDefaultAsync(ne => ne.Id == dto.NewLeadEnquiryID);
            if (newLeadEnquiry == null)
                throw new KeyNotFoundException("NewLeadEnquiry not found");

            var newDocument = new NewLeadEnquiryDocuments
            {
                NewLeadEnquiryID = dto.NewLeadEnquiryID,
                FileName = dto.FileName,
                IsActive = true, // Default to active
                CreatedBy = dto.CreatedBy,
                CreatedDate = DateTime.Now,
                UpdatedBy = dto.UpdatedBy,
                UpdatedDate = DateTime.Now
            };

            await _context.TblNewLeadEnquiryDocuments.AddAsync(newDocument);
            await _context.SaveChangesAsync();

            dto.Id = newDocument.Id; // Assuming Id is auto-generated
            return dto;
        }

        // Update an existing document
        public async Task<NewLeadEnquiryDocumentsDTO> Update(NewLeadEnquiryDocumentsDTO dto)
        {
            var document = await _context.TblNewLeadEnquiryDocuments.FindAsync(dto.Id);

            if (document == null) throw new KeyNotFoundException("Document not found.");

            var newLeadEnquiry = await _context.TblNewLeadEnquiry
                .FirstOrDefaultAsync(ne => ne.Id == dto.NewLeadEnquiryID);
            if (newLeadEnquiry == null)
                throw new KeyNotFoundException("NewLeadEnquiry not found");


            document.NewLeadEnquiryID = dto.NewLeadEnquiryID;
            document.FileName = dto.FileName;
            document.IsActive = dto.IsActive;
            document.CreatedBy = dto.CreatedBy;
            document.CreatedDate = dto.CreatedDate;
            document.UpdatedBy = dto.UpdatedBy;
            document.UpdatedDate = DateTime.Now;

            _context.Entry(document).State = EntityState.Modified;

            // Save changes
            await _context.SaveChangesAsync();

            return dto;
        }

        // Delete a document
        public async Task<bool> Delete(string id)
        {
            var existingData = await _repository.Get(id);
            if (existingData == null)
            {
                throw new ArgumentException($"Document with ID {id} not found.");
            }

            existingData.IsActive = false; // Soft delete
            await _repository.Update(existingData); // Save changes
            return true;
        }
    }
}

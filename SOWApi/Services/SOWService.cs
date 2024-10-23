using DataServices.Data;
using DataServices.Models;
using DataServices.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Net.NetworkInformation;
using System.Xml.Linq;

namespace SOWApi.Services
{
    public class SOWService : ISOWService
    {
        private readonly IRepository<SOW> _repository;
        private readonly DataBaseContext _context;

        public SOWService(IRepository<SOW> repository, DataBaseContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<IEnumerable<SOWDTO>> GetAll()
        {
            var sows = await _context.TblSOW
                .Include(c => c.Client)
                .Include(t => t.Project)
                .Include(s => s.SOWStatus)
                .ToListAsync();

            var sowDtos = sows.Select(sow=> new SOWDTO
            { 
                    Id = sow.Id,
                    Title = sow.Title,
                    Client = sow.Client?.Name,
                    Project = sow.Project?.ProjectName,
                    PreparedDate = sow.PreparedDate,
                    SubmittedDate = sow.SubmittedDate,
                    Status = sow.SOWStatus?.Status,
                    Comments = sow.Comments,
                    IsActive = sow.IsActive,
                    CreatedBy = sow.CreatedBy,
                    CreatedDate = sow.CreatedDate,
                    UpdatedBy = sow.UpdatedBy,
                    UpdatedDate = sow.UpdatedDate
                }).ToList();
          
            return sowDtos;
        }

        public async Task<SOWDTO> Get(string id)
        {
            var sows = await _context.TblSOW
                .Include(c => c.Client)
                .Include(t => t.Project)
                .Include(s => s.SOWStatus)
                .FirstOrDefaultAsync(t => t.Id == id);
            if (sows == null) return null;

            return new SOWDTO
            {
                Id = sows.Id,
                Title = sows.Title,
                Client = sows.Client?.Name,
                Project = sows.Project?.ProjectName,
                PreparedDate = sows.PreparedDate,
                SubmittedDate = sows.SubmittedDate,
                Status = sows.SOWStatus?.Status,
                Comments = sows.Comments,
                IsActive = sows.IsActive,
                CreatedBy = sows.CreatedBy,
                CreatedDate = sows.CreatedDate,
                UpdatedBy = sows.UpdatedBy,
                UpdatedDate = sows.UpdatedDate
            };
        }

        public async Task<SOWDTO> Add(SOWDTO _object)
        {
            // Check if the SOW Title already exists
            var existingSOW = await _context.TblSOW
                .FirstOrDefaultAsync(t => t.Title == _object.Title);

            if (existingSOW != null)
                throw new ArgumentException("A Title with the same name already exists.");

            var client = await _context.TblClient
               .FirstOrDefaultAsync(d => d.Name == _object.Client);

            if (client == null)
                throw new KeyNotFoundException("Client not found");

            var project = await _context.TblProject
               .FirstOrDefaultAsync(d => d.ProjectName == _object.Project);

            if (project == null)
                throw new KeyNotFoundException("Project not found");

            var status = await _context.TblSOWStatus
               .FirstOrDefaultAsync(d => d.Status == _object.Status);

            if (status == null)
                throw new KeyNotFoundException("SalesContact not found");


            var sow = new SOW
            {
                Title = _object.Title,
                ClientId = client?.Id,
                ProjectId = project?.Id,
                PreparedDate = _object.PreparedDate,
                SubmittedDate = _object.SubmittedDate,
                Status = status?.Id,
                Comments = _object.Comments,
                IsActive = _object.IsActive,
                CreatedBy = _object.CreatedBy,
                CreatedDate = _object.CreatedDate,
                UpdatedBy = _object.UpdatedBy,
                UpdatedDate = _object.UpdatedDate
            };

            _context.TblSOW.Add(sow);
            await _context.SaveChangesAsync();

            _object.Id = sow.Id;
            return _object;

        }

        public async Task<SOWDTO> Update(SOWDTO _object)
        {
            // Check if the SOW Title already exists
            var existingSOW = await _context.TblSOW
                .FirstOrDefaultAsync(t => t.Title == _object.Title);

            if (existingSOW != null)
                throw new ArgumentException("A Title with the same name already exists.");

            var sow = await _context.TblSOW.FindAsync(_object.Id);

            if (sow == null)
                throw new KeyNotFoundException("SOW not found");

            var client = await _context.TblClient
              .FirstOrDefaultAsync(d => d.Name == _object.Client);

            if (client == null)
                throw new KeyNotFoundException("Client not found");

            var project = await _context.TblProject
               .FirstOrDefaultAsync(d => d.ProjectName == _object.Project);

            if (project == null)
                throw new KeyNotFoundException("Project not found");

            var status = await _context.TblSOWStatus
               .FirstOrDefaultAsync(d => d.Status == _object.Status);

            if (status == null)
                throw new KeyNotFoundException("SalesContact not found");
            sow.Title = _object.Title;
            sow.ClientId = client?.Id;
            sow.ProjectId = project?.Id;
            sow.PreparedDate = _object.PreparedDate;
            sow.SubmittedDate = _object.SubmittedDate;
            sow.Status = status?.Id;
            sow.Comments = _object.Comments;
            sow.IsActive = _object.IsActive;
            sow.CreatedBy = _object.CreatedBy;
            sow.CreatedDate = _object.CreatedDate;
            sow.UpdatedBy = _object.UpdatedBy;
            sow.UpdatedDate = _object.UpdatedDate;

            _context.Entry(sow).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return _object;
        }

        public async Task<bool> Delete(string id)
        {
            // Check if the SOW exists
            var existingsow = await _repository.Get(id);
            if (existingsow == null)
            {
                throw new ArgumentException($"SOW with ID {id} not found.");
            }

            existingsow.IsActive = false; // Soft delete
            await _repository.Update(existingsow); // Save changes
            return true;
        }
    }
}
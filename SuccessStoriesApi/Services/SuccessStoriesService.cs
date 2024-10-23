//SuccessStoriesServices


using DataServices.Data;
using DataServices.Models;
using DataServices.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SuccessStoriesApi.Services
{
    public class SuccessStoriesService : ISuccessStoriesService
    {
        private readonly IRepository<SuccessStories> _repository;
        private readonly DataBaseContext _context;

        public SuccessStoriesService(IRepository<SuccessStories> repository, DataBaseContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<IEnumerable<SuccessStoriesDTO>> GetAll()
        {
            var successStories = await _context.TblSuccessStories
                .Include(t => t.Employee)
                .Include(t => t.Client)
                .Include(t => t.Project)
                .ToListAsync();

            return successStories.Select(c => new SuccessStoriesDTO
            {
                Id = c.Id,
                Client = c.Client?.Name ?? "N/A",
                Project = c.Project?.ProjectName ?? "N/A",
                AssignTo = c.Employee?.Name?? "N/A",
                Status = c.Status,
                Comments = c.Comments,
                IsActive = c.IsActive,
                CreatedBy = c.CreatedBy,
                CreatedDate = c.CreatedDate,
                UpdatedBy = c.UpdatedBy,
                UpdatedDate = c.UpdatedDate
            }).ToList();
        }

        public async Task<SuccessStoriesDTO> Get(string id)
        {
            var successStory = await _context.TblSuccessStories
                .Include(t => t.Employee)
                .Include(t => t.Client)
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (successStory == null)
                return null;

            return new SuccessStoriesDTO
            {
                Id = successStory.Id,
                Client = successStory.Client?.Id ?? "N/A",
                Project = successStory.Project?.Id ?? "N/A",
                AssignTo = successStory.Employee?.Id ?? "N/A",
                Status = successStory.Status,
                Comments = successStory.Comments,
                IsActive = successStory.IsActive,
                CreatedBy = successStory.CreatedBy,
                CreatedDate = successStory.CreatedDate,
                UpdatedBy = successStory.UpdatedBy,
                UpdatedDate = successStory.UpdatedDate
            };
        }

        public async Task<SuccessStoriesDTO> Add(SuccessStoriesDTO successStoriesDto)
        {
            var project = await _context.TblProject.FirstOrDefaultAsync(p => p.ProjectName == successStoriesDto.Project);
            if (project == null)
                throw new KeyNotFoundException("Project not found");

            var client = await _context.TblClient.FirstOrDefaultAsync(c => c.Name == successStoriesDto.Client);
            if (client == null)
                throw new KeyNotFoundException("Client not found");

            var assignTo = await _context.TblEmployee.FirstOrDefaultAsync(e => e.Name == successStoriesDto.AssignTo);
            if (assignTo == null)
                throw new KeyNotFoundException("Employee (AssignTo) not found");

            var successStory = new SuccessStories
            {
                ClientID = client.Id,
                ProjectId = project.Id,
                AssignTo = assignTo.Id,
                Status = successStoriesDto.Status,
                Comments = successStoriesDto.Comments,
                IsActive = successStoriesDto.IsActive,
                CreatedBy = successStoriesDto.CreatedBy,
                CreatedDate = DateTime.UtcNow,
                UpdatedBy = successStoriesDto.UpdatedBy,
                UpdatedDate = DateTime.UtcNow
            };

            _context.TblSuccessStories.Add(successStory);
            await _context.SaveChangesAsync();

            successStoriesDto.Id = successStory.Id;
            return successStoriesDto;
        }

        public async Task<SuccessStoriesDTO> Update(SuccessStoriesDTO successStoriesDto)
        {
            var successStory = await _context.TblSuccessStories.FindAsync(successStoriesDto.Id);
            if (successStory == null)
                throw new KeyNotFoundException("Success Story not found");

            var project = await _context.TblProject.FirstOrDefaultAsync(p => p.ProjectName == successStoriesDto.Project);
            if (project == null)
                throw new KeyNotFoundException("Project not found");

            var client = await _context.TblClient.FirstOrDefaultAsync(c => c.Name == successStoriesDto.Client);
            if (client == null)
                throw new KeyNotFoundException("Client not found");

            var assignTo = await _context.TblEmployee.FirstOrDefaultAsync(e => e.Name == successStoriesDto.AssignTo);
            if (assignTo == null)
                throw new KeyNotFoundException("Employee (AssignTo) not found");

            // Update fields
            successStory.ClientID = client.Id;
            successStory.ProjectId = project.Id;
            successStory.AssignTo = assignTo.Id;
            successStory.Status = successStoriesDto.Status;
            successStory.Comments = successStoriesDto.Comments;
            successStory.IsActive = successStoriesDto.IsActive;
            successStory.UpdatedBy = successStoriesDto.UpdatedBy;
            successStory.UpdatedDate = DateTime.UtcNow;

            _context.TblSuccessStories.Update(successStory);
            await _context.SaveChangesAsync();

            return successStoriesDto;
        }

        public async Task<bool> Delete(string id)
        {
            var successStory = await _context.TblSuccessStories.FindAsync(id);
            if (successStory == null)
                return false;

            _context.TblSuccessStories.Remove(successStory);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

using DataServices.Data;
using DataServices.Models;
using DataServices.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;

namespace BestPerformersAPI.Services
{
    public class BestPerformersService : IBestPerformersService
    {
        private readonly IRepository<BestPerformers> _repository;
        private readonly DataBaseContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BestPerformersService(IRepository<BestPerformers> repository, DataBaseContext context, IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<BestPerformersDTO>> GetAll()
        {
            var bestperformers = await _context.TblBestPerformers
                .Include(e => e.Employee)
                .Include(e => e.Client)
                .Include(e => e.Project)
                .ToListAsync();

            var bestperformersDtos = new List<BestPerformersDTO>();

            foreach (var bestperformer in bestperformers)
            {
                bestperformersDtos.Add(new BestPerformersDTO
                {
                    Id = bestperformer.Id,
                    Employee = bestperformer.Employee?.Name,
                    Frequency = bestperformer.Frequency,
                    Client = bestperformer.Client?.Name,
                    Project = bestperformer.Project?.ProjectName,
                    IsActive = bestperformer.IsActive,
                    CreatedBy = bestperformer.CreatedBy,
                    CreatedDate = bestperformer.CreatedDate,
                    UpdatedBy = bestperformer.UpdatedBy,
                    UpdatedDate = bestperformer.UpdatedDate
                });
            }

            return bestperformersDtos;
        }


        public async Task<BestPerformersDTO> Get(string id)
        {
            var bestperformer = await _context.TblBestPerformers
                .Include(e => e.Employee)
                .Include(e => e.Client)
                .Include(e => e.Project)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (bestperformer == null)
                return null;

            return new BestPerformersDTO
            {
                Id = bestperformer.Id,
                Employee = bestperformer.Employee?.Name,
                Frequency = bestperformer.Frequency,
                Client = bestperformer.Client?.Name,
                Project = bestperformer.Project?.ProjectName,
                IsActive = bestperformer.IsActive,
                CreatedBy = bestperformer.CreatedBy,
                CreatedDate = bestperformer.CreatedDate,
                UpdatedBy = bestperformer.UpdatedBy,
                UpdatedDate = bestperformer.UpdatedDate
            };
        }

        // Add a new Best Performer asynchronously
        public async Task<BestPerformersDTO> Add(BestPerformersDTO bestPerformersDTO)
        {
            var bestperformer = new BestPerformers();
            // Check if the BestPerfomrer Id already exists
            var existingBestPerformer = await _context.TblBestPerformers
                .FirstOrDefaultAsync(t => t.Id == bestPerformersDTO.Id);

            if (existingBestPerformer != null)
                throw new ArgumentException("A Employee with the same ID already exists.");

            var employee = new BestPerformers();

            if (employee == null)
            {
                throw new ArgumentException($"Invalid employee ID, Please enter a valid employee ID");
            }
            else
            {
                // If no employee ID is provided, allow null for the EmployeeID
                employee.EmployeeID = null;
            }

            var client = new BestPerformers();

            if (client == null)
            {
                throw new ArgumentException($"Invalid client ID, Please enter a valid client ID");
            }
            else
            {
                // If no client is provided, allow null for the ClientID
                client.ClientID = null;
            }

            var project = new BestPerformers();

            if (project == null)
            {
                throw new ArgumentException($"Invalid project ID, Please enter a valid project ID");
            }
            else
            {
                // If no project is provided, allow null for the ProjectID
                project.ProjectID = null;
            }
            var EmployeeName = _httpContextAccessor.HttpContext?.User?.FindFirst("EmployeeName")?.Value;

            bestperformer.Frequency = bestPerformersDTO.Frequency;
            bestperformer.IsActive = true;
            bestperformer.CreatedBy = EmployeeName;
            bestperformer.CreatedDate = DateTime.Now;

            _context.TblBestPerformers.Add(bestperformer);
            await _context.SaveChangesAsync();

            bestPerformersDTO.Id = bestperformer.Id;
            return bestPerformersDTO;
        }

        public async Task<BestPerformersDTO> Update(BestPerformersDTO bestPerformersDTO)
        {
            var userName = _httpContextAccessor.HttpContext?.User?.FindFirst("EmployeeName")?.Value;

            // Check if the Bestperformer ID already exists
            var existingbestperformer = await _context.TblBestPerformers
                .FirstOrDefaultAsync(t => t.Id == bestPerformersDTO.Id);
            if (existingbestperformer != null)
                throw new ArgumentException("A BestPerformers with the same ID already exists.");

            var bestPerformer = await _context.TblBestPerformers.FindAsync(bestPerformersDTO.Id);

            if (bestPerformer == null)
                throw new KeyNotFoundException("BestPerformer not found");

            // Check if a employee ID is provided
            if (!string.IsNullOrWhiteSpace(bestPerformersDTO.Id))
            {
                // Look for the employee ID in the database
                var employee = await _context.TblEmployee
                    .FirstOrDefaultAsync(d => d.Id == bestPerformersDTO.Employee);

                // If department is not found, throw an exception
                if (employee == null)
                {
                    throw new ArgumentException("Invalid employee ID, Please enter a valid employee ID.");
                }
                bestPerformer.EmployeeID = employee.Id; // Update the DepartmentId
            }
            else
            {
                // Allow DepartmentId to be null if no department name is provided
                bestPerformer.EmployeeID = null;
            }

            bestPerformer.Frequency = bestPerformersDTO.Frequency;
            bestPerformer.UpdatedBy = userName;
            bestPerformer.UpdatedDate = DateTime.Now;

            _context.Entry(bestPerformer).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return bestPerformersDTO;
        }

        public async Task<bool> Delete(string id)
        {
            // Check if the BestPerformers exists
            var existingData = await _repository.Get(id);
            if (existingData == null)
            {
                throw new ArgumentException($"BestPerformers with ID {id} not found.");
            }
            existingData.IsActive = false; // Soft delete
            await _repository.Update(existingData); // Save changes
            return true;
        }

        // Helper method to map from BestPerformers to BestPerformersDTO
        private BestPerformersDTO MapToDTO(BestPerformers bestPerformer)
        {
            if (bestPerformer == null) return null;

            return new BestPerformersDTO
            {
                Id = bestPerformer.Id,
                Employee = bestPerformer.EmployeeID,
                Frequency = bestPerformer.Frequency,
                Client = bestPerformer.ClientID,
                Project = bestPerformer.Id
            };
        }

        // Helper method to map from IEnumerable<BestPerformers> to IEnumerable<BestPerformersDTO>
        private IEnumerable<BestPerformersDTO> MapToDTO(IEnumerable<BestPerformers> bestPerformers)
        {
            if (bestPerformers == null) return null;

            var bestPerformersDTOs = new List<BestPerformersDTO>();
            foreach (var performer in bestPerformers)
            {
                bestPerformersDTOs.Add(MapToDTO(performer));
            }
            return bestPerformersDTOs;
        }

        public Task<BestPerformersDTO> GetByName(string name)
        {
            throw new NotImplementedException();
        }
    }
}
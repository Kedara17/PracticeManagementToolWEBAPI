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

        public BestPerformersService(IRepository<BestPerformers> repository, DataBaseContext context)
        {
            _repository = repository;
            _context = context;
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
                    EmployeeID = bestperformer.Employee?.Name,
                    Frequency = bestperformer.Frequency,
                    ClientID = bestperformer.Client?.Name,
                    ProjectID = bestperformer.Project?.ProjectName,
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
                EmployeeID = bestperformer.Employee?.Name,
                Frequency = bestperformer.Frequency,
                ClientID = bestperformer.Client?.Name,
                ProjectID = bestperformer.Project?.ProjectName,
                IsActive = bestperformer.IsActive,
                CreatedBy = bestperformer.CreatedBy,
                CreatedDate = bestperformer.CreatedDate,
                UpdatedBy = bestperformer.UpdatedBy,
                UpdatedDate = bestperformer.UpdatedDate
            };
        }

        public async Task<BestPerformersDTO> Update(BestPerformersDTO bestPerformersDTO)
        {
            if (bestPerformersDTO == null)
            {
                throw new ArgumentNullException(nameof(bestPerformersDTO), "Input data is null");
            }

            // Retrieve the existing entity from the database to ensure it exists
            var existingEntity = await _context.TblBestPerformers
                .FirstOrDefaultAsync(e => e.Id == bestPerformersDTO.Id);

            if (existingEntity == null)
            {
                throw new ArgumentException($"BestPerformer with ID {bestPerformersDTO.Id} not found.");
            }

            // Update the fields of the existing entity based on the DTO
            existingEntity.EmployeeID = bestPerformersDTO.EmployeeID;
            existingEntity.Frequency = bestPerformersDTO.Frequency;
            existingEntity.ClientID = bestPerformersDTO.ClientID;
            existingEntity.ProjectID = bestPerformersDTO.ProjectID;
            existingEntity.UpdatedBy = bestPerformersDTO.UpdatedBy;
            existingEntity.UpdatedDate = DateTime.UtcNow;  // Update timestamp or other fields if needed

            try
            {
                // Call the repository to perform the update
                await _repository.Update(existingEntity);

                // Return the updated DTO after successful update
                return bestPerformersDTO;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // Handle concurrency issues (e.g., retry logic or informing the user)
                throw new InvalidOperationException("Concurrency conflict occurred. The record may have been modified by another user.", ex);
            }

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

        // Add a new Best Performer asynchronously
        public async Task<BestPerformersDTO> Add(BestPerformersDTO bestPerformersDTO)
        {
            if (bestPerformersDTO == null)
            {
                return null;
            }

            var bestPerformer = new BestPerformers
            {
                EmployeeID = bestPerformersDTO.EmployeeID,
                Frequency = bestPerformersDTO.Frequency,
                ClientID = bestPerformersDTO.ClientID,
                ProjectID = bestPerformersDTO.ProjectID
            };

            var createdBestPerformer = await _repository.Create(bestPerformer);
            return MapToDTO(createdBestPerformer);
        }


        // Helper method to map from BestPerformers to BestPerformersDTO
        private BestPerformersDTO MapToDTO(BestPerformers bestPerformer)
        {
            if (bestPerformer == null) return null;

            return new BestPerformersDTO
            {
                Id = bestPerformer.Id,
                EmployeeID = bestPerformer.EmployeeID,
                Frequency = bestPerformer.Frequency,
                ClientID = bestPerformer.ClientID,
                ProjectID = bestPerformer.ProjectID
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
    }
}
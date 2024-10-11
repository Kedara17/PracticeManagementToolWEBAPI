using DataServices.Models;
using DataServices.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BestPerformersAPI.Services
{
    public class BestPerformersService : IBestPerformersService
    {
        private readonly IRepository<BestPerformers> _repository;
        private readonly ILogger<BestPerformersService> _logger;

        public BestPerformersService(IRepository<BestPerformers> repository, ILogger<BestPerformersService> logger)
        {
            _repository = repository;
            _logger = logger;
        }
        // Fetch all Best Performers asynchronously
        public async Task<IEnumerable<BestPerformersDTO>> GetAll()
        {
            var bestPerformers = await _repository.GetAll();
            return MapToDTO(bestPerformers);
        }

        // Fetch a single Best Performer by ID asynchronously
        public async Task<BestPerformersDTO> Get(string id)
        {
            var bestPerformer = await _repository.Get(id);
            if (bestPerformer == null)
            {
                _logger.LogWarning($"Best performer with id: {id} was not found.");
                return null;
            }
            return MapToDTO(bestPerformer);
        }

        // Add a new Best Performer asynchronously
        public async Task<BestPerformersDTO> Add(BestPerformersDTO bestPerformersDTO)
        {
            if (bestPerformersDTO == null)
            {
                _logger.LogWarning("Attempted to add a null BestPerformersDTO.");
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

        // Update an existing Best Performer asynchronously
        public async Task<BestPerformersDTO> Update(BestPerformersDTO bestPerformersDTO)
        {
            if (bestPerformersDTO == null)
            {
                _logger.LogWarning("Attempted to update a null BestPerformersDTO.");
                return null;
            }

            var bestPerformer = new BestPerformers
            {
                //Id = bestPerformersDTO.Id,
                EmployeeID = bestPerformersDTO.EmployeeID,
                Frequency = bestPerformersDTO.Frequency,
                ClientID = bestPerformersDTO.ClientID,
                ProjectID = bestPerformersDTO.ProjectID
            };

            var updatedBestPerformer = await _repository.Update(bestPerformer);
            return MapToDTO(updatedBestPerformer);
        }

        // Delete a Best Performer by ID asynchronously
        public async Task<bool> Delete(string id)
        {
            return await _repository.Delete(id);
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
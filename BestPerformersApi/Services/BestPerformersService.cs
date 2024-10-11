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
            try
            {
                var bestPerformers = await _repository.GetAll();
                return MapToDTO(bestPerformers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all best performers.");
                return new List<BestPerformersDTO>(); // Return an empty list or consider rethrowing an exception.
            }
        }

        // Fetch a single Best Performer by ID asynchronously
        public async Task<BestPerformersDTO> Get(string id)
        {
            try
            {
                var bestPerformer = await _repository.Get(id);
                if (bestPerformer == null)
                {
                    _logger.LogWarning($"Best performer with id: {id} was not found.");
                    return null; // Consider throwing a custom exception for not found.
                }
                return MapToDTO(bestPerformer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching best performer with id: {id}.");
                return null; // Consider returning a default object or rethrowing the exception.
            }
        }

        // Add a new Best Performer asynchronously
        public async Task<BestPerformersDTO> Add(BestPerformersDTO bestPerformersDTO)
        {
            if (bestPerformersDTO == null)
            {
                _logger.LogWarning("Attempted to add a null BestPerformersDTO.");
                return null;
            }

            try
            {
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a new best performer.");
                return null; // Handle the response according to your application's needs.
            }
        }

        // Update an existing Best Performer asynchronously
        public async Task<BestPerformersDTO> Update(BestPerformersDTO bestPerformersDTO)
        {
            if (bestPerformersDTO == null)
            {
                _logger.LogWarning("Attempted to update a null BestPerformersDTO.");
                return null;
            }

            try
            {
                var bestPerformer = new BestPerformers
                {
                    Id = bestPerformersDTO.Id,
                    EmployeeID = bestPerformersDTO.EmployeeID,
                    Frequency = bestPerformersDTO.Frequency,
                    ClientID = bestPerformersDTO.ClientID,
                    ProjectID = bestPerformersDTO.ProjectID
                };

                var updatedBestPerformer = await _repository.Update(bestPerformer);
                return MapToDTO(updatedBestPerformer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the best performer.");
                return null; // Handle the response according to your application's needs.
            }
        }

        // Delete a Best Performer by ID asynchronously
        public async Task<bool> Delete(string id)
        {
            try
            {
                return await _repository.Delete(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting the best performer with id: {id}.");
                return false; // Indicate failure or handle accordingly.
            }
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
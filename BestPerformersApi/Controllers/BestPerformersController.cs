using BestPerformersAPI.Services;
using DataServices.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BestPerformersAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BestPerformersController : ControllerBase
    {
        private readonly IBestPerformersService _bestPerformersServices;
        private readonly ILogger<BestPerformersController> _logger;

        public BestPerformersController(IBestPerformersService bestPerformersServices, ILogger<BestPerformersController> logger)
        {
            _bestPerformersServices = bestPerformersServices;
            _logger = logger;
        }

        // GET: api/bestperformers
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<BestPerformersDTO>>> GetAll()
        {
            _logger.LogInformation("Fetching all best performers");
            var bestPerformers = await _bestPerformersServices.GetAll();
            _logger.LogInformation("Fetched {Count} best performers", bestPerformers.Count());

            return Ok(bestPerformers);
        }

        // GET: api/bestperformers/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<BestPerformersDTO>> Get(string id)
        {
            _logger.LogInformation("Fetching best performer with ID {Id}", id);
            var bestPerformer = await _bestPerformersServices.Get(id);

            if (bestPerformer == null)
            {
                _logger.LogWarning("Best performer with ID {Id} not found", id);
                return NotFound();
            }

            return Ok(bestPerformer);
        }

        // POST: api/bestperformers
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<BestPerformersDTO>> Add([FromBody] BestPerformersDTO bestPerformersDTO)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for BestPerformersDTO");
                return BadRequest(ModelState); // Return the validation errors
            }

            if (bestPerformersDTO == null)
            {
                _logger.LogWarning("Attempt to add null BestPerformersDTO");
                return BadRequest("BestPerformersDTO cannot be null.");
            }

            _logger.LogInformation("Adding a new best performer");
            var createdBestPerformer = await _bestPerformersServices.Add(bestPerformersDTO);

            _logger.LogInformation("Best performer with ID {Id} created", createdBestPerformer.Id);
            return CreatedAtAction(nameof(Get), new { id = createdBestPerformer.Id }, createdBestPerformer);
        }

        // PUT: api/bestperformers/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<BestPerformersDTO>> Update([FromBody] BestPerformersDTO bestPerformersDTO)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for BestPerformersDTO");
                return BadRequest(ModelState); // Return the validation errors
            }

            if (bestPerformersDTO == null)
            {
                _logger.LogWarning("Attempt to update with null BestPerformersDTO");
                return BadRequest("BestPerformersDTO cannot be null.");
            }

            _logger.LogInformation("Updating best performer with ID {Id}", bestPerformersDTO.Id);
            var updatedBestPerformer = await _bestPerformersServices.Update(bestPerformersDTO);

            if (updatedBestPerformer == null)
            {
                _logger.LogWarning("Best performer with ID {Id} not found for update", bestPerformersDTO.Id);
                return NotFound();
            }

            return Ok(updatedBestPerformer);
        }

        // DELETE: api/bestperformers/{id}
        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            _logger.LogInformation("Deleting best performer with ID {Id}", id);
            var result = await _bestPerformersServices.Delete(id);

            if (!result)
            {
                _logger.LogWarning("Best performer with ID {Id} not found for deletion", id);
                return NotFound();
            }

            _logger.LogInformation("Best performer with ID {Id} deleted", id);
            return NoContent();
        }
    }
}

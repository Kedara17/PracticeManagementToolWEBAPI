using BestPerformersAPI.Services;
using DataServices.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Security.Claims;
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

        //// PUT: api/bestperformers/{id}
        //[HttpPut("{id}")]
        //[Authorize(Roles = "Admin")]
        //public async Task<ActionResult<BestPerformersDTO>> Update(string id, [FromBody] BestPerformersDTO bestPerformersDTO)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        _logger.LogWarning("Invalid model state for BestPerformersDTO");
        //        return BadRequest(ModelState); // Return the validation errors
        //    }

        //    if (bestPerformersDTO == null)
        //    {
        //        _logger.LogWarning("Attempt to update with null BestPerformersDTO");
        //        return BadRequest("BestPerformersDTO cannot be null.");
        //    }

        //    _logger.LogInformation("Updating best performer with ID {Id}", bestPerformersDTO.Id);
        //    var updatedBestPerformer = await _bestPerformersServices.Update(bestPerformersDTO);

        //    if (updatedBestPerformer == null)
        //    {
        //        _logger.LogWarning("Best performer with ID {Id} not found for update", bestPerformersDTO.Id);
        //        return NotFound();
        //    }

        //    var userRole = User.FindFirstValue(ClaimTypes.Role);  // Extract user role from token/claims

        //    // Fetch the existing blog to determine the current status
        //    var existingBestPerformer = await _bestPerformersServices.Get(id);
        //    if (existingBestPerformer == null)
        //    {
        //        return NotFound("Blog not found");
        //    }

        //    // If trying to reactivate the blog (set IsActive = true)
        //    if (!existingBestPerformer.IsActive && bestPerformersDTO.IsActive)
        //    {
        //        if (userRole != "Admin")
        //        {
        //            return Forbid("Only admins can reactivate a blog");
        //        }
        //    }

        //    try
        //    {
        //        var updatedBestPerformers = await _bestPerformersServices.Update(bestPerformersDTO, userRole);
        //        return Ok(updatedBestPerformers);
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        _logger.LogWarning(ex.Message);
        //        return BadRequest(ex.Message);
        //    }

        //    //return Ok(updatedBestPerformer);
        //}

        //// PUT: api/bestperformers/{id}
        //[HttpPut("{id}")]
        //[Authorize(Roles = "Admin")]
        //public async Task<ActionResult<BestPerformersDTO>> Update(string id, [FromBody] BestPerformersDTO bestPerformersDTO)
        //{
        //    if (bestPerformersDTO == null)
        //    {
        //        _logger.LogWarning("Attempt to update with null BestPerformersDTO");
        //        return BadRequest("BestPerformersDTO cannot be null.");
        //    }

        //    if (!ModelState.IsValid)
        //    {
        //        _logger.LogWarning("Invalid model state for BestPerformersDTO");
        //        return BadRequest(ModelState); // Return the validation errors
        //    }

        //    var existingBestPerformer = await _bestPerformersServices.Get(id);
        //    if (existingBestPerformer == null)
        //    {
        //        _logger.LogWarning("Best performer with ID {Id} not found for update", id);
        //        return NotFound("Best performer not found");
        //    }

        //    var userRole = User.FindFirstValue(ClaimTypes.Role); // Extract user role from token/claims

        //    // Check if the update is trying to reactivate an inactive best performer
        //    if (!existingBestPerformer.IsActive && bestPerformersDTO.IsActive && userRole != "Admin")
        //    {
        //        return Forbid("Only admins can reactivate a best performer");
        //    }

        //    try
        //    {
        //        var updatedBestPerformer = await _bestPerformersServices.Update(bestPerformersDTO, userRole);
        //        _logger.LogInformation("Successfully updated best performer with ID {Id}", updatedBestPerformer.Id);
        //        return Ok(updatedBestPerformer);
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        _logger.LogWarning(ex.Message);
        //        return BadRequest(ex.Message);
        //    }
        //}


        // PUT: api/bestperformers/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<BestPerformersDTO>> Update(string id, [FromBody] BestPerformersDTO bestPerformersDTO)
        {
            if (bestPerformersDTO == null)
            {
                _logger.LogWarning("Attempt to update with null BestPerformersDTO");
                return BadRequest("BestPerformersDTO cannot be null.");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for BestPerformersDTO");
                return BadRequest(ModelState); // Return the validation errors
            }

            var userRole = User.FindFirstValue(ClaimTypes.Role); // Extract user role from token/claims

            // Fetch the existing best performer to determine the current status
            var existingBestPerformer = await _bestPerformersServices.Get(id);
            if (existingBestPerformer == null)
            {
                _logger.LogWarning("Best performer with ID {Id} not found for update", id);
                return NotFound("Best performer not found");
            }

            // Check if the update is trying to reactivate an inactive best performer
            if (!existingBestPerformer.IsActive && bestPerformersDTO.IsActive && userRole != "Admin")
            {
                return Forbid("Only admins can reactivate a best performer");
            }

            try
            {
                // Perform the update with the userRole parameter
                var updatedBestPerformers = await _bestPerformersServices.Update(bestPerformersDTO, userRole);
                _logger.LogInformation("Successfully updated best performer with ID {Id}", updatedBestPerformers.Id);
                return Ok(updatedBestPerformers);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return BadRequest(ex.Message);
            }
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

//SuccessStoriesController


//New one


using Microsoft.AspNetCore.Mvc;
using SuccessStoriesApi.Services;
using DataServices.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SuccessStoriesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SuccessStoriesController : ControllerBase
    {
        private readonly ISuccessStoriesService _service;
        private readonly ILogger<SuccessStoriesController> _logger;

        public SuccessStoriesController(ISuccessStoriesService service, ILogger<SuccessStoriesController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<SuccessStoriesDTO>>> GetAll()
        {
            _logger.LogInformation("Fetching all SuccessStories");
            var data = await _service.GetAll();

            if (User.IsInRole("Admin"))
            {
                return Ok(data); // Admin sees all data
            }
            else
            {
                return Ok(data.Where(d => d.IsActive)); // Non-admins see only active data
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<SuccessStoriesDTO>> Get(string id)
        {
            _logger.LogInformation("Fetching SuccessStories with id: {Id}", id);
            var data = await _service.Get(id);

            if (data == null)
            {
                _logger.LogWarning("SuccessStories with id: {Id} not found", id);
                return NotFound();
            }

            if (User.IsInRole("Admin") || data.IsActive)
            {
                return Ok(data);
            }
            else if (data.IsActive)
            {
                return Ok(data); // Non-admins can only see active data
            }
            else
            {
                _logger.LogWarning("SuccessStories with id: {Id} is inactive and user does not have admin privileges", id);
                return Forbid();
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<SuccessStoriesDTO>> Add([FromBody] SuccessStoriesDTO successStories)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for creating SuccessStories");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Creating a new SuccessStories");



            try
            {
                var created = await _service.Add(successStories);
                return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Director, Project Manager, Team Lead")]
        public async Task<IActionResult> Update(string id, [FromBody] SuccessStoriesDTO successStories)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for updating SuccessStories");
                return BadRequest(ModelState);
            }

            if (id != successStories.Id)
            {
                _logger.LogWarning("ID mismatch: {Id} does not match with the request body", id);
                return BadRequest("ID mismatch.");
            }

            try
            {
                await _service.Update(successStories);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(ex.Message);
            }
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            _logger.LogInformation("Deleting SuccessStories with id: {Id}", id);
            var success = await _service.Delete(id);

            if (!success)
            {
                _logger.LogWarning("SuccessStories with id: {Id} not found", id);
                return NotFound();
            }

            return NoContent();
        }
    }
}

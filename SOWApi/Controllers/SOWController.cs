using DataServices.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SOWApi.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SOWApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SOWController : ControllerBase
    {
        private readonly ISOWService _Service;
        private readonly ILogger<SOWController> _logger;

        public SOWController(ISOWService Service, ILogger<SOWController> logger)
        {
            _Service = Service;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Director, Project Manager, Team Lead, Team Member")]
        public async Task<ActionResult<IEnumerable<SOWDTO>>> GetAll()
        {
            _logger.LogInformation("Fetching all SOW");
            var sow = await _Service.GetAll();
            if (User.IsInRole("Admin"))
            {
                return Ok(sow); // Admin can see all data
            }
            else
            {
                return Ok(sow.Where(d => d.IsActive)); // Non-admins see only active data
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Director, Project Manager, Team Lead, Team Member")]
        public async Task<ActionResult<SOWDTO>> Get(string id)
        {
            _logger.LogInformation("Fetching sow with id: {Id}", id);
            var sow = await _Service.Get(id);

            if (sow == null)
            {
                _logger.LogWarning("sow with id: {Id} not found", id);
                return NotFound();
            }

            // Check if the logged-in user has the "Admin" role
            if (User.IsInRole("Admin"))
            {
                return Ok(sow); // Admin can see both active and inactive 
            }
            else if (sow.IsActive)
            {
                return Ok(sow); // Non-admins can only see active data
            }
            else
            {
                _logger.LogWarning("Department with id: {Id} is inactive and user does not have admin privileges", id);
                return Forbid(); // Return forbidden if non-admin tries to access an inactive 
            }
        }


        [HttpPost]
        [Authorize(Roles = "Admin, Director, Project Manager")]
        public async Task<ActionResult<SOWDTO>> Add([FromBody] SOWDTO _object)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for creating sow");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Creating a new sow");
            try
            {
                var created = await _Service.Add(_object);
                return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return BadRequest(ex.Message);
            }
        }


        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Director, Project Manager")]
        public async Task<IActionResult> Update(string id, [FromBody] SOWDTO _object)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for updating sow");
                return BadRequest(ModelState);
            }

            if (id != _object.Id)
            {
                _logger.LogWarning("sow id: {Id} does not match with the id in the request body", id);
                return BadRequest("sow ID mismatch.");
            }

            try
            {
                await _Service.Update(_object);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return BadRequest(ex.Message);
            }
            return NoContent();
        }

        [HttpPatch("{id}/toggle-active")]
        [Authorize(Roles = "Admin, Director, Project Manager")]
        public async Task<IActionResult> Delete(string id)
        {
            _logger.LogInformation("Toggling active status for SOW with id: {Id}", id);

            try
            {
                // Retrieve the current sow record
                var sow = await _Service.Get(id);
                if (sow == null)
                {
                    return NotFound("SOW not found");
                }

                // Role-based access: only Admins can activate, all specified roles can deactivate
                if (sow.IsActive)
                {
                    // Active to Inactive: Allow Admin, Director, Project Manager
                    if (!User.IsInRole("Admin") && !User.IsInRole("Director") && !User.IsInRole("Project Manager"))
                    {
                        return Forbid("Only Admins, Directors, and Project Managers can deactivate a sow.");
                    }
                }
                else
                {
                    // Inactive to Active: Allow only Admin
                    if (!User.IsInRole("Admin"))
                    {
                        return Forbid("Only Admins can activate a sow.");
                    }
                }

                // Toggle the active status
                bool newStatus = await _Service.Delete(id);
                _logger.LogInformation("SOW with id: {Id} is now {Status}", id, newStatus ? "Active" : "Inactive");

                return Ok(new { id, IsActive = newStatus });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling active status for SOW with id: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

    }
}
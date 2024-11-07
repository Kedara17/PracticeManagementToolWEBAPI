using DataServices.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ClientApi.Services;
using ClientServices.Services;
using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Authorization;

namespace ClientApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _Service;
        private readonly ILogger<ClientController> _logger;

        public ClientController(IClientService service, ILogger<ClientController> logger)
        {
            _Service = service;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Director, Project Manager, Team Lead, Team Member")]
        public async Task<ActionResult<IEnumerable<ClientDTO>>> GetAll()
        {
            _logger.LogInformation("Fetching all");
            var data = await _Service.GetAll();
            if (User.IsInRole("Admin"))
            {
                return Ok(data); // Admin can see all data
            }
            else
            {
                return Ok(data.Where(d => d.IsActive)); // Non-admins see only active data
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Director, Project Manager, Team Lead, Team Member")]
        public async Task<ActionResult<ClientDTO>> Get(string id)
        {
            _logger.LogInformation("Fetching with id: {Id}", id);
            var data = await _Service.Get(id);

            if (data == null)
            {
                _logger.LogWarning("with id: {Id} not found", id);
                return NotFound();
            }

            // Check if the logged-in user has the "Admin" role
            if (User.IsInRole("Admin"))
            {
                return Ok(data); // Admin can see both active and inactive 
            }
            else if (data.IsActive)
            {
                return Ok(data); // Non-admins can only see active data
            }
            else
            {
                _logger.LogWarning("Client with id: {Id} is inactive and user does not have admin privileges", id);
                return Forbid(); // Return forbidden if non-admin tries to access an inactive 
            }
        }


        [HttpPost]
        [Authorize(Roles = "Admin, Director, Project Manager")]
        public async Task<ActionResult<ClientDTO>> Create([FromBody] ClientDTO _object)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for creating sow");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Creating a new Client");
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
        public async Task<IActionResult> Update(string id, [FromBody] ClientDTO _object)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for updating ");
                return BadRequest(ModelState);
            }

            if (id != _object.Id)
            {
                _logger.LogWarning("id: {Id} does not match with the id in the request body", id);
                return BadRequest("ID mismatch.");
            }

            _logger.LogInformation("Updating  with id: {Id}", id);

            try
            {
                await _Service.Update(_object);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(ex.Message);
            }

            return NoContent();

        }

        [HttpPatch("{id}/toggle-active")]
        [Authorize(Roles = "Admin, Director, Project Manager")]
        public async Task<IActionResult> Delete(string id)
        {
            _logger.LogInformation("Toggling active status for Client with id: {Id}", id);

            try
            {
                // Retrieve the current client record
                var client = await _Service.Get(id);
                if (client == null)
                {
                    return NotFound("Client not found");
                }

                // Role-based access: only Admins can activate, all specified roles can deactivate
                if (client.IsActive)
                {
                    // Active to Inactive: Allow Admin, Director, Project Manager
                    if (!User.IsInRole("Admin") && !User.IsInRole("Director") && !User.IsInRole("Project Manager"))
                    {
                        return Forbid("Only Admins, Directors, and Project Managers can deactivate a client.");
                    }
                }
                else
                {
                    // Inactive to Active: Allow only Admin
                    if (!User.IsInRole("Admin"))
                    {
                        return Forbid("Only Admins can activate a client.");
                    }
                }

                // Toggle the active status
                bool newStatus = await _Service.Delete(id);
                _logger.LogInformation("Client with id: {Id} is now {Status}", id, newStatus ? "Active" : "Inactive");

                return Ok(new { id, IsActive = newStatus });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling active status for Client with id: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }



    }
}
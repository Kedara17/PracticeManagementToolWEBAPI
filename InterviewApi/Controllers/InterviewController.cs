using DataServices.Models;
using InterviewApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InterviewApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InterviewController : Controller
    {
        private readonly IInterviewService _service;
        private readonly ILogger<InterviewController> _logger;

        public InterviewController(IInterviewService Service, ILogger<InterviewController> logger)
        {
            _service = Service;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Director, Project Manager, Team Lead, Team Member")]
        public async Task<ActionResult<IEnumerable<Interviews>>> GetAll()
        {
            _logger.LogInformation("Fetching all employeeTechnology");
            var data = await _service.GetAll();
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
        public async Task<ActionResult<Interviews>> Get(string id)
        {
            _logger.LogInformation("Fetching employee with id: {Id}", id);
            var data = await _service.Get(id);

            if (data == null)
            {
                _logger.LogWarning("Employee with id: {Id} not found", id);
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
                _logger.LogWarning("Interview with id: {Id} is inactive and user does not have admin privileges", id);
                return Forbid(); // Return forbidden if non-admin tries to access an inactive 
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Director, Project Manager")]
        public async Task<IActionResult> Add(InterviewsDTO _object)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for creating Interview");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Creating a new Interview");
            try
            {
                var created = await _service.Add(_object);
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
        public async Task<IActionResult> Update(string id, [FromBody] InterviewsDTO _object)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for updating interview");
                return BadRequest(ModelState);
            }

            if (id != _object.Id)
            {
                _logger.LogWarning("Interview id: {Id} does not match with the id in the request body", id);
                return BadRequest("Interview ID mismatch.");
            }

            _logger.LogInformation("Updating interview with id: {Id}", id);
            try
            {
                await _service.Update(_object);
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
            _logger.LogInformation("Toggling active status for Interview with id: {Id}", id);

            try
            {
                // Retrieve the current interview record
                var interview = await _service.Get(id);
                if (interview == null)
                {
                    return NotFound("Interview not found");
                }

                // Role-based access: only Admins can activate, all specified roles can deactivate
                if (interview.IsActive)
                {
                    // Active to Inactive: Allow Admin, Director, Project Manager
                    if (!User.IsInRole("Admin") && !User.IsInRole("Director") && !User.IsInRole("Project Manager"))
                    {
                        return Forbid("Only Admins, Directors, and Project Managers can deactivate a interview.");
                    }
                }
                else
                {
                    // Inactive to Active: Allow only Admin
                    if (!User.IsInRole("Admin"))
                    {
                        return Forbid("Only Admins can activate a interview.");
                    }
                }

                // Toggle the active status
                bool newStatus = await _service.Delete(id);
                _logger.LogInformation("Interview with id: {Id} is now {Status}", id, newStatus ? "Active" : "Inactive");

                return Ok(new { id, IsActive = newStatus });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling active status for Interview with id: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }


    }
}
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

        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin, Director, Project Manager")]
        public async Task<IActionResult> Delete(string id)
        {
            _logger.LogInformation("Deleting interview with id: {Id}", id);
            var success = await _service.Delete(id);

            if (!success)
            {
                _logger.LogWarning("Interview with id: {Id} not found", id);
                return NotFound();
            }

            return NoContent();
        }

    }
}
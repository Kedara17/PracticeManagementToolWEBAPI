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

        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin, Director, Project Manager")]
        public async Task<IActionResult> Delete(string id)
        {
            _logger.LogInformation("Deleting sow with id: {Id}", id);
            var success = await _Service.Delete(id);

            if (!success)
            {
                _logger.LogWarning("sow with id: {Id} not found", id);
                return NotFound();
            }

            return NoContent();
        }
    }
}
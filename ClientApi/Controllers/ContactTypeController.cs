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
    public class ContactTypeController : ControllerBase
    {
        private readonly IContactTypeService _Service;
        private readonly ILogger<ContactTypeController> _logger;

        public ContactTypeController(IContactTypeService service, ILogger<ContactTypeController> logger)
        {
            _Service = service;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Director, Project Manager, Team Lead, Team Member")]
        public async Task<ActionResult<IEnumerable<ContactType>>> GetAll()
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
        public async Task<ActionResult<ContactType>> Get(string id)
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
                _logger.LogWarning("ContactType with id: {Id} is inactive and user does not have admin privileges", id);
                return Forbid(); // Return forbidden if non-admin tries to access an inactive 
            }
        }


        [HttpPost]
        [Authorize(Roles = "Admin, Director, Project Manager")]
        public async Task<ActionResult<ContactType>> Add([FromBody] ContactType _object)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for creating sow");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Creating a new sow");
            var data = new ContactType
            {
                TypeName = _object.TypeName,
                IsActive = _object.IsActive,
                CreatedBy = _object.CreatedBy,
                CreatedDate = _object.CreatedDate,
                UpdatedBy = _object.UpdatedBy,
                UpdatedDate = _object.UpdatedDate
            };

            var created = await _Service.Add(data);
            return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
        }


        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Director, Project Manager")]
        public async Task<IActionResult> Update(string id, [FromBody] ContactType _object)
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

            var existingData = await _Service.Get(id);
            if (existingData == null)
            {
                _logger.LogWarning("with id: {Id} not found", id);
                return NotFound();
            }

            _logger.LogInformation("Updating  with id: {Id}", id);

            // Update properties from DTO
            existingData.TypeName = _object.TypeName;
            existingData.IsActive = _object.IsActive;
            existingData.UpdatedBy = _object.UpdatedBy;
            existingData.UpdatedDate = _object.UpdatedDate;

            await _Service.Update(existingData);

            return Ok(existingData);
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin, Director, Project Manager")]
        public async Task<IActionResult> Delete(string id)
        {
            _logger.LogInformation("Deleting with id: {Id}", id);
            var success = await _Service.Delete(id);

            if (!success)
            {
                _logger.LogWarning("with id: {Id} not found", id);
                return NotFound();
            }

            return NoContent();
        }
    }
}


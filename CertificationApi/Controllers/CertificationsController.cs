
//Controller


//neww one

using DataServices.Models;
using CertificationsApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CertificationsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CertificationsController : ControllerBase
    {
        private readonly ICertificationsService _service;
        private readonly ILogger<CertificationsController> _logger;

        public CertificationsController(ICertificationsService service, ILogger<CertificationsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Director, Project Manager, Team Lead, Team Member")]
        public async Task<ActionResult<IEnumerable<CertificationsDTO>>> GetAll()
        {
            _logger.LogInformation("Fetching all certifications");
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
        [Authorize(Roles = "Admin, Director, Project Manager, Team Lead, Team Member")]
        public async Task<ActionResult<CertificationsDTO>> Get(string id)
        {
            _logger.LogInformation("Fetching certification with id: {Id}", id);
            var data = await _service.Get(id);

            if (data == null)
            {
                _logger.LogWarning("Certification with id: {Id} not found", id);
                return NotFound();
            }

            if (User.IsInRole("Admin"))
            {
                return Ok(data); // Admin sees all
            }
            else if (data.IsActive)
            {
                return Ok(data); // Non-admins see only active data
            }
            else
            {
                _logger.LogWarning("Certification with id: {Id} is inactive and user does not have admin privileges", id);
                return Forbid();
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Director, Project Manager")]
        public async Task<ActionResult<CertificationsDTO>> Add([FromBody] CertificationsDTO certification)
        {
            Console.WriteLine("Entered");
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for creating certification");
                Console.WriteLine("Invalid model state for creating certification");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Creating a new certification");

            try
            {
                var created = await _service.Add(certification);
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
        public async Task<IActionResult> Update(string id, [FromBody] CertificationsDTO certification)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for updating certification");
                return BadRequest(ModelState);
            }

            if (id != certification.Id)
            {
                _logger.LogWarning("ID mismatch: {Id} does not match with the request body", id);
                return BadRequest("ID mismatch.");
            }

            try
            {
                await _service.Update(certification);
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
            _logger.LogInformation("Deleting certification with id: {Id}", id);
            var success = await _service.Delete(id);

            if (!success)
            {
                _logger.LogWarning("Certification with id: {Id} not found", id);
                return NotFound();
            }

            return NoContent();
        }
    }
}

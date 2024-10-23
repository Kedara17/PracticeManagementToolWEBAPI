using DataServices.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using POCAPI.Services;

namespace POCAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class POCController : ControllerBase
    {
        private readonly IPOCService _Service;
        private readonly ILogger<POCController> _logger;

        public POCController(IPOCService Service, ILogger<POCController> logger)
        {
            _Service = Service;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Director, Project Manager, Team Lead, Team Member")]
        public async Task<ActionResult<IEnumerable<POCDTO>>> GetAll()
        {
            _logger.LogInformation("Fetching all ");
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
        public async Task<ActionResult<POCDTO>> Get(string id)
        {
            _logger.LogInformation("Fetching with id: {Id}", id);
            var pocs = await _Service.Get(id);

            if (pocs == null)
            {
                _logger.LogWarning("with id: {Id} not found", id);
                return NotFound();
            }

            // Check if the logged-in user has the "Admin" role
            if (User.IsInRole("Admin"))
            {
                return Ok(pocs); // Admin can see both active and inactive 
            }
            else if (pocs.IsActive)
            {
                return Ok(pocs); // Non-admins can only see active data
            }
            else
            {
                _logger.LogWarning("Blogs with id: {Id} is inactive and user does not have admin privileges", id);
                return Forbid(); // Return forbidden if non-admin tries to access an inactive 
            }
        }


        [HttpPost]
        [Authorize(Roles = "Admin, Director, Project Manager")]
        public async Task<ActionResult<POCDTO>> Add([FromBody] POCDTO pocDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for creating pocs");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Creating a new POC");
            try
            {
                var created = await _Service.Add(pocDto);
                return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("uploadFile")]
        [Authorize(Roles = "Admin, Director, Project Manager")]
        public async Task<IActionResult> UploadFile(POCDocumentDTO pocDocument)
        {
            try
            {
                var filePath = await _Service.UploadFileAsync(pocDocument);
                return Ok(new { message = "Your File is uploaded successfully.", path = filePath });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Director, Project Manager")]
        public async Task<IActionResult> Update(string id, [FromBody] POCDTO _object)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for updating Blogs");
                return BadRequest(ModelState);
            }

            if (id != _object.Id)
            {
                _logger.LogWarning("Blogs id: {Id} does not match with the id in the request body", id);
                return BadRequest("Blogs ID mismatch.");
            }

            _logger.LogInformation("Updating Blogs with id: {Id}", id);
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
            _logger.LogInformation("Deleting Blogs with id: {Id}", id);
            var success = await _Service.Delete(id);

            if (!success)
            {
                _logger.LogWarning("Blogs with id: {Id} not found", id);
                return NotFound();
            }

            return NoContent();
        }
    }

}

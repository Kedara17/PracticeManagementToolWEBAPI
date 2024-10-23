using DataServices.Data;
using DataServices.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewLeadApi.Services;

namespace NewLeadApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewLeadEnquiryController : ControllerBase
    {
        private readonly INewLeadEnquiryService _service;
        private readonly ILogger<NewLeadEnquiryController> _logger;
        private readonly DataBaseContext _context;
        public NewLeadEnquiryController(INewLeadEnquiryService service, ILogger<NewLeadEnquiryController> logger, DataBaseContext context)
        {
            _service = service;
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Director, Project Manager, Team Lead, Team Member")]
        public async Task<ActionResult<IEnumerable<NewLeadEnquiryDTO>>> GetAll()
        {
            _logger.LogInformation("Fetching all new lead enquiries");
            var newLeadEnquiries = await _service.GetAll();
            return Ok(newLeadEnquiries);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Director, Project Manager, Team Lead, Team Member")]
        public async Task<ActionResult<NewLeadEnquiryDTO>> Get(string id)
        {
            _logger.LogInformation("Fetching new lead enquiry with id: {Id}", id);
            var newLeadEnquiry = await _service.Get(id);

            if (newLeadEnquiry == null)
            {
                _logger.LogWarning("New lead enquiry with id: {Id} not found", id);
                return NotFound();
            }

            return Ok(newLeadEnquiry);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Director, Project Manager")]
        public async Task<IActionResult> Add(NewLeadEnquiryDTO dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for creating new lead enquiry");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Creating a new lead enquiry");
            try
            {
                var created = await _service.Add(dto);
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
        public async Task<IActionResult> UploadFile(NewLeadEnquiryProfileDTO newLeadEnquiryProfile)
        {
            try
            {
                var filePath = await _service.UploadFileAsync(newLeadEnquiryProfile);
                return Ok(new { message = "Your File is uploaded successfully.", path = filePath });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }


        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Director, Project Manager, Team Lead")]
        public async Task<IActionResult> Update(string id, [FromBody] NewLeadEnquiryDTO dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for updating new lead enquiry");
                return BadRequest(ModelState);
            }

            if (id != dto.Id)
            {
                _logger.LogWarning("New lead enquiry id: {Id} does not match with the id in the request body", id);
                return BadRequest("New lead enquiry ID mismatch.");
            }

            _logger.LogInformation("Updating new lead enquiry with id: {Id}", id);
            try
            {
                await _service.Update(dto);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return BadRequest(ex.Message);
            }
            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            _logger.LogInformation("Deleting with id: {Id}", id);
            var success = await _service.Delete(id);

            if (!success)
            {
                _logger.LogWarning("with id: {Id} not found", id);
                return NotFound();
            }

            return NoContent();
        }
    }
}

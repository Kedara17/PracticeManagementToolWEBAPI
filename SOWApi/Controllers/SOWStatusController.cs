using DataServices.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SOWApi.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SOWAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SOWStatusController : ControllerBase
    {
        private readonly ISOWStatusService _sowStatusService;
        private readonly ILogger<SOWStatusController> _logger;

        public SOWStatusController(ISOWStatusService sowStatusService, ILogger<SOWStatusController> logger)
        {
            _sowStatusService = sowStatusService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Director, Project Manager, Team Lead, Team Member")]
        public async Task<ActionResult<IEnumerable<SOWStatus>>> GetAll()
        {
            _logger.LogInformation("Fetching all SOWStatus data");
            var sowStatus = await _sowStatusService.GetAll();
            if (User.IsInRole("Admin"))
            {
                return Ok(sowStatus); // Admin can see all data
            }
            else
            {
                return Ok(sowStatus.Where(d => d.IsActive)); // Non-admins see only active data
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Director, Project Manager, Team Lead, Team Member")]
        public async Task<ActionResult<SOWStatus>> Get(string id)
        {
            _logger.LogInformation("Fetching sowStatus with id: {Id}", id);
            var sowStatus = await _sowStatusService.Get(id);

            if (sowStatus == null)
            {
                _logger.LogWarning("sowStatus with id: {Id} not found", id);
                return NotFound();
            }

            // Check if the logged-in user has the "Admin" role
            if (User.IsInRole("Admin"))
            {
                return Ok(sowStatus); // Admin can see both active and inactive 
            }
            else if (sowStatus.IsActive)
            {
                return Ok(sowStatus); // Non-admins can only see active data
            }
            else
            {
                _logger.LogWarning("Department with id: {Id} is inactive and user does not have admin privileges", id);
                return Forbid(); // Return forbidden if non-admin tries to access an inactive 
            }
        }


        [HttpPost]
        [Authorize(Roles = "Admin, Director, Project Manager")]
        public async Task<ActionResult<SOWStatus>> Add([FromBody] SOWStatusDTO swoStatusDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for creating sowStatus");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Creating a new sowtatus");
            var sowStatus = new SOWStatus
            {
                Status = swoStatusDto.Status,
                IsActive = swoStatusDto.IsActive,
                CreatedBy = swoStatusDto.CreatedBy,
                CreatedDate = swoStatusDto.CreatedDate,
                UpdatedBy = swoStatusDto.UpdatedBy,
                UpdatedDate = swoStatusDto.UpdatedDate
            };

            var created = await _sowStatusService.Add(sowStatus);
            return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
        }


        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Director, Project Manager")]
        public async Task<IActionResult> Update(string id, [FromBody] SOWStatusDTO swoStatusDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for updating sowStatus");
                return BadRequest(ModelState);
            }

            if (id != swoStatusDto.Id)
            {
                _logger.LogWarning("sowStatus id: {Id} does not match with the id in the request body", id);
                return BadRequest("sowStatus ID mismatch.");
            }

            var existingsow = await _sowStatusService.Get(id);
            if (existingsow == null)
            {
                _logger.LogWarning("sowStatus with id: {Id} not found", id);
                return NotFound();
            }

            _logger.LogInformation("Updating sowStatus with id: {Id}", id);

            // Update properties from DTO
            existingsow.Status = swoStatusDto.Status;
            existingsow.IsActive = swoStatusDto.IsActive;
            existingsow.UpdatedBy = swoStatusDto.UpdatedBy;
            existingsow.UpdatedDate = swoStatusDto.UpdatedDate;

            await _sowStatusService.Update(existingsow);

            return Ok(existingsow);
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin, Director, Project Manager")]
        public async Task<IActionResult> Delete(string id)
        {
            _logger.LogInformation("Deleting sowStatus with id: {Id}", id);
            var success = await _sowStatusService.Delete(id);

            if (!success)
            {
                _logger.LogWarning("sowStatus with id: {Id} not found", id);
                return NotFound();
            }

            return NoContent();
        }
    }
}

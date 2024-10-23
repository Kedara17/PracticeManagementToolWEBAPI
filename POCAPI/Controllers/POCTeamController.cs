using DataServices.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using POCAPI.Services;

namespace POCAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class POCTeamController : ControllerBase
    {
        private readonly IPOCTeamService _Service;
        private readonly ILogger<POCTeamController> _logger;

        public POCTeamController(IPOCTeamService Service, ILogger<POCTeamController> logger)
        {
            _Service = Service;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Director, Project Manager, Team Lead, Team Member")]
        public async Task<ActionResult<IEnumerable<POCTeamDTO>>> GetAll()
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
        public async Task<ActionResult<POCTeamDTO>> Get(string id)
        {
            _logger.LogInformation("Fetching with id: {Id}", id);
            var pocTeams = await _Service.Get(id);

            if (pocTeams == null)
            {
                _logger.LogWarning("with id: {Id} not found", id);
                return NotFound();
            }

            // Check if the logged-in user has the "Admin" role
            if (User.IsInRole("Admin"))
            {
                return Ok(pocTeams); // Admin can see both active and inactive 
            }
            else if (pocTeams.IsActive)
            {
                return Ok(pocTeams); // Non-admins can only see active data
            }
            else
            {
                _logger.LogWarning("POCTeam with id: {Id} is inactive and user does not have admin privileges", id);
                return Forbid(); // Return forbidden if non-admin tries to access an inactive 
            }
        }


        [HttpPost]
        [Authorize(Roles = "Admin, Director, Project Manager")]
        public async Task<ActionResult<POCTeamDTO>> Add([FromBody] POCTeamDTO _object)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for creating POCTeam");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Creating a new POCTeam");
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
        public async Task<IActionResult> Update(string id, [FromBody] POCTeamDTO _object)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for updating Blogs");
                return BadRequest(ModelState);
            }

            if (id != _object.Id)
            {
                _logger.LogWarning("POCTeam id: {Id} does not match with the id in the request body", id);
                return BadRequest("POCTeam ID mismatch.");
            }

            _logger.LogInformation("Updating POCTeams with id: {Id}", id);
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
            _logger.LogInformation("Deleting POCTeam with id: {Id}", id);
            var success = await _Service.Delete(id);

            if (!success)
            {
                _logger.LogWarning("POCTeams with id: {Id} not found", id);
                return NotFound();
            }

            return NoContent();
        }
    }
}

using DataServices.Models;
using DesignationApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DesignationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DesignationController : ControllerBase
    {
        private readonly IDesignationService _Service;
        private readonly ILogger<DesignationController> _logger;

        public DesignationController(IDesignationService service, ILogger<DesignationController> logger)
        {
            _Service = service;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<DesignationDTO>>> GetAll()
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
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DesignationDTO>> Get(string id)
        {
            _logger.LogInformation("Fetching with id: {Id}", id);
            var data = await _Service.Get(id);

            if (data == null)
            {
                _logger.LogWarning("Designation with id: {Id} not found", id);
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
                _logger.LogWarning("Designation with id: {Id} is inactive and user does not have admin privileges", id);
                return Forbid(); // Return forbidden if non-admin tries to access an inactive 
            }
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DesignationDTO>> Add([FromBody] DesignationCreateDTO createDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for creating");
                return BadRequest(ModelState);
            }
            // Check if designation name is unique
            var existingDesignation = await _Service.GetByName(createDto.Name);
            if (existingDesignation != null)
            {
                _logger.LogWarning("Designation with name '{Name}' already exists", createDto.Name);
                return BadRequest($"Designation with name '{createDto.Name}' already exists.");
            }

            _logger.LogInformation("Creating a new Designation");

            try
            {
                var designationDto = new DesignationDTO { Name = createDto.Name };
                var created = await _Service.Add(designationDto);
                return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return BadRequest(ex.Message);
            }
        }


        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(string id, [FromBody] DesignationUpdateDTO updateDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for updating ");
                return BadRequest(ModelState);
            }

            if (id != updateDto.Id)
            {
                _logger.LogWarning("id: {Id} does not match with the id in the request body", id);
                return BadRequest("ID mismatch.");
            }

            // Retrieve the department by ID
            var existingDesignation= await _Service.Get(id);

            if (existingDesignation == null)
            {
                _logger.LogWarning("Department with id: {Id} not found", id);
                return NotFound();
            }

            // Only admins can reactivate inactive records
            if (!existingDesignation.IsActive && !User.IsInRole("Admin"))
            {
                _logger.LogWarning("User without admin privileges attempted to reactivate department with id: {Id}", id);
                return Forbid();
            }

            // Check if the updated name is unique (excluding the current department)
            var designationByName = await _Service.GetByName(updateDto.Name);
            if (designationByName != null && designationByName.Id != id)
            {
                _logger.LogWarning("Designation with name '{Name}' already exists", updateDto.Name);
                return BadRequest($"Designation with name '{updateDto.Name}' already exists.");
            }

            try
            {
                var designationDto = new DesignationDTO { Id = id, Name = updateDto.Name, IsActive = updateDto.IsActive };
                await _Service.Update(designationDto);
                //// Update department (including IsActive state)
                //existingDesignation.Name = updateDto.Name;
                //existingDesignation.IsActive = updateDto.IsActive; // Admin can change the active state
                //await _Service.Update(existingDesignation);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(ex.Message);
            }

            return NoContent();
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin")]
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

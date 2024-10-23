using DataServices.Models;
using DepartmentApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Metrics;

namespace DepartmentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _Service;
        private readonly ILogger<DepartmentController> _logger;

        public DepartmentController(IDepartmentService service, ILogger<DepartmentController> logger)
        {
            _Service = service;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<DepartmentDTO>>> GetAll()
        {
            _logger.LogInformation("Fetching all departments");
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
        public async Task<ActionResult<DepartmentDTO>> Get(string id)
        {
            _logger.LogInformation("Fetching department with id: {Id}", id);
            var data = await _Service.Get(id);

            if (data == null)
            {
                _logger.LogWarning("Department with id: {Id} not found", id);
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
                _logger.LogWarning("Department with id: {Id} is inactive and user does not have admin privileges", id);
                return Forbid(); // Return forbidden if non-admin tries to access an inactive 
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DepartmentDTO>> Add([FromBody] DepartmentCreateDTO createDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for creating department");
                return BadRequest(ModelState);
            }

            // Check if department name is unique
            var existingDepartment = await _Service.GetByName(createDto.Name);
            if (existingDepartment != null)
            {
                _logger.LogWarning("Department with name '{Name}' already exists", createDto.Name);
                return BadRequest($"Department with name '{createDto.Name}' already exists.");
            }

            _logger.LogInformation("Creating a new department");

            try
            {
                var departmentDto = new DepartmentDTO { Name = createDto.Name }; // Create a new DTO instance for the service
                var created = await _Service.Add(departmentDto);
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
        public async Task<IActionResult> Update(string id, [FromBody] DepartmentUpdateDTO updateDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for updating department");
                return BadRequest(ModelState);
            }

            // Check if the ID in the route matches the ID in the body
            if (id != updateDto.Id)
            {
                _logger.LogWarning("Department id: {Id} does not match with the id in the request body", id);
                return BadRequest("ID mismatch.");
            }

            // Check if the updated name is unique (excluding the current department)
            var existingDepartment = await _Service.GetByName(updateDto.Name);
            if (existingDepartment != null && existingDepartment.Id != id)
            {
                _logger.LogWarning("Department with name '{Name}' already exists", updateDto.Name);
                return BadRequest($"Department with name '{updateDto.Name}' already exists.");
            }

            try
            {
                // Map the updateDto back to the original DepartmentDTO
                var departmentDto = new DepartmentDTO { Id = id, Name = updateDto.Name };
                await _Service.Update(departmentDto);
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


using DataServices.Data;
using DataServices.Models;
using EmployeeApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EmployeeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly ILogger<EmployeeController> _logger;
        private readonly DataBaseContext _context;

        public EmployeeController(IEmployeeService employeeService, ILogger<EmployeeController> logger, DataBaseContext context)
        {
            _employeeService = employeeService;
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Director, Project Manager, Team Lead, Team Member")]
        public async Task<ActionResult<IEnumerable<EmployeeDTO>>> GetAll()
        {
            _logger.LogInformation("Fetching all employees");
            var employees = await _employeeService.GetAll();
            if (User.IsInRole("Admin"))
            {
                return Ok(employees); // Admin can see all data
            }
            else
            {
                return Ok(employees.Where(d => d.IsActive)); // Non-admins see only active data
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Director, Project Manager, Team Lead, Team Member")]
        public async Task<ActionResult<EmployeeDTO>> Get(string id)
        {
            _logger.LogInformation("Fetching employee with id: {Id}", id);
            var employee = await _employeeService.Get(id);

            if (employee == null)
            {
                _logger.LogWarning("Employee with id: {Id} not found", id);
                return NotFound();
            }
            // Check if the logged-in user has the "Admin" role
            if (User.IsInRole("Admin"))
            {
                return Ok(employee); // Admin can see both active and inactive 
            }
            else if (employee.IsActive)
            {
                return Ok(employee); // Non-admins can only see active data
            }
            else
            {
                _logger.LogWarning("Employee with id: {Id} is inactive and user does not have admin privileges", id);
                return Forbid(); // Return forbidden if non-admin tries to access an inactive 
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeDTO employeeDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for creating employee");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Creating a new employee");

            try
            {
                var createdEmployee = await _employeeService.Add(employeeDto);
                return CreatedAtAction(nameof(Get), new { id = createdEmployee.Id }, createdEmployee);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("uploadFile")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UploadFile(EmployeeProfileDTO employeeProfile)
        {
            try
            {
                var filePath = await _employeeService.UploadFileAsync(employeeProfile);
                return Ok(new { message = "Your File is uploaded successfully.", path = filePath });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(string id, [FromBody] EmployeeDTO empDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for updating employee");
                return BadRequest(ModelState);
            }

            if (id != empDto.Id)
            {
                _logger.LogWarning("Employee id: {Id} does not match with the id in the request body", id);
                return BadRequest("Employee ID mismatch.");
            }

            try
            {
                await _employeeService.Update(empDto);
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
            _logger.LogInformation("Deleting employee with id: {Id}", id);
            var success = await _employeeService.Delete(id);

            if (!success)
            {
                _logger.LogWarning("Employee with id: {Id} not found", id);
                return NotFound();
            }

            return NoContent();
        }
    }
}

using DataServices.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BlogsApi.Services;
using Microsoft.AspNetCore.Authorization;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Security.Claims;

namespace BlogsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogsController : ControllerBase
    {
        private readonly IBlogsService _Service;
        private readonly ILogger<BlogsController> _logger;

        public BlogsController(IBlogsService Service, ILogger<BlogsController> logger)
        {
            _Service = Service;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Director, Project Manager, Team Lead, Team Member")]
        public async Task<ActionResult<IEnumerable<BlogsDTO>>> GetAll()
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
        public async Task<ActionResult<BlogsDTO>> Get(string id)
        {
            _logger.LogInformation("Fetching with id: {Id}", id);
            var blogs = await _Service.Get(id);

            if (blogs == null)
            {
                _logger.LogWarning("with id: {Id} not found", id);
                return NotFound();
            }

            // Check if the logged-in user has the "Admin" role
            if (User.IsInRole("Admin"))
            {
                return Ok(blogs); // Admin can see both active and inactive 
            }
            else if (blogs.IsActive)
            {
                return Ok(blogs); // Non-admins can only see active data
            }
            else
            {
                _logger.LogWarning("Blogs with id: {Id} is inactive and user does not have admin privileges", id);
                return Forbid(); // Return forbidden if non-admin tries to access an inactive 
            }
        }


        [HttpPost]
        [Authorize(Roles = "Admin, Director, Project Manager")]
        public async Task<ActionResult<BlogsDTO>> Add([FromBody] BlogsDTO _object)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for creating blogs");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Creating a new Blogs");
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
        [Authorize(Roles = "Admin, Director, Project Manager, Team Lead")]
        public async Task<IActionResult> Update(string id, [FromBody] BlogsDTO _object)
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

        [HttpPatch("{id}/toggle-active")]
        [Authorize(Roles = "Admin, Director, Project Manager")]
        public async Task<IActionResult> Delete(string id)
        {
            _logger.LogInformation("Toggling active status for Blog with id: {Id}", id);

            try
            {
                // Retrieve the current blog record
                var blog = await _Service.Get(id);
                if (blog == null)
                {
                    return NotFound("Blog not found");
                }

                // Role-based access: only Admins can activate, all specified roles can deactivate
                if (blog.IsActive)
                {
                    // Active to Inactive: Allow Admin, Director, Project Manager
                    if (!User.IsInRole("Admin") && !User.IsInRole("Director") && !User.IsInRole("Project Manager"))
                    {
                        return Forbid("Only Admins, Directors, and Project Managers can deactivate a blog.");
                    }
                }
                else
                {
                    // Inactive to Active: Allow only Admin
                    if (!User.IsInRole("Admin"))
                    {
                        return Forbid("Only Admins can activate a blog.");
                    }
                }

                // Toggle the active status
                bool newStatus = await _Service.Delete(id);
                _logger.LogInformation("Blog with id: {Id} is now {Status}", id, newStatus ? "Active" : "Inactive");

                return Ok(new { id, IsActive = newStatus });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling active status for Blog with id: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

    }
}
using DataServices.Models;
using DataServices.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechnologyApi.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TechnologyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TechnologyController : ControllerBase
    {
        private readonly ITechnologyService _technologyService;
        private readonly ILogger<TechnologyController> _logger;

        public TechnologyController(ITechnologyService technologyService, ILogger<TechnologyController> logger)
        {
            _technologyService = technologyService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<Technology>>> GetTechnologies()
        {
            _logger.LogInformation("Fetching all technologies");
            var technologies = await _technologyService.GetAll();
            if (User.IsInRole("Admin"))
            {
                return Ok(technologies); // Admin can see all data
            }
            else
            {
                return Ok(technologies.Where(d => d.IsActive)); // Non-admins see only active data
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Technology>> GetTechnology(string id)
        {
            _logger.LogInformation("Fetching technology with id: {Id}", id);
            var technology = await _technologyService.Get(id);

            if (technology == null)
            {
                _logger.LogWarning("Technology with id: {Id} not found", id);
                return NotFound();
            }
            return Ok(technology);
            // Check if the logged-in user has the "Admin" role
            //if (User.IsInRole("Admin"))
            //{
            //    return Ok(technology); // Admin can see both active and inactive 
            //}
            //else (technology.IsActive)
            //{
            //    return Ok(technology); // Non-admins can only see active data
            //}


        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<TechnologyViewModel>> Create([FromBody] TechnologyViewModel createDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for creating technology");
                return BadRequest(ModelState);
            }
            _logger.LogInformation("Creating a new technology");

            try
            {
                var technologyDto = new TechnologyViewModel { Name = createDto.Name, Department = createDto.Department };
                var createdTechnology = await _technologyService.Add(technologyDto);
                return CreatedAtAction(nameof(GetTechnology), new { id = createdTechnology.Id }, createdTechnology);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateTechnology(string id, [FromBody] TechnologyViewModel updateDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for updating technology");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Updating technology with id: {Id}", id);

            try
            {
                await _technologyService.Update(id, updateDto);
                return Content("Updated Successfully");
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
        public async Task<IActionResult> DeleteTechnology(string id)
        {
            _logger.LogInformation("Deleting technology with id: {Id}", id);

            var result = await _technologyService.Delete(id);

            if (!result)
            {
                _logger.LogWarning("Technology with id: {Id} not found", id);
                return NotFound();
            }

            return NoContent();
        }
    }
}
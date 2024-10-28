//using DataServices.Models;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using NewLeadApi.Services;

//namespace NewLeadApi.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class NewLeadEnquiryTechnologyController : ControllerBase
//    {
//        private readonly INewLeadEnquiryTechnologyService _service;

//        public NewLeadEnquiryTechnologyController(INewLeadEnquiryTechnologyService service)
//        {
//            _service = service;
//        }

//        // GET: api/NewLeadEnquiryTechnology
//        [HttpGet]
//        public async Task<ActionResult<IEnumerable<NewLeadEnquiryTechnologyDTO>>> GetAll()
//        {
//            var technologies = await _service.GetAll();
//            return Ok(technologies);
//        }

//        // GET: api/NewLeadEnquiryTechnology/{id}
//        [HttpGet("{id}")]
//        public async Task<ActionResult<NewLeadEnquiryTechnologyDTO>> Get(string id)
//        {
//            var technology = await _service.Get(id);
//            if (technology == null)
//            {
//                return NotFound();
//            }
//            return Ok(technology);
//        }

//        [HttpPost]
//        public async Task<ActionResult<NewLeadEnquiryTechnologyDTO>> Add([FromBody] NewLeadEnquiryTechnologyDTO dto)
//        {
//            if (!ModelState.IsValid)
//            {
//                return BadRequest(ModelState);
//            }

//            var createdTechnology = await _service.Add(dto);
//            return CreatedAtAction(nameof(Get), new { id = createdTechnology.NewLeadEnquiryID }, createdTechnology);
//        }

//        // Adjust Update method
//        [HttpPut("{id}")]
//        public async Task<ActionResult<NewLeadEnquiryTechnologyDTO>> Update(string id, [FromBody] NewLeadEnquiryTechnologyDTO dto)
//        {
//            if (!ModelState.IsValid)
//            {
//                return BadRequest(ModelState);
//            }

//            if (id != dto.Id)  // Ensure the ID matches
//            {
//                return BadRequest("ID mismatch.");
//            }

//            try
//            {
//                var updatedTechnology = await _service.Update(dto);
//                return Ok(updatedTechnology);
//            }
//            catch (KeyNotFoundException)
//            {
//                return NotFound();
//            }
//        }


//        // DELETE: api/NewLeadEnquiryTechnology/{id}
//        [HttpDelete("{id}")]
//        public async Task<ActionResult<bool>> Delete(string id)
//        {
//            var success = await _service.Delete(id);
//            if (!success)
//            {
//                return NotFound();
//            }
//            return Ok(success);
//        }
//    }
//}

using DataServices.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewLeadApi.Services;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewLeadApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewLeadEnquiryTechnologyController : ControllerBase
    {
        private readonly INewLeadEnquiryTechnologyService _service;
        private readonly ILogger<NewLeadEnquiryTechnologyController> _logger;

        public NewLeadEnquiryTechnologyController(INewLeadEnquiryTechnologyService service, ILogger<NewLeadEnquiryTechnologyController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // GET: api/NewLeadEnquiryTechnology
        [HttpGet]
        [Authorize(Roles = "Admin, Manager, Lead, Team Member")]
        public async Task<ActionResult<IEnumerable<NewLeadEnquiryTechnologyDTO>>> GetAll()
        {
            _logger.LogInformation("Fetching all lead enquiry technologies");
            var technologies = await _service.GetAll();

            if (User.IsInRole("Admin"))
            {
                return Ok(technologies); // Admin can see all data
            }
            else
            {
                return Ok(technologies.Where(d => d.IsActive)); // Non-admins see only active data
            }
        }

        // GET: api/NewLeadEnquiryTechnology/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Manager, Lead, Team Member")]
        public async Task<ActionResult<NewLeadEnquiryTechnologyDTO>> Get(string id)
        {
            _logger.LogInformation("Fetching technology with id: {Id}", id);
            var technology = await _service.Get(id);

            if (technology == null)
            {
                _logger.LogWarning("Technology with id: {Id} not found", id);
                return NotFound();
            }

            if (User.IsInRole("Admin"))
            {
                return Ok(technology); // Admin can see both active and inactive technologies
            }
            else if (technology.IsActive)
            {
                return Ok(technology); // Non-admins can only see active data
            }
            else
            {
                _logger.LogWarning("Technology with id: {Id} is inactive and user does not have admin privileges", id);
                return Forbid(); // Return forbidden if non-admin tries to access an inactive technology
            }
        }

        // POST: api/NewLeadEnquiryTechnology
        [HttpPost]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<ActionResult<NewLeadEnquiryTechnologyDTO>> Add([FromBody] NewLeadEnquiryTechnologyDTO dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for creating technology");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Creating a new technology entry");

            try
            {
                var createdTechnology = await _service.Add(dto);
                return CreatedAtAction(nameof(Get), new { id = createdTechnology.NewLeadEnquiryID }, createdTechnology);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/NewLeadEnquiryTechnology/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Manager, Lead")]
        public async Task<IActionResult> Update(string id, [FromBody] NewLeadEnquiryTechnologyDTO dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for updating technology");
                return BadRequest(ModelState);
            }

            if (id != dto.Id)
            {
                _logger.LogWarning("Technology id: {Id} does not match with the id in the request body", id);
                return BadRequest("Technology ID mismatch.");
            }

            try
            {
                await _service.Update(dto);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(ex.Message);
            }

            return NoContent();
        }

        // DELETE: api/NewLeadEnquiryTechnology/{id}
        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            _logger.LogInformation("Deleting technology with id: {Id}", id);
            var success = await _service.Delete(id);

            if (!success)
            {
                _logger.LogWarning("Technology with id: {Id} not found", id);
                return NotFound();
            }

            return NoContent();
        }
    }
}

using DataServices.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewLeadApi.Services;
using System.Runtime.InteropServices;

namespace NewLeadApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewLeadEnquiryFollowupController : ControllerBase
    {
        private readonly INewLeadEnquiryFollowupService _followupService;

        public NewLeadEnquiryFollowupController(INewLeadEnquiryFollowupService followupService)
        {
            _followupService = followupService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Manager, Lead, Team Member")]
        public async Task<ActionResult<IEnumerable<NewLeadEnquiryFollowupDTO>>> GetAll()
        {
            var followups = await _followupService.GetAll();
            return Ok(followups);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Manager, Lead, Team Member")]
        public async Task<ActionResult<NewLeadEnquiryFollowupDTO>> Get(string id)
        {
            var followup = await _followupService.Get(id);
            if (followup == null)
            {
                return NotFound();
            }

            return Ok(followup);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<ActionResult<NewLeadEnquiryFollowupDTO>> Add([FromBody] NewLeadEnquiryFollowupDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdFollowup = await _followupService.Add(dto);
            return CreatedAtAction(nameof(Get), new { id = createdFollowup.NewLeadEnquiryID }, createdFollowup);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Manager, Lead")]
        public async Task<IActionResult> Update(string id, [FromBody] NewLeadEnquiryFollowupDTO dto)
        {
            if (id != dto.Id)
            {
                return BadRequest("ID mismatch");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _followupService.Update(dto);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _followupService.Delete(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

    }
}
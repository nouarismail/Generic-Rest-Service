using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrdersApp.Data;
using OrdersApp.Models;

namespace OrdersApp.Controllers
{
    [ApiController]
    [Route("api/forms/{formType}")]
    public class FormsController : ControllerBase
    {
        private readonly IFormEntryRepository _repo;

        public FormsController(IFormEntryRepository repo)
        {
            _repo = repo;
        }

        
        public async Task<IActionResult> GetList(
    string formType,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] string? search = null,
    [FromQuery] string? orderStatus = null,
    [FromQuery] string? fulfillmentPriority = null,
    [FromQuery] bool? requiresSignature = null,
    [FromQuery] DateTime? orderDateFrom = null,
    [FromQuery] DateTime? orderDateTo = null)
        {
            var result = await _repo.GetListAsync(
                formType,
                page,
                pageSize,
                search,
                orderStatus,
                fulfillmentPriority,
                requiresSignature,
                orderDateFrom,
                orderDateTo);

            var items = result.Items.Select(e => new
            {
                e.Id,
                e.FormType,
                e.CreatedAt,
                e.UpdatedAt,
                Data = JsonDocument.Parse(e.Data).RootElement
            });

            return Ok(new
            {
                page = result.Page,
                pageSize = result.PageSize,
                totalCount = result.TotalCount,
                search,
                orderStatus,
                fulfillmentPriority,
                requiresSignature,
                orderDateFrom,
                orderDateTo,
                items
            });
        }
        
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetOne(string formType, Guid id)
        {
            var entry = await _repo.GetOneAsync(formType, id);

            if (entry == null)
                return NotFound();

            return Ok(new
            {
                entry.Id,
                entry.FormType,
                entry.CreatedAt,
                entry.UpdatedAt,
                entry.Data
            });
        }

        
        [HttpPost]
        public async Task<IActionResult> Create(
            string formType,
            [FromBody] JsonElement data)
        {
            var entry = await _repo.CreateAsync(formType, data);

            return CreatedAtAction(
                nameof(GetOne),
                new { formType, id = entry.Id },
                new
                {
                    entry.Id,
                    entry.FormType,
                    entry.CreatedAt,
                    entry.Data
                });
        }

        
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(
            string formType,
            Guid id,
            [FromBody] JsonElement data)
        {
            var entry = await _repo.UpdateAsync(formType, id, data);

            if (entry == null)
                return NotFound();

            return Ok(new
            {
                entry.Id,
                entry.FormType,
                entry.CreatedAt,
                entry.UpdatedAt,
                entry.Data
            });
        }

        
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(string formType, Guid id)
        {
            var deleted = await _repo.DeleteAsync(formType, id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }

}

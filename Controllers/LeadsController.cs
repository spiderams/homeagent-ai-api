using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealEstateAIAssistant.Data;
using RealEstateAIAssistant.Dtos;

namespace RealEstateAIAssistant.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeadsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LeadsController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLead(int id)
        {
            var lead = await _context.Leads
                .FirstOrDefaultAsync(x => x.Id == id);

            if (lead == null)
            {
                return NotFound();
            }

            return Ok(lead);
        }
     
        [HttpGet("{id}/messages")]
        public async Task<IActionResult> GetLeadMessages(int id)
        {
            var lead = await _context.Leads
                .FirstOrDefaultAsync(x => x.Id == id);

            if (lead == null)
            {
                return NotFound();
            }

            var messages = await _context.ConversationMessages
                .Where(x => x.SessionId == lead.Phone)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();

            return Ok(messages);
        }
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus( int id,[FromBody] UpdateLeadStatusRequest request)
        {
            var lead = await _context.Leads.FindAsync(id);

            if (lead == null)
                return NotFound();

            lead.Status = request.Status;

            await _context.SaveChangesAsync();

            return Ok(lead);
        }
        [HttpGet("history/{phone}")]
        public async Task<IActionResult> GetHistory(string phone)
        {
            var messages = await _context.ConversationMessages
                .Where(x => x.SessionId == phone)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();

            return Ok(messages);
        }
        [HttpGet]
        public async Task<IActionResult> GetLeads([FromQuery] string userId)
        {
            var leads = await _context.Leads
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return Ok(leads);
        }

    }
}

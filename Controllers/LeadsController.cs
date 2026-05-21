using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealEstateAIAssistant.Data;

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
        [HttpGet]
        public async Task<IActionResult> GetLeads()
        {
            var leads = await _context.Leads
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            return Ok(leads);
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

    }
}

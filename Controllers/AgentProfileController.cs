using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealEstateAIAssistant.Data;
using RealEstateAIAssistant.Dtos;
using RealEstateAIAssistant.Models;

namespace RealEstateAIAssistant.Controllers;

[ApiController]
[Route("api/agent-profile")]
public class AgentProfileController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AgentProfileController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> SaveProfile(
        [FromBody] AgentProfileRequest request)
    {
        var profile = await _context.AgentProfiles
            .FirstOrDefaultAsync(x => x.UserId == request.UserId);

        if (profile == null)
        {
            profile = new AgentProfile
            {
                UserId = request.UserId,
                WhatsAppNumber = request.WhatsAppNumber,
                Name = request.Name
            };

            _context.AgentProfiles.Add(profile);
        }
        else
        {
            profile.WhatsAppNumber = request.WhatsAppNumber;
            profile.Name = request.Name;
        }

        await _context.SaveChangesAsync();

        return Ok(profile);
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetProfile(string userId)
    {
        var profile = await _context.AgentProfiles
            .FirstOrDefaultAsync(x => x.UserId == userId);

        return Ok(profile);
    }
}
using Microsoft.AspNetCore.Mvc;
using Twilio.TwiML;
using RealEstateAIAssistant.Services;
using RealEstateAIAssistant.Data;
using Microsoft.EntityFrameworkCore;

namespace RealEstateAIAssistant.Controllers;

[ApiController]
[Route("api/whatsapp")]
public class WhatsAppController : ControllerBase
{
    private readonly OpenAIService _openAIService;
    private readonly ApplicationDbContext _context;

    public WhatsAppController(
        OpenAIService openAIService,
        ApplicationDbContext context)
    {
        _openAIService = openAIService;
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> ReceiveMessage()
    {
        try
        {
            Console.WriteLine("WHATSAPP HIT");

            var message = Request.Form["Body"].ToString();
            var from = Request.Form["From"].ToString();

            Console.WriteLine($"FROM: {from}");
            Console.WriteLine($"MESSAGE: {message}");

            var cleanFrom = from.Replace("whatsapp:", "");
            Console.WriteLine($"CLEAN FROM: {cleanFrom}");
            

            var agent = await _context.AgentProfiles
                .FirstOrDefaultAsync(x =>
                    x.WhatsAppNumber == cleanFrom ||
                    x.WhatsAppNumber == from
                );
            Console.WriteLine($"AGENT FOUND: {agent?.UserId}");
            var userId = agent?.UserId ?? "whatsapp-unassigned";

            Console.WriteLine($"MAPPED USER ID: {userId}");

            var aiResponse = await _openAIService.AskAI(
                from,
                message,
                userId
            );

            var twiml =
$@"<?xml version=""1.0"" encoding=""UTF-8""?>
<Response>
    <Message>{aiResponse.Reply}</Message>
</Response>";

            Console.WriteLine("TWIML RESPONSE:");
            Console.WriteLine(twiml);

            return new ContentResult
            {
                Content = twiml,
                ContentType = "text/xml",
                StatusCode = 200
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());

            return BadRequest(ex.Message);
        }
    }
}
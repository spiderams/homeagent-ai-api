using Microsoft.AspNetCore.Mvc;
using Twilio.TwiML;
using RealEstateAIAssistant.Services;

namespace RealEstateAIAssistant.Controllers;

[ApiController]
[Route("api/whatsapp")]
public class WhatsAppController : ControllerBase
{
    private readonly OpenAIService _openAIService;

    public WhatsAppController(
        OpenAIService openAIService)
    {
        _openAIService = openAIService;
    }

    [HttpPost]
    public async Task<IActionResult> ReceiveMessage()
    {
        try
        {
            Console.WriteLine("WHATSAPP HIT");

            var message =
                Request.Form["Body"];

            var from =
                Request.Form["From"];

            Console.WriteLine($"FROM: {from}");
            Console.WriteLine($"MESSAGE: {message}");

            var aiResponse =
                await _openAIService.AskAI(
                    from!,
                    message!
                );

            var response =
                new MessagingResponse();

            response.Message(
                aiResponse.Reply
            );

            return Content(
                response.ToString(),
                "application/xml"
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());

            return BadRequest(
                ex.Message
            );
        }
    }
}
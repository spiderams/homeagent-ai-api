using Microsoft.AspNetCore.Mvc;
using Twilio.TwiML;
using RealEstateAIAssistant.Services;

namespace RealEstateAIAssistant.Controllers;

[ApiController]
[Route("api/[controller]")]
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
        var message =
            Request.Form["Body"];

        var from =
            Request.Form["From"];

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
            "text/xml"
        );
    }
}
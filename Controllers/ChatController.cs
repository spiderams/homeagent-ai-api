using Microsoft.AspNetCore.Mvc;
using RealEstateAIAssistant.Dtos;
using RealEstateAIAssistant.Services;

namespace RealEstateAIAssistant.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly OpenAIService _openAIService;

        public ChatController(OpenAIService openAIService)
        {
            _openAIService = openAIService;
        }

        [HttpPost]
        public async Task<IActionResult> Chat([FromBody] ChatRequest request)
        {
            var aiResponse = await _openAIService.AskAI(
                request.SessionId,
                request.Message,
                request.UserId
            );

            return Ok(aiResponse);
        }
    }
}
using Microsoft.EntityFrameworkCore;
using OpenAI.Chat;
using RealEstateAIAssistant.Data;
using RealEstateAIAssistant.Dtos;
using RealEstateAIAssistant.Models;
using System.Text.Json;

namespace RealEstateAIAssistant.Services
{
    public class OpenAIService
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public OpenAIService(
            IConfiguration configuration,
            ApplicationDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public async Task<AIResponse> AskAI(
      string sessionId,
      string userMessage,
      string userId = "whatsapp-public")
        {
            try
            {
                // GET API KEY

                string? apiKey =
                  _configuration["OpenAI:ApiKey"];

                Console.WriteLine("OPENAI API KEY:");
                Console.WriteLine(apiKey);

                if (string.IsNullOrEmpty(apiKey))
                {
                    throw new Exception(
                        "OpenAI API key missing"
                    );
                }

                // OPENAI CLIENT

                var client = new ChatClient(
                    model: "gpt-4o-mini",
                    apiKey: apiKey
                );

                // SAVE USER MESSAGE

                        _context.ConversationMessages.Add(
            new ConversationMessage
            {
                SessionId = sessionId,
                UserId = userId,
                Role = "user",
                Content = userMessage,
                CreatedAt = DateTime.UtcNow
            });
                await _context.SaveChangesAsync();

                // LOAD HISTORY

                var history = await _context
                    .ConversationMessages
                    .Where(x =>
                        x.SessionId == sessionId)
                    .OrderBy(x =>
                        x.CreatedAt)
                    .Take(10)
                    .ToListAsync();

                // BUILD HISTORY TEXT

                string historyText =
                    string.Join(
                        "\n",
                        history.Select(x =>
                            $"{x.Role}: {x.Content}")
                    );

                // AI PROMPT

                string prompt = $$"""
You are an AI real estate assistant.

IMPORTANT RULES:

Lead scoring rules:

- Cold:
  vague conversation
  no budget
  no location

- Warm:
  budget + location provided

- Hot:
  ready to buy
  financing approved
  urgent timeline
  serious investor

Conversation rules:

- Use full conversation context
- Never forget previous information
- Update information dynamically
- Never ask again for known info
- Ask short and natural follow-up questions
- Sound professional and conversational

Current conversation:
{{historyText}}

Return ONLY valid JSON.

Do not use markdown.
Do not explain anything.

Return this exact structure:

{
  "reply": "your response",
  "intent": "Buyer",
  "budget": "250k",
  "location": "Quebec",
  "leadScore": "Cold"
}
""";

                // OPENAI REQUEST

                var result =
                    await client.CompleteChatAsync(
                        prompt
                    );

                // RAW RESPONSE

                string rawResponse =
                    result.Value.Content[0].Text;

                Console.WriteLine(
                    "RAW OPENAI RESPONSE:"
                );

                Console.WriteLine(rawResponse);

                // CLEAN JSON

                string json =
                    rawResponse
                        .Replace("```json", "")
                        .Replace("```", "")
                        .Trim();

                Console.WriteLine(
                    "CLEAN JSON:"
                );

                Console.WriteLine(json);

                // DESERIALIZE

                AIResponse? aiResponse =
                    JsonSerializer.Deserialize<AIResponse>(
                        json,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                // FALLBACK

                if (aiResponse == null)
                {
                    aiResponse = new AIResponse
                    {
                        Reply =
                            "Sorry, I could not process your request.",
                        LeadScore = "Cold"
                    };
                }

                // BETTER LEAD SCORING

                string conversation =
                    historyText.ToLower();

                if (
                    conversation.Contains("financing") ||
                    conversation.Contains("ready to buy") ||
                    conversation.Contains("urgent") ||
                    conversation.Contains("this month") ||
                    conversation.Contains("cash buyer")
                )
                {
                    aiResponse.LeadScore = "Hot";
                }

                // SAVE AI MESSAGE

                _context.ConversationMessages.Add(
      new ConversationMessage
      {
          SessionId = sessionId,
          UserId = userId,
          Role = "assistant",
          Content = aiResponse.Reply,
          CreatedAt = DateTime.UtcNow
      });

                // SAVE OR UPDATE LEAD

                var existingLead = await _context.Leads
    .FirstOrDefaultAsync(x =>
        x.Phone == sessionId &&
        x.UserId == userId);

                if (existingLead == null)
                {
                    var newLead = new Lead
                    {
                        Phone = sessionId,
                        UserId = userId,
                        Intent = aiResponse.Intent,
                        Budget = aiResponse.Budget,
                        Location = aiResponse.Location,
                        LeadScore = aiResponse.LeadScore,
                        Status = "New",
                        Summary = historyText,
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.Leads.Add(newLead);
                }
                else
                {
                    existingLead.Intent =
                        aiResponse.Intent;

                    existingLead.Budget =
                        aiResponse.Budget;

                    existingLead.Location =
                        aiResponse.Location;

                    existingLead.LeadScore =
                        aiResponse.LeadScore;

                    existingLead.Summary =
                        historyText;
                }

                await _context.SaveChangesAsync();

                Console.WriteLine(
                    $"AI RESPONSE: {aiResponse.Reply}"
                );

                return aiResponse;
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "OPENAI SERVICE ERROR:"
                );

                Console.WriteLine(ex.ToString());

                return new AIResponse
                {
                    Reply =
                        "Sorry, an error occurred while processing your request.",
                    LeadScore = "Cold"
                };
            }
        }
    }
}
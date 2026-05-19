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
            string userMessage)
        {
            string apiKey = _configuration["OpenAI:ApiKey"]!;

            var client = new ChatClient(
                model: "gpt-4o-mini",
                apiKey: apiKey
            );

            // SAVE USER MESSAGE

            _context.ConversationMessages.Add(
                new ConversationMessage
                {
                    SessionId = sessionId,
                    Role = "user",
                    Content = userMessage
                });

            await _context.SaveChangesAsync();

            // LOAD HISTORY

            var history = await _context.ConversationMessages
                .Where(x => x.SessionId == sessionId)
                .OrderBy(x => x.CreatedAt)
                .Take(10)
                .ToListAsync();

            // BUILD HISTORY TEXT

            string historyText = string.Join(
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

- Use the full conversation context
- Never forget previous information
- Update information when user provides new values
- If user says "250k", update budget to 250k
- If user says "good view", treat it as a desired feature
- Never ask again for information already provided
- Ask short and natural follow-up questions
- Sound conversational and professional

Current conversation:
{{historyText}}

Return ONLY valid JSON.

Do not use markdown.
Do not explain anything.
Do not write ```json.

Return this exact structure:

{
  "reply": "your response",
  "intent": "Buyer",
  "budget": "250k",
  "location": "Quebec",
  "leadScore": "Cold | Warm | Hot"
}
""";

            var result = await client.CompleteChatAsync(prompt);

            string json = result.Value.Content[0].Text;

            Console.WriteLine("RAW AI RESPONSE:");
            Console.WriteLine(json);

            // CLEAN JSON

            json = json.Replace("```json", "")
                       .Replace("```", "")
                       .Trim();

            Console.WriteLine("CLEAN JSON:");
            Console.WriteLine(json);

            // DESERIALIZE

            var aiResponse = JsonSerializer.Deserialize<AIResponse>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            // FALLBACK

            if (aiResponse == null)
            {
                return new AIResponse
                {
                    Reply = "Sorry, I could not process the request.",
                    LeadScore = "Cold"
                };
            }
            // BETTER LEAD SCORING

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

            // SAVE AI RESPONSE

            _context.ConversationMessages.Add(
                new ConversationMessage
                {
                    SessionId = sessionId,
                    Role = "assistant",
                    Content = aiResponse.Reply
                });

            // SAVE OR UPDATE LEAD

            var existingLead = await _context.Leads
                .FirstOrDefaultAsync(x =>
                    x.Phone == sessionId);

            if (existingLead == null)
            {
                var newLead = new Lead
                {
                    Phone = sessionId,
                    Intent = aiResponse.Intent,
                    Budget = aiResponse.Budget,
                    Location = aiResponse.Location,
                    LeadScore = aiResponse.LeadScore,
                    Summary = historyText
                };

                _context.Leads.Add(newLead);
            }
            else
            {
                existingLead.Intent = aiResponse.Intent;
                existingLead.Budget = aiResponse.Budget;
                existingLead.Location = aiResponse.Location;
                existingLead.LeadScore = aiResponse.LeadScore;
                existingLead.Summary = historyText;
            }

            await _context.SaveChangesAsync();

            return aiResponse;
        }
    }
}
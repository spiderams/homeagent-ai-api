namespace RealEstateAIAssistant.Models
{
    public class AgentProfile
    {
        public int Id { get; set; }

        public string UserId { get; set; } = "";

        public string? WhatsAppNumber { get; set; }

        public string? Name { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
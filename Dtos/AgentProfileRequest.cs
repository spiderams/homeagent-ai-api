namespace RealEstateAIAssistant.Dtos
{
    public class AgentProfileRequest
    {
        public string UserId { get; set; } = "";

        public string WhatsAppNumber { get; set; } = "";

        public string? Name { get; set; }
    }
}
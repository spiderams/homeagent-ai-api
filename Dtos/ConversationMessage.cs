namespace RealEstateAIAssistant.Models
{
    public class ConversationState
    {
        public int Id { get; set; }

        public string SessionId { get; set; } = string.Empty;

        public string? Intent { get; set; }

        public string? Budget { get; set; }

        public string? Location { get; set; }

        public string? PropertyType { get; set; }
        public string? UserId { get; set; }
    }
}
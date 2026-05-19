namespace RealEstateAIAssistant.Models
{
    public class ConversationMessage
    {
        public int Id { get; set; }

        public string SessionId { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;
        // user / assistant

        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
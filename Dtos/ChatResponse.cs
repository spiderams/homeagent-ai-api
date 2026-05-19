namespace RealEstateAIAssistant.Dtos
{
    public class ChatResponse
    {
        public string Reply { get; set; } = string.Empty;
        public string? Intent { get; set; }
        public string? Budget { get; set; }
        public string? Location { get; set; }
        public string? LeadScore { get; set; }
    }
}
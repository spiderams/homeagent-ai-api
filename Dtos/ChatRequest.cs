namespace RealEstateAIAssistant.Dtos
{
    public class ChatRequest
    {
        public string SessionId { get; set; } = "";
        public string Message { get; set; } = "";
        public string UserId { get; set; } = "";
    }
}
namespace RealEstateAIAssistant.Models
{
    public class Lead
    {
        public int Id { get; set; }

        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }

        public string? Intent { get; set; } // Buyer, Seller, Investor
        public string? Budget { get; set; }
        public string? Location { get; set; }
        public string? PropertyType { get; set; }
        public string? Timeline { get; set; }

        public string? LeadScore { get; set; } // Cold, Warm, Hot
        public string? Summary { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? AppointmentDate { get; set; }

        public string? AppointmentTime { get; set; }
        public string? Status { get; set; }
    }
}
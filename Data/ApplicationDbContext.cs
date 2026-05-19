using Microsoft.EntityFrameworkCore;
using RealEstateAIAssistant.Models;

namespace RealEstateAIAssistant.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<ConversationMessage> ConversationMessages
    => Set<ConversationMessage>();
        public DbSet<ConversationState> ConversationState
=> Set<ConversationState>();
        public DbSet<Lead> Leads => Set<Lead>();
    }
}
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstateAIAssistant.Migrations
{
    /// <inheritdoc />
    public partial class AddUserAppointmentStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Leads",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "ConversationState",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ConversationState");
        }
    }
}

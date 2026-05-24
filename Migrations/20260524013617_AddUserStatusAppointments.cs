using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstateAIAssistant.Migrations
{
    /// <inheritdoc />
    public partial class AddUserStatusAppointments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "ConversationMessages",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ConversationMessages");
        }
    }
}

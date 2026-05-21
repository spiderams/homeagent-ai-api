using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstateAIAssistant.Migrations
{
    /// <inheritdoc />
    public partial class AppointmentFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AppointmentDate",
                table: "Leads",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AppointmentTime",
                table: "Leads",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ConversationState",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SessionId = table.Column<string>(type: "TEXT", nullable: false),
                    Intent = table.Column<string>(type: "TEXT", nullable: true),
                    Budget = table.Column<string>(type: "TEXT", nullable: true),
                    Location = table.Column<string>(type: "TEXT", nullable: true),
                    PropertyType = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConversationState", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConversationState");

            migrationBuilder.DropColumn(
                name: "AppointmentDate",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "AppointmentTime",
                table: "Leads");
        }
    }
}

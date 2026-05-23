using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstateAIAssistant.Migrations
{
    /// <inheritdoc />
    public partial class AddLeadStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Leads",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Leads");
        }
    }
}

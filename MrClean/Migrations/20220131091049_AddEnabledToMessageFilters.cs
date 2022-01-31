using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MrClean.Migrations
{
    public partial class AddEnabledToMessageFilters : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Enabled",
                table: "MessageFilters",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Enabled",
                table: "MessageFilters");
        }
    }
}

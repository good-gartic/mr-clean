using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MrClean.Migrations
{
    public partial class MessageFilters : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MessageFilters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Delay = table.Column<long>(type: "bigint", nullable: false),
                    Pattern = table.Column<string>(type: "text", nullable: true),
                    RepostChannelId = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                    ChannelsSpecification = table.Column<string>(type: "text", nullable: true),
                    UsersSpecification = table.Column<string>(type: "text", nullable: true),
                    RolesSpecification = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageFilters", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MessageFilters");
        }
    }
}

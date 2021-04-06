using Microsoft.EntityFrameworkCore.Migrations;

namespace Faforever.Qai.Core.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountLinks",
                columns: table => new
                {
                    DisocrdId = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FafId = table.Column<int>(type: "INTEGER", nullable: false),
                    FafUsername = table.Column<string>(type: "TEXT", nullable: true),
                    DiscordUsername = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountLinks", x => x.DisocrdId);
                });

            migrationBuilder.CreateTable(
                name: "DiscordConfigs",
                columns: table => new
                {
                    GuildId = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Prefix = table.Column<string>(type: "TEXT", nullable: false),
                    RoleWhenLinked = table.Column<ulong>(type: "INTEGER", nullable: true),
                    UserBlacklist = table.Column<string>(type: "TEXT", nullable: false),
                    FafLinks = table.Column<string>(type: "TEXT", nullable: false),
                    Records = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscordConfigs", x => x.GuildId);
                });

            migrationBuilder.CreateTable(
                name: "RelayConfigurations",
                columns: table => new
                {
                    DiscordServer = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DiscordToIRCLinks = table.Column<string>(type: "TEXT", nullable: false),
                    Webhooks = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelayConfigurations", x => x.DiscordServer);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountLinks");

            migrationBuilder.DropTable(
                name: "DiscordConfigs");

            migrationBuilder.DropTable(
                name: "RelayConfigurations");
        }
    }
}

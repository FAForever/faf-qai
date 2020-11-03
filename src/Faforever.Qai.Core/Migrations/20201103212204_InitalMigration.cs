using Microsoft.EntityFrameworkCore.Migrations;

namespace Faforever.Qai.Core.Migrations
{
	public partial class InitalMigration : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "DiscordConfigs",
				columns: table => new
				{
					GuildId = table.Column<ulong>(nullable: false)
						.Annotation("Sqlite:Autoincrement", true),
					Prefix = table.Column<string>(nullable: false),
					UserBlacklist = table.Column<string>(nullable: false),
					FafLinks = table.Column<string>(nullable: false),
					Records = table.Column<string>(nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_DiscordConfigs", x => x.GuildId);
				});

			migrationBuilder.CreateTable(
				name: "RelayConfigurations",
				columns: table => new
				{
					DiscordServer = table.Column<ulong>(nullable: false)
						.Annotation("Sqlite:Autoincrement", true),
					DiscordToIRCLinks = table.Column<string>(nullable: false),
					Webhooks = table.Column<string>(nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_RelayConfigurations", x => x.DiscordServer);
				});
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "DiscordConfigs");

			migrationBuilder.DropTable(
				name: "RelayConfigurations");
		}
	}
}

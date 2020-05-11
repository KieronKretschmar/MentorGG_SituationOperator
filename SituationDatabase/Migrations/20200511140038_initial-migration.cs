using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SituationDatabase.Migrations
{
    public partial class initialmigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EffectiveHeGrenade",
                columns: table => new
                {
                    MatchId = table.Column<long>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Round = table.Column<short>(nullable: false),
                    StartTime = table.Column<int>(nullable: false),
                    SteamId = table.Column<long>(nullable: false),
                    EnemiesHit = table.Column<int>(nullable: false),
                    TotalEnemyDamage = table.Column<int>(nullable: false),
                    TotalTeamDamage = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EffectiveHeGrenade", x => new { x.MatchId, x.Id });
                });

            migrationBuilder.CreateTable(
                name: "SmokeFail",
                columns: table => new
                {
                    MatchId = table.Column<long>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Round = table.Column<short>(nullable: false),
                    StartTime = table.Column<int>(nullable: false),
                    SteamId = table.Column<long>(nullable: false),
                    LineupId = table.Column<int>(nullable: false),
                    LineupName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmokeFail", x => new { x.MatchId, x.Id });
                });

            migrationBuilder.CreateIndex(
                name: "IX_EffectiveHeGrenade_MatchId",
                table: "EffectiveHeGrenade",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_EffectiveHeGrenade_SteamId",
                table: "EffectiveHeGrenade",
                column: "SteamId");

            migrationBuilder.CreateIndex(
                name: "IX_SmokeFail_MatchId",
                table: "SmokeFail",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_SmokeFail_SteamId",
                table: "SmokeFail",
                column: "SteamId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EffectiveHeGrenade");

            migrationBuilder.DropTable(
                name: "SmokeFail");
        }
    }
}

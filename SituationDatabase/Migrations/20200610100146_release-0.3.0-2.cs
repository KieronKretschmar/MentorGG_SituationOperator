using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SituationDatabase.Migrations
{
    public partial class release0302 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HighImpactRound",
                columns: table => new
                {
                    MatchId = table.Column<long>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Round = table.Column<short>(nullable: false),
                    StartTime = table.Column<int>(nullable: false),
                    SteamId = table.Column<long>(nullable: false),
                    Kills = table.Column<int>(nullable: false),
                    DamageDealt = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HighImpactRound", x => new { x.MatchId, x.Id });
                    table.ForeignKey(
                        name: "FK_HighImpactRound_Match_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Match",
                        principalColumn: "MatchId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HighImpactRound_Round_MatchId_Round",
                        columns: x => new { x.MatchId, x.Round },
                        principalTable: "Round",
                        principalColumns: new[] { "MatchId", "Round" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HighImpactRound_PlayerMatch_MatchId_SteamId",
                        columns: x => new { x.MatchId, x.SteamId },
                        principalTable: "PlayerMatch",
                        principalColumns: new[] { "MatchId", "SteamId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HighImpactRound_PlayerRound_MatchId_Round_SteamId",
                        columns: x => new { x.MatchId, x.Round, x.SteamId },
                        principalTable: "PlayerRound",
                        principalColumns: new[] { "MatchId", "Round", "SteamId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MultiKill",
                columns: table => new
                {
                    MatchId = table.Column<long>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Round = table.Column<short>(nullable: false),
                    StartTime = table.Column<int>(nullable: false),
                    SteamId = table.Column<long>(nullable: false),
                    Kills = table.Column<int>(nullable: false),
                    FirstKillWeapon = table.Column<short>(nullable: false),
                    SingleBurst = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MultiKill", x => new { x.MatchId, x.Id });
                    table.ForeignKey(
                        name: "FK_MultiKill_Match_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Match",
                        principalColumn: "MatchId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MultiKill_Round_MatchId_Round",
                        columns: x => new { x.MatchId, x.Round },
                        principalTable: "Round",
                        principalColumns: new[] { "MatchId", "Round" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MultiKill_PlayerMatch_MatchId_SteamId",
                        columns: x => new { x.MatchId, x.SteamId },
                        principalTable: "PlayerMatch",
                        principalColumns: new[] { "MatchId", "SteamId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MultiKill_PlayerRound_MatchId_Round_SteamId",
                        columns: x => new { x.MatchId, x.Round, x.SteamId },
                        principalTable: "PlayerRound",
                        principalColumns: new[] { "MatchId", "Round", "SteamId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HighImpactRound_MatchId",
                table: "HighImpactRound",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_HighImpactRound_SteamId",
                table: "HighImpactRound",
                column: "SteamId");

            migrationBuilder.CreateIndex(
                name: "IX_HighImpactRound_MatchId_SteamId",
                table: "HighImpactRound",
                columns: new[] { "MatchId", "SteamId" });

            migrationBuilder.CreateIndex(
                name: "IX_HighImpactRound_MatchId_Round_SteamId",
                table: "HighImpactRound",
                columns: new[] { "MatchId", "Round", "SteamId" });

            migrationBuilder.CreateIndex(
                name: "IX_MultiKill_MatchId",
                table: "MultiKill",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_MultiKill_SteamId",
                table: "MultiKill",
                column: "SteamId");

            migrationBuilder.CreateIndex(
                name: "IX_MultiKill_MatchId_SteamId",
                table: "MultiKill",
                columns: new[] { "MatchId", "SteamId" });

            migrationBuilder.CreateIndex(
                name: "IX_MultiKill_MatchId_Round_SteamId",
                table: "MultiKill",
                columns: new[] { "MatchId", "Round", "SteamId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HighImpactRound");

            migrationBuilder.DropTable(
                name: "MultiKill");
        }
    }
}

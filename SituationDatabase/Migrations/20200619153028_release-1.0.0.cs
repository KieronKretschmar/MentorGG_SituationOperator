using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SituationDatabase.Migrations
{
    public partial class release100 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CollateralKill",
                columns: table => new
                {
                    MatchId = table.Column<long>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Round = table.Column<short>(nullable: false),
                    StartTime = table.Column<int>(nullable: false),
                    SteamId = table.Column<long>(nullable: false),
                    EnemiesKilled = table.Column<int>(nullable: false),
                    Weapon = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollateralKill", x => new { x.MatchId, x.Id });
                    table.ForeignKey(
                        name: "FK_CollateralKill_Match_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Match",
                        principalColumn: "MatchId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CollateralKill_Round_MatchId_Round",
                        columns: x => new { x.MatchId, x.Round },
                        principalTable: "Round",
                        principalColumns: new[] { "MatchId", "Round" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CollateralKill_PlayerMatch_MatchId_SteamId",
                        columns: x => new { x.MatchId, x.SteamId },
                        principalTable: "PlayerMatch",
                        principalColumns: new[] { "MatchId", "SteamId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CollateralKill_PlayerRound_MatchId_Round_SteamId",
                        columns: x => new { x.MatchId, x.Round, x.SteamId },
                        principalTable: "PlayerRound",
                        principalColumns: new[] { "MatchId", "Round", "SteamId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FlashAssist",
                columns: table => new
                {
                    MatchId = table.Column<long>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Round = table.Column<short>(nullable: false),
                    StartTime = table.Column<int>(nullable: false),
                    SteamId = table.Column<long>(nullable: false),
                    GrenadeId = table.Column<long>(nullable: false),
                    FlashedTeammates = table.Column<int>(nullable: false),
                    TimeFlashedTeammates = table.Column<int>(nullable: false),
                    FlashedTeammatesDeaths = table.Column<int>(nullable: false),
                    TimeFlashedEnemies = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlashAssist", x => new { x.MatchId, x.Id });
                    table.ForeignKey(
                        name: "FK_FlashAssist_Match_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Match",
                        principalColumn: "MatchId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FlashAssist_Round_MatchId_Round",
                        columns: x => new { x.MatchId, x.Round },
                        principalTable: "Round",
                        principalColumns: new[] { "MatchId", "Round" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FlashAssist_PlayerMatch_MatchId_SteamId",
                        columns: x => new { x.MatchId, x.SteamId },
                        principalTable: "PlayerMatch",
                        principalColumns: new[] { "MatchId", "SteamId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FlashAssist_PlayerRound_MatchId_Round_SteamId",
                        columns: x => new { x.MatchId, x.Round, x.SteamId },
                        principalTable: "PlayerRound",
                        principalColumns: new[] { "MatchId", "Round", "SteamId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KillThroughSmoke",
                columns: table => new
                {
                    MatchId = table.Column<long>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Round = table.Column<short>(nullable: false),
                    StartTime = table.Column<int>(nullable: false),
                    SteamId = table.Column<long>(nullable: false),
                    Weapon = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KillThroughSmoke", x => new { x.MatchId, x.Id });
                    table.ForeignKey(
                        name: "FK_KillThroughSmoke_Match_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Match",
                        principalColumn: "MatchId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KillThroughSmoke_Round_MatchId_Round",
                        columns: x => new { x.MatchId, x.Round },
                        principalTable: "Round",
                        principalColumns: new[] { "MatchId", "Round" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KillThroughSmoke_PlayerMatch_MatchId_SteamId",
                        columns: x => new { x.MatchId, x.SteamId },
                        principalTable: "PlayerMatch",
                        principalColumns: new[] { "MatchId", "SteamId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KillThroughSmoke_PlayerRound_MatchId_Round_SteamId",
                        columns: x => new { x.MatchId, x.Round, x.SteamId },
                        principalTable: "PlayerRound",
                        principalColumns: new[] { "MatchId", "Round", "SteamId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MissedTradeKill",
                columns: table => new
                {
                    MatchId = table.Column<long>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Round = table.Column<short>(nullable: false),
                    StartTime = table.Column<int>(nullable: false),
                    SteamId = table.Column<long>(nullable: false),
                    DistanceToVictim = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MissedTradeKill", x => new { x.MatchId, x.Id });
                    table.ForeignKey(
                        name: "FK_MissedTradeKill_Match_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Match",
                        principalColumn: "MatchId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MissedTradeKill_Round_MatchId_Round",
                        columns: x => new { x.MatchId, x.Round },
                        principalTable: "Round",
                        principalColumns: new[] { "MatchId", "Round" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MissedTradeKill_PlayerMatch_MatchId_SteamId",
                        columns: x => new { x.MatchId, x.SteamId },
                        principalTable: "PlayerMatch",
                        principalColumns: new[] { "MatchId", "SteamId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MissedTradeKill_PlayerRound_MatchId_Round_SteamId",
                        columns: x => new { x.MatchId, x.Round, x.SteamId },
                        principalTable: "PlayerRound",
                        principalColumns: new[] { "MatchId", "Round", "SteamId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RankDistribution",
                columns: table => new
                {
                    RankBeforeMatch = table.Column<int>(nullable: false),
                    PlayerRoundCount = table.Column<int>(nullable: false),
                    SituationCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "TradeKill",
                columns: table => new
                {
                    MatchId = table.Column<long>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Round = table.Column<short>(nullable: false),
                    StartTime = table.Column<int>(nullable: false),
                    SteamId = table.Column<long>(nullable: false),
                    TimeBetweenKills = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradeKill", x => new { x.MatchId, x.Id });
                    table.ForeignKey(
                        name: "FK_TradeKill_Match_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Match",
                        principalColumn: "MatchId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TradeKill_Round_MatchId_Round",
                        columns: x => new { x.MatchId, x.Round },
                        principalTable: "Round",
                        principalColumns: new[] { "MatchId", "Round" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TradeKill_PlayerMatch_MatchId_SteamId",
                        columns: x => new { x.MatchId, x.SteamId },
                        principalTable: "PlayerMatch",
                        principalColumns: new[] { "MatchId", "SteamId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TradeKill_PlayerRound_MatchId_Round_SteamId",
                        columns: x => new { x.MatchId, x.Round, x.SteamId },
                        principalTable: "PlayerRound",
                        principalColumns: new[] { "MatchId", "Round", "SteamId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WallBangKill",
                columns: table => new
                {
                    MatchId = table.Column<long>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Round = table.Column<short>(nullable: false),
                    StartTime = table.Column<int>(nullable: false),
                    SteamId = table.Column<long>(nullable: false),
                    Weapon = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WallBangKill", x => new { x.MatchId, x.Id });
                    table.ForeignKey(
                        name: "FK_WallBangKill_Match_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Match",
                        principalColumn: "MatchId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WallBangKill_Round_MatchId_Round",
                        columns: x => new { x.MatchId, x.Round },
                        principalTable: "Round",
                        principalColumns: new[] { "MatchId", "Round" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WallBangKill_PlayerMatch_MatchId_SteamId",
                        columns: x => new { x.MatchId, x.SteamId },
                        principalTable: "PlayerMatch",
                        principalColumns: new[] { "MatchId", "SteamId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WallBangKill_PlayerRound_MatchId_Round_SteamId",
                        columns: x => new { x.MatchId, x.Round, x.SteamId },
                        principalTable: "PlayerRound",
                        principalColumns: new[] { "MatchId", "Round", "SteamId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CollateralKill_MatchId",
                table: "CollateralKill",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_CollateralKill_SteamId",
                table: "CollateralKill",
                column: "SteamId");

            migrationBuilder.CreateIndex(
                name: "IX_CollateralKill_MatchId_SteamId",
                table: "CollateralKill",
                columns: new[] { "MatchId", "SteamId" });

            migrationBuilder.CreateIndex(
                name: "IX_CollateralKill_MatchId_Round_SteamId",
                table: "CollateralKill",
                columns: new[] { "MatchId", "Round", "SteamId" });

            migrationBuilder.CreateIndex(
                name: "IX_FlashAssist_MatchId",
                table: "FlashAssist",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_FlashAssist_SteamId",
                table: "FlashAssist",
                column: "SteamId");

            migrationBuilder.CreateIndex(
                name: "IX_FlashAssist_MatchId_SteamId",
                table: "FlashAssist",
                columns: new[] { "MatchId", "SteamId" });

            migrationBuilder.CreateIndex(
                name: "IX_FlashAssist_MatchId_Round_SteamId",
                table: "FlashAssist",
                columns: new[] { "MatchId", "Round", "SteamId" });

            migrationBuilder.CreateIndex(
                name: "IX_KillThroughSmoke_MatchId",
                table: "KillThroughSmoke",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_KillThroughSmoke_SteamId",
                table: "KillThroughSmoke",
                column: "SteamId");

            migrationBuilder.CreateIndex(
                name: "IX_KillThroughSmoke_MatchId_SteamId",
                table: "KillThroughSmoke",
                columns: new[] { "MatchId", "SteamId" });

            migrationBuilder.CreateIndex(
                name: "IX_KillThroughSmoke_MatchId_Round_SteamId",
                table: "KillThroughSmoke",
                columns: new[] { "MatchId", "Round", "SteamId" });

            migrationBuilder.CreateIndex(
                name: "IX_MissedTradeKill_MatchId",
                table: "MissedTradeKill",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_MissedTradeKill_SteamId",
                table: "MissedTradeKill",
                column: "SteamId");

            migrationBuilder.CreateIndex(
                name: "IX_MissedTradeKill_MatchId_SteamId",
                table: "MissedTradeKill",
                columns: new[] { "MatchId", "SteamId" });

            migrationBuilder.CreateIndex(
                name: "IX_MissedTradeKill_MatchId_Round_SteamId",
                table: "MissedTradeKill",
                columns: new[] { "MatchId", "Round", "SteamId" });

            migrationBuilder.CreateIndex(
                name: "IX_TradeKill_MatchId",
                table: "TradeKill",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_TradeKill_SteamId",
                table: "TradeKill",
                column: "SteamId");

            migrationBuilder.CreateIndex(
                name: "IX_TradeKill_MatchId_SteamId",
                table: "TradeKill",
                columns: new[] { "MatchId", "SteamId" });

            migrationBuilder.CreateIndex(
                name: "IX_TradeKill_MatchId_Round_SteamId",
                table: "TradeKill",
                columns: new[] { "MatchId", "Round", "SteamId" });

            migrationBuilder.CreateIndex(
                name: "IX_WallBangKill_MatchId",
                table: "WallBangKill",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_WallBangKill_SteamId",
                table: "WallBangKill",
                column: "SteamId");

            migrationBuilder.CreateIndex(
                name: "IX_WallBangKill_MatchId_SteamId",
                table: "WallBangKill",
                columns: new[] { "MatchId", "SteamId" });

            migrationBuilder.CreateIndex(
                name: "IX_WallBangKill_MatchId_Round_SteamId",
                table: "WallBangKill",
                columns: new[] { "MatchId", "Round", "SteamId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CollateralKill");

            migrationBuilder.DropTable(
                name: "FlashAssist");

            migrationBuilder.DropTable(
                name: "KillThroughSmoke");

            migrationBuilder.DropTable(
                name: "MissedTradeKill");

            migrationBuilder.DropTable(
                name: "RankDistribution");

            migrationBuilder.DropTable(
                name: "TradeKill");

            migrationBuilder.DropTable(
                name: "WallBangKill");
        }
    }
}

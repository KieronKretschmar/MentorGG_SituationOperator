using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SituationDatabase.Migrations
{
    public partial class addmetatables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Match",
                columns: table => new
                {
                    MatchId = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MatchDate = table.Column<DateTime>(nullable: false),
                    Map = table.Column<string>(nullable: true),
                    WinnerTeam = table.Column<byte>(nullable: false),
                    Score1 = table.Column<short>(nullable: false),
                    Score2 = table.Column<short>(nullable: false),
                    RealScore1 = table.Column<short>(nullable: false),
                    RealScore2 = table.Column<short>(nullable: false),
                    NumRoundsT1 = table.Column<short>(nullable: false),
                    NumRoundsCt1 = table.Column<short>(nullable: false),
                    NumRoundsT2 = table.Column<short>(nullable: false),
                    NumRoundsCt2 = table.Column<short>(nullable: false),
                    BombPlants1 = table.Column<short>(nullable: false),
                    BombPlants2 = table.Column<short>(nullable: false),
                    BombExplodes1 = table.Column<short>(nullable: false),
                    BombExplodes2 = table.Column<short>(nullable: false),
                    BombDefuses1 = table.Column<short>(nullable: false),
                    BombDefuses2 = table.Column<short>(nullable: false),
                    MoneyEarned1 = table.Column<int>(nullable: false),
                    MoneyEarned2 = table.Column<int>(nullable: false),
                    MoneySpent1 = table.Column<int>(nullable: false),
                    MoneySpent2 = table.Column<int>(nullable: false),
                    Source = table.Column<byte>(nullable: false),
                    GameType = table.Column<byte>(nullable: false),
                    AvgRank = table.Column<float>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Match", x => x.MatchId);
                });

            migrationBuilder.CreateTable(
                name: "PlayerMatch",
                columns: table => new
                {
                    MatchId = table.Column<long>(nullable: false),
                    SteamId = table.Column<long>(nullable: false),
                    Team = table.Column<byte>(nullable: false),
                    KillCount = table.Column<short>(nullable: false),
                    AssistCount = table.Column<short>(nullable: false),
                    DeathCount = table.Column<short>(nullable: false),
                    Score = table.Column<short>(nullable: false),
                    Mvps = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerMatch", x => new { x.MatchId, x.SteamId });
                    table.ForeignKey(
                        name: "FK_PlayerMatch_Match_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Match",
                        principalColumn: "MatchId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Round",
                columns: table => new
                {
                    MatchId = table.Column<long>(nullable: false),
                    Round = table.Column<short>(nullable: false),
                    WinnerTeam = table.Column<byte>(nullable: false),
                    OriginalSide = table.Column<bool>(nullable: false),
                    BombPlanted = table.Column<bool>(nullable: false),
                    WinType = table.Column<byte>(nullable: false),
                    RoundTime = table.Column<int>(nullable: false),
                    StartTime = table.Column<int>(nullable: false),
                    EndTime = table.Column<int>(nullable: false),
                    RealEndTime = table.Column<int>(nullable: false),
                    PlayerMatchMatchId = table.Column<long>(nullable: true),
                    PlayerMatchSteamId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Round", x => new { x.MatchId, x.Round });
                    table.ForeignKey(
                        name: "FK_Round_Match_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Match",
                        principalColumn: "MatchId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Round_PlayerMatch_PlayerMatchMatchId_PlayerMatchSteamId",
                        columns: x => new { x.PlayerMatchMatchId, x.PlayerMatchSteamId },
                        principalTable: "PlayerMatch",
                        principalColumns: new[] { "MatchId", "SteamId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlayerRound",
                columns: table => new
                {
                    MatchId = table.Column<long>(nullable: false),
                    RoundNumber = table.Column<short>(nullable: false),
                    SteamId = table.Column<long>(nullable: false),
                    PlayedEquipmentValue = table.Column<int>(nullable: false),
                    MoneyInitial = table.Column<int>(nullable: false),
                    IsCt = table.Column<bool>(nullable: false),
                    ArmorType = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerRound", x => new { x.MatchId, x.RoundNumber, x.SteamId });
                    table.ForeignKey(
                        name: "FK_PlayerRound_Match_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Match",
                        principalColumn: "MatchId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerRound_Round_MatchId_RoundNumber",
                        columns: x => new { x.MatchId, x.RoundNumber },
                        principalTable: "Round",
                        principalColumns: new[] { "MatchId", "Round" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerRound_PlayerMatch_MatchId_SteamId",
                        columns: x => new { x.MatchId, x.SteamId },
                        principalTable: "PlayerMatch",
                        principalColumns: new[] { "MatchId", "SteamId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Match_MatchId",
                table: "Match",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerMatch_MatchId",
                table: "PlayerMatch",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerRound_MatchId",
                table: "PlayerRound",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerRound_MatchId_SteamId",
                table: "PlayerRound",
                columns: new[] { "MatchId", "SteamId" });

            migrationBuilder.CreateIndex(
                name: "IX_Round_MatchId",
                table: "Round",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_Round_PlayerMatchMatchId_PlayerMatchSteamId",
                table: "Round",
                columns: new[] { "PlayerMatchMatchId", "PlayerMatchSteamId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerRound");

            migrationBuilder.DropTable(
                name: "Round");

            migrationBuilder.DropTable(
                name: "PlayerMatch");

            migrationBuilder.DropTable(
                name: "Match");
        }
    }
}

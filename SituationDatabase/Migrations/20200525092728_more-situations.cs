using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SituationDatabase.Migrations
{
    public partial class moresituations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeathInducedBombDrop",
                columns: table => new
                {
                    MatchId = table.Column<long>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Round = table.Column<short>(nullable: false),
                    StartTime = table.Column<int>(nullable: false),
                    SteamId = table.Column<long>(nullable: false),
                    PickedUpAfter = table.Column<int>(nullable: false),
                    TeammatesAlive = table.Column<int>(nullable: false),
                    ClosestTeammateDistance = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeathInducedBombDrop", x => new { x.MatchId, x.Id });
                    table.ForeignKey(
                        name: "FK_DeathInducedBombDrop_Match_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Match",
                        principalColumn: "MatchId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeathInducedBombDrop_Round_MatchId_Round",
                        columns: x => new { x.MatchId, x.Round },
                        principalTable: "Round",
                        principalColumns: new[] { "MatchId", "Round" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeathInducedBombDrop_PlayerMatch_MatchId_SteamId",
                        columns: x => new { x.MatchId, x.SteamId },
                        principalTable: "PlayerMatch",
                        principalColumns: new[] { "MatchId", "SteamId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeathInducedBombDrop_PlayerRound_MatchId_Round_SteamId",
                        columns: x => new { x.MatchId, x.Round, x.SteamId },
                        principalTable: "PlayerRound",
                        principalColumns: new[] { "MatchId", "Round", "SteamId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KillWithOwnFlashAssist",
                columns: table => new
                {
                    MatchId = table.Column<long>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Round = table.Column<short>(nullable: false),
                    StartTime = table.Column<int>(nullable: false),
                    SteamId = table.Column<long>(nullable: false),
                    TimeFlashedAfterDeath = table.Column<int>(nullable: false),
                    TimeBetweenDetonationAndKill = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KillWithOwnFlashAssist", x => new { x.MatchId, x.Id });
                    table.ForeignKey(
                        name: "FK_KillWithOwnFlashAssist_Match_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Match",
                        principalColumn: "MatchId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KillWithOwnFlashAssist_Round_MatchId_Round",
                        columns: x => new { x.MatchId, x.Round },
                        principalTable: "Round",
                        principalColumns: new[] { "MatchId", "Round" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KillWithOwnFlashAssist_PlayerMatch_MatchId_SteamId",
                        columns: x => new { x.MatchId, x.SteamId },
                        principalTable: "PlayerMatch",
                        principalColumns: new[] { "MatchId", "SteamId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KillWithOwnFlashAssist_PlayerRound_MatchId_Round_SteamId",
                        columns: x => new { x.MatchId, x.Round, x.SteamId },
                        principalTable: "PlayerRound",
                        principalColumns: new[] { "MatchId", "Round", "SteamId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PushBeforeSmokeDetonated",
                columns: table => new
                {
                    MatchId = table.Column<long>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Round = table.Column<short>(nullable: false),
                    StartTime = table.Column<int>(nullable: false),
                    SteamId = table.Column<long>(nullable: false),
                    SmokeDetonationTime = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PushBeforeSmokeDetonated", x => new { x.MatchId, x.Id });
                    table.ForeignKey(
                        name: "FK_PushBeforeSmokeDetonated_Match_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Match",
                        principalColumn: "MatchId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PushBeforeSmokeDetonated_Round_MatchId_Round",
                        columns: x => new { x.MatchId, x.Round },
                        principalTable: "Round",
                        principalColumns: new[] { "MatchId", "Round" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PushBeforeSmokeDetonated_PlayerMatch_MatchId_SteamId",
                        columns: x => new { x.MatchId, x.SteamId },
                        principalTable: "PlayerMatch",
                        principalColumns: new[] { "MatchId", "SteamId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PushBeforeSmokeDetonated_PlayerRound_MatchId_Round_SteamId",
                        columns: x => new { x.MatchId, x.Round, x.SteamId },
                        principalTable: "PlayerRound",
                        principalColumns: new[] { "MatchId", "Round", "SteamId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RifleFiredWhileMoving",
                columns: table => new
                {
                    MatchId = table.Column<long>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Round = table.Column<short>(nullable: false),
                    StartTime = table.Column<int>(nullable: false),
                    SteamId = table.Column<long>(nullable: false),
                    Weapon = table.Column<short>(nullable: false),
                    Bullets = table.Column<int>(nullable: false),
                    InaccurateBullets = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RifleFiredWhileMoving", x => new { x.MatchId, x.Id });
                    table.ForeignKey(
                        name: "FK_RifleFiredWhileMoving_Match_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Match",
                        principalColumn: "MatchId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RifleFiredWhileMoving_Round_MatchId_Round",
                        columns: x => new { x.MatchId, x.Round },
                        principalTable: "Round",
                        principalColumns: new[] { "MatchId", "Round" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RifleFiredWhileMoving_PlayerMatch_MatchId_SteamId",
                        columns: x => new { x.MatchId, x.SteamId },
                        principalTable: "PlayerMatch",
                        principalColumns: new[] { "MatchId", "SteamId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RifleFiredWhileMoving_PlayerRound_MatchId_Round_SteamId",
                        columns: x => new { x.MatchId, x.Round, x.SteamId },
                        principalTable: "PlayerRound",
                        principalColumns: new[] { "MatchId", "Round", "SteamId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SelfFlash",
                columns: table => new
                {
                    MatchId = table.Column<long>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Round = table.Column<short>(nullable: false),
                    StartTime = table.Column<int>(nullable: false),
                    SteamId = table.Column<long>(nullable: false),
                    TimeFlashedSelf = table.Column<int>(nullable: false),
                    TimeFlashedEnemies = table.Column<int>(nullable: false),
                    AngleToCrosshairSelf = table.Column<int>(nullable: false),
                    DeathTimeSelf = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SelfFlash", x => new { x.MatchId, x.Id });
                    table.ForeignKey(
                        name: "FK_SelfFlash_Match_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Match",
                        principalColumn: "MatchId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SelfFlash_Round_MatchId_Round",
                        columns: x => new { x.MatchId, x.Round },
                        principalTable: "Round",
                        principalColumns: new[] { "MatchId", "Round" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SelfFlash_PlayerMatch_MatchId_SteamId",
                        columns: x => new { x.MatchId, x.SteamId },
                        principalTable: "PlayerMatch",
                        principalColumns: new[] { "MatchId", "SteamId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SelfFlash_PlayerRound_MatchId_Round_SteamId",
                        columns: x => new { x.MatchId, x.Round, x.SteamId },
                        principalTable: "PlayerRound",
                        principalColumns: new[] { "MatchId", "Round", "SteamId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeamFlash",
                columns: table => new
                {
                    MatchId = table.Column<long>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Round = table.Column<short>(nullable: false),
                    StartTime = table.Column<int>(nullable: false),
                    SteamId = table.Column<long>(nullable: false),
                    FlashedTeammates = table.Column<int>(nullable: false),
                    FimeFlashedTeammates = table.Column<int>(nullable: false),
                    FlashedTeammatesDeaths = table.Column<int>(nullable: false),
                    TimeFlashedEnemies = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamFlash", x => new { x.MatchId, x.Id });
                    table.ForeignKey(
                        name: "FK_TeamFlash_Match_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Match",
                        principalColumn: "MatchId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamFlash_Round_MatchId_Round",
                        columns: x => new { x.MatchId, x.Round },
                        principalTable: "Round",
                        principalColumns: new[] { "MatchId", "Round" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamFlash_PlayerMatch_MatchId_SteamId",
                        columns: x => new { x.MatchId, x.SteamId },
                        principalTable: "PlayerMatch",
                        principalColumns: new[] { "MatchId", "SteamId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamFlash_PlayerRound_MatchId_Round_SteamId",
                        columns: x => new { x.MatchId, x.Round, x.SteamId },
                        principalTable: "PlayerRound",
                        principalColumns: new[] { "MatchId", "Round", "SteamId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UnnecessaryReload",
                columns: table => new
                {
                    MatchId = table.Column<long>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Round = table.Column<short>(nullable: false),
                    StartTime = table.Column<int>(nullable: false),
                    SteamId = table.Column<long>(nullable: false),
                    Weapon = table.Column<short>(nullable: false),
                    AmmoBefore = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnnecessaryReload", x => new { x.MatchId, x.Id });
                    table.ForeignKey(
                        name: "FK_UnnecessaryReload_Match_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Match",
                        principalColumn: "MatchId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UnnecessaryReload_Round_MatchId_Round",
                        columns: x => new { x.MatchId, x.Round },
                        principalTable: "Round",
                        principalColumns: new[] { "MatchId", "Round" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UnnecessaryReload_PlayerMatch_MatchId_SteamId",
                        columns: x => new { x.MatchId, x.SteamId },
                        principalTable: "PlayerMatch",
                        principalColumns: new[] { "MatchId", "SteamId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UnnecessaryReload_PlayerRound_MatchId_Round_SteamId",
                        columns: x => new { x.MatchId, x.Round, x.SteamId },
                        principalTable: "PlayerRound",
                        principalColumns: new[] { "MatchId", "Round", "SteamId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeathInducedBombDrop_MatchId",
                table: "DeathInducedBombDrop",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_DeathInducedBombDrop_SteamId",
                table: "DeathInducedBombDrop",
                column: "SteamId");

            migrationBuilder.CreateIndex(
                name: "IX_DeathInducedBombDrop_MatchId_SteamId",
                table: "DeathInducedBombDrop",
                columns: new[] { "MatchId", "SteamId" });

            migrationBuilder.CreateIndex(
                name: "IX_DeathInducedBombDrop_MatchId_Round_SteamId",
                table: "DeathInducedBombDrop",
                columns: new[] { "MatchId", "Round", "SteamId" });

            migrationBuilder.CreateIndex(
                name: "IX_KillWithOwnFlashAssist_MatchId",
                table: "KillWithOwnFlashAssist",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_KillWithOwnFlashAssist_SteamId",
                table: "KillWithOwnFlashAssist",
                column: "SteamId");

            migrationBuilder.CreateIndex(
                name: "IX_KillWithOwnFlashAssist_MatchId_SteamId",
                table: "KillWithOwnFlashAssist",
                columns: new[] { "MatchId", "SteamId" });

            migrationBuilder.CreateIndex(
                name: "IX_KillWithOwnFlashAssist_MatchId_Round_SteamId",
                table: "KillWithOwnFlashAssist",
                columns: new[] { "MatchId", "Round", "SteamId" });

            migrationBuilder.CreateIndex(
                name: "IX_PushBeforeSmokeDetonated_MatchId",
                table: "PushBeforeSmokeDetonated",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_PushBeforeSmokeDetonated_SteamId",
                table: "PushBeforeSmokeDetonated",
                column: "SteamId");

            migrationBuilder.CreateIndex(
                name: "IX_PushBeforeSmokeDetonated_MatchId_SteamId",
                table: "PushBeforeSmokeDetonated",
                columns: new[] { "MatchId", "SteamId" });

            migrationBuilder.CreateIndex(
                name: "IX_PushBeforeSmokeDetonated_MatchId_Round_SteamId",
                table: "PushBeforeSmokeDetonated",
                columns: new[] { "MatchId", "Round", "SteamId" });

            migrationBuilder.CreateIndex(
                name: "IX_RifleFiredWhileMoving_MatchId",
                table: "RifleFiredWhileMoving",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_RifleFiredWhileMoving_SteamId",
                table: "RifleFiredWhileMoving",
                column: "SteamId");

            migrationBuilder.CreateIndex(
                name: "IX_RifleFiredWhileMoving_MatchId_SteamId",
                table: "RifleFiredWhileMoving",
                columns: new[] { "MatchId", "SteamId" });

            migrationBuilder.CreateIndex(
                name: "IX_RifleFiredWhileMoving_MatchId_Round_SteamId",
                table: "RifleFiredWhileMoving",
                columns: new[] { "MatchId", "Round", "SteamId" });

            migrationBuilder.CreateIndex(
                name: "IX_SelfFlash_MatchId",
                table: "SelfFlash",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_SelfFlash_SteamId",
                table: "SelfFlash",
                column: "SteamId");

            migrationBuilder.CreateIndex(
                name: "IX_SelfFlash_MatchId_SteamId",
                table: "SelfFlash",
                columns: new[] { "MatchId", "SteamId" });

            migrationBuilder.CreateIndex(
                name: "IX_SelfFlash_MatchId_Round_SteamId",
                table: "SelfFlash",
                columns: new[] { "MatchId", "Round", "SteamId" });

            migrationBuilder.CreateIndex(
                name: "IX_TeamFlash_MatchId",
                table: "TeamFlash",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamFlash_SteamId",
                table: "TeamFlash",
                column: "SteamId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamFlash_MatchId_SteamId",
                table: "TeamFlash",
                columns: new[] { "MatchId", "SteamId" });

            migrationBuilder.CreateIndex(
                name: "IX_TeamFlash_MatchId_Round_SteamId",
                table: "TeamFlash",
                columns: new[] { "MatchId", "Round", "SteamId" });

            migrationBuilder.CreateIndex(
                name: "IX_UnnecessaryReload_MatchId",
                table: "UnnecessaryReload",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_UnnecessaryReload_SteamId",
                table: "UnnecessaryReload",
                column: "SteamId");

            migrationBuilder.CreateIndex(
                name: "IX_UnnecessaryReload_MatchId_SteamId",
                table: "UnnecessaryReload",
                columns: new[] { "MatchId", "SteamId" });

            migrationBuilder.CreateIndex(
                name: "IX_UnnecessaryReload_MatchId_Round_SteamId",
                table: "UnnecessaryReload",
                columns: new[] { "MatchId", "Round", "SteamId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeathInducedBombDrop");

            migrationBuilder.DropTable(
                name: "KillWithOwnFlashAssist");

            migrationBuilder.DropTable(
                name: "PushBeforeSmokeDetonated");

            migrationBuilder.DropTable(
                name: "RifleFiredWhileMoving");

            migrationBuilder.DropTable(
                name: "SelfFlash");

            migrationBuilder.DropTable(
                name: "TeamFlash");

            migrationBuilder.DropTable(
                name: "UnnecessaryReload");
        }
    }
}

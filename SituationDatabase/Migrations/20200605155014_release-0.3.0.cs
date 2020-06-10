using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SituationDatabase.Migrations
{
    public partial class release030 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "WinType",
                table: "Round",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint unsigned");

            migrationBuilder.CreateTable(
                name: "BombDropAtSpawn",
                columns: table => new
                {
                    MatchId = table.Column<long>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Round = table.Column<short>(nullable: false),
                    StartTime = table.Column<int>(nullable: false),
                    SteamId = table.Column<long>(nullable: false),
                    PickedUpAfter = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BombDropAtSpawn", x => new { x.MatchId, x.Id });
                    table.ForeignKey(
                        name: "FK_BombDropAtSpawn_Match_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Match",
                        principalColumn: "MatchId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BombDropAtSpawn_Round_MatchId_Round",
                        columns: x => new { x.MatchId, x.Round },
                        principalTable: "Round",
                        principalColumns: new[] { "MatchId", "Round" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BombDropAtSpawn_PlayerMatch_MatchId_SteamId",
                        columns: x => new { x.MatchId, x.SteamId },
                        principalTable: "PlayerMatch",
                        principalColumns: new[] { "MatchId", "SteamId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BombDropAtSpawn_PlayerRound_MatchId_Round_SteamId",
                        columns: x => new { x.MatchId, x.Round, x.SteamId },
                        principalTable: "PlayerRound",
                        principalColumns: new[] { "MatchId", "Round", "SteamId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Clutch",
                columns: table => new
                {
                    MatchId = table.Column<long>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Round = table.Column<short>(nullable: false),
                    StartTime = table.Column<int>(nullable: false),
                    SteamId = table.Column<long>(nullable: false),
                    EnemiesAlive = table.Column<int>(nullable: false),
                    EquipmentValue = table.Column<int>(nullable: false),
                    EnemyEquipmentValue = table.Column<int>(nullable: false),
                    WinType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clutch", x => new { x.MatchId, x.Id });
                    table.ForeignKey(
                        name: "FK_Clutch_Match_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Match",
                        principalColumn: "MatchId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Clutch_Round_MatchId_Round",
                        columns: x => new { x.MatchId, x.Round },
                        principalTable: "Round",
                        principalColumns: new[] { "MatchId", "Round" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Clutch_PlayerMatch_MatchId_SteamId",
                        columns: x => new { x.MatchId, x.SteamId },
                        principalTable: "PlayerMatch",
                        principalColumns: new[] { "MatchId", "SteamId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Clutch_PlayerRound_MatchId_Round_SteamId",
                        columns: x => new { x.MatchId, x.Round, x.SteamId },
                        principalTable: "PlayerRound",
                        principalColumns: new[] { "MatchId", "Round", "SteamId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HasNotBoughtDefuseKit",
                columns: table => new
                {
                    MatchId = table.Column<long>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Round = table.Column<short>(nullable: false),
                    StartTime = table.Column<int>(nullable: false),
                    SteamId = table.Column<long>(nullable: false),
                    MoneyLeft = table.Column<int>(nullable: false),
                    DefuseKitsInTeam = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HasNotBoughtDefuseKit", x => new { x.MatchId, x.Id });
                    table.ForeignKey(
                        name: "FK_HasNotBoughtDefuseKit_Match_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Match",
                        principalColumn: "MatchId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HasNotBoughtDefuseKit_Round_MatchId_Round",
                        columns: x => new { x.MatchId, x.Round },
                        principalTable: "Round",
                        principalColumns: new[] { "MatchId", "Round" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HasNotBoughtDefuseKit_PlayerMatch_MatchId_SteamId",
                        columns: x => new { x.MatchId, x.SteamId },
                        principalTable: "PlayerMatch",
                        principalColumns: new[] { "MatchId", "SteamId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HasNotBoughtDefuseKit_PlayerRound_MatchId_Round_SteamId",
                        columns: x => new { x.MatchId, x.Round, x.SteamId },
                        principalTable: "PlayerRound",
                        principalColumns: new[] { "MatchId", "Round", "SteamId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BombDropAtSpawn_MatchId",
                table: "BombDropAtSpawn",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_BombDropAtSpawn_SteamId",
                table: "BombDropAtSpawn",
                column: "SteamId");

            migrationBuilder.CreateIndex(
                name: "IX_BombDropAtSpawn_MatchId_SteamId",
                table: "BombDropAtSpawn",
                columns: new[] { "MatchId", "SteamId" });

            migrationBuilder.CreateIndex(
                name: "IX_BombDropAtSpawn_MatchId_Round_SteamId",
                table: "BombDropAtSpawn",
                columns: new[] { "MatchId", "Round", "SteamId" });

            migrationBuilder.CreateIndex(
                name: "IX_Clutch_MatchId",
                table: "Clutch",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_Clutch_SteamId",
                table: "Clutch",
                column: "SteamId");

            migrationBuilder.CreateIndex(
                name: "IX_Clutch_MatchId_SteamId",
                table: "Clutch",
                columns: new[] { "MatchId", "SteamId" });

            migrationBuilder.CreateIndex(
                name: "IX_Clutch_MatchId_Round_SteamId",
                table: "Clutch",
                columns: new[] { "MatchId", "Round", "SteamId" });

            migrationBuilder.CreateIndex(
                name: "IX_HasNotBoughtDefuseKit_MatchId",
                table: "HasNotBoughtDefuseKit",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_HasNotBoughtDefuseKit_SteamId",
                table: "HasNotBoughtDefuseKit",
                column: "SteamId");

            migrationBuilder.CreateIndex(
                name: "IX_HasNotBoughtDefuseKit_MatchId_SteamId",
                table: "HasNotBoughtDefuseKit",
                columns: new[] { "MatchId", "SteamId" });

            migrationBuilder.CreateIndex(
                name: "IX_HasNotBoughtDefuseKit_MatchId_Round_SteamId",
                table: "HasNotBoughtDefuseKit",
                columns: new[] { "MatchId", "Round", "SteamId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BombDropAtSpawn");

            migrationBuilder.DropTable(
                name: "Clutch");

            migrationBuilder.DropTable(
                name: "HasNotBoughtDefuseKit");

            migrationBuilder.AlterColumn<byte>(
                name: "WinType",
                table: "Round",
                type: "tinyint unsigned",
                nullable: false,
                oldClrType: typeof(int));
        }
    }
}

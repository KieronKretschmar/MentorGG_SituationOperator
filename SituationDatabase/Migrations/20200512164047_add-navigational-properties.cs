using Microsoft.EntityFrameworkCore.Migrations;

namespace SituationDatabase.Migrations
{
    public partial class addnavigationalproperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayerRound_Round_MatchId_RoundNumber",
                table: "PlayerRound");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerRound",
                table: "PlayerRound");

            migrationBuilder.DropColumn(
                name: "RoundNumber",
                table: "PlayerRound");

            migrationBuilder.AddColumn<short>(
                name: "Round",
                table: "PlayerRound",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerRound",
                table: "PlayerRound",
                columns: new[] { "MatchId", "Round", "SteamId" });

            migrationBuilder.CreateIndex(
                name: "IX_SmokeFail_MatchId_SteamId",
                table: "SmokeFail",
                columns: new[] { "MatchId", "SteamId" });

            migrationBuilder.CreateIndex(
                name: "IX_SmokeFail_MatchId_Round_SteamId",
                table: "SmokeFail",
                columns: new[] { "MatchId", "Round", "SteamId" });

            migrationBuilder.CreateIndex(
                name: "IX_EffectiveHeGrenade_MatchId_SteamId",
                table: "EffectiveHeGrenade",
                columns: new[] { "MatchId", "SteamId" });

            migrationBuilder.CreateIndex(
                name: "IX_EffectiveHeGrenade_MatchId_Round_SteamId",
                table: "EffectiveHeGrenade",
                columns: new[] { "MatchId", "Round", "SteamId" });

            migrationBuilder.AddForeignKey(
                name: "FK_EffectiveHeGrenade_Match_MatchId",
                table: "EffectiveHeGrenade",
                column: "MatchId",
                principalTable: "Match",
                principalColumn: "MatchId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EffectiveHeGrenade_Round_MatchId_Round",
                table: "EffectiveHeGrenade",
                columns: new[] { "MatchId", "Round" },
                principalTable: "Round",
                principalColumns: new[] { "MatchId", "Round" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EffectiveHeGrenade_PlayerMatch_MatchId_SteamId",
                table: "EffectiveHeGrenade",
                columns: new[] { "MatchId", "SteamId" },
                principalTable: "PlayerMatch",
                principalColumns: new[] { "MatchId", "SteamId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EffectiveHeGrenade_PlayerRound_MatchId_Round_SteamId",
                table: "EffectiveHeGrenade",
                columns: new[] { "MatchId", "Round", "SteamId" },
                principalTable: "PlayerRound",
                principalColumns: new[] { "MatchId", "Round", "SteamId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerRound_Round_MatchId_Round",
                table: "PlayerRound",
                columns: new[] { "MatchId", "Round" },
                principalTable: "Round",
                principalColumns: new[] { "MatchId", "Round" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SmokeFail_Match_MatchId",
                table: "SmokeFail",
                column: "MatchId",
                principalTable: "Match",
                principalColumn: "MatchId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SmokeFail_Round_MatchId_Round",
                table: "SmokeFail",
                columns: new[] { "MatchId", "Round" },
                principalTable: "Round",
                principalColumns: new[] { "MatchId", "Round" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SmokeFail_PlayerMatch_MatchId_SteamId",
                table: "SmokeFail",
                columns: new[] { "MatchId", "SteamId" },
                principalTable: "PlayerMatch",
                principalColumns: new[] { "MatchId", "SteamId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SmokeFail_PlayerRound_MatchId_Round_SteamId",
                table: "SmokeFail",
                columns: new[] { "MatchId", "Round", "SteamId" },
                principalTable: "PlayerRound",
                principalColumns: new[] { "MatchId", "Round", "SteamId" },
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EffectiveHeGrenade_Match_MatchId",
                table: "EffectiveHeGrenade");

            migrationBuilder.DropForeignKey(
                name: "FK_EffectiveHeGrenade_Round_MatchId_Round",
                table: "EffectiveHeGrenade");

            migrationBuilder.DropForeignKey(
                name: "FK_EffectiveHeGrenade_PlayerMatch_MatchId_SteamId",
                table: "EffectiveHeGrenade");

            migrationBuilder.DropForeignKey(
                name: "FK_EffectiveHeGrenade_PlayerRound_MatchId_Round_SteamId",
                table: "EffectiveHeGrenade");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerRound_Round_MatchId_Round",
                table: "PlayerRound");

            migrationBuilder.DropForeignKey(
                name: "FK_SmokeFail_Match_MatchId",
                table: "SmokeFail");

            migrationBuilder.DropForeignKey(
                name: "FK_SmokeFail_Round_MatchId_Round",
                table: "SmokeFail");

            migrationBuilder.DropForeignKey(
                name: "FK_SmokeFail_PlayerMatch_MatchId_SteamId",
                table: "SmokeFail");

            migrationBuilder.DropForeignKey(
                name: "FK_SmokeFail_PlayerRound_MatchId_Round_SteamId",
                table: "SmokeFail");

            migrationBuilder.DropIndex(
                name: "IX_SmokeFail_MatchId_SteamId",
                table: "SmokeFail");

            migrationBuilder.DropIndex(
                name: "IX_SmokeFail_MatchId_Round_SteamId",
                table: "SmokeFail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerRound",
                table: "PlayerRound");

            migrationBuilder.DropIndex(
                name: "IX_EffectiveHeGrenade_MatchId_SteamId",
                table: "EffectiveHeGrenade");

            migrationBuilder.DropIndex(
                name: "IX_EffectiveHeGrenade_MatchId_Round_SteamId",
                table: "EffectiveHeGrenade");

            migrationBuilder.DropColumn(
                name: "Round",
                table: "PlayerRound");

            migrationBuilder.AddColumn<short>(
                name: "RoundNumber",
                table: "PlayerRound",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerRound",
                table: "PlayerRound",
                columns: new[] { "MatchId", "RoundNumber", "SteamId" });

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerRound_Round_MatchId_RoundNumber",
                table: "PlayerRound",
                columns: new[] { "MatchId", "RoundNumber" },
                principalTable: "Round",
                principalColumns: new[] { "MatchId", "Round" },
                onDelete: ReferentialAction.Cascade);
        }
    }
}

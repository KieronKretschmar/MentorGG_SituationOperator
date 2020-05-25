using Microsoft.EntityFrameworkCore.Migrations;

namespace SituationDatabase.Migrations
{
    public partial class updatemodels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FimeFlashedTeammates",
                table: "TeamFlash");

            migrationBuilder.DropColumn(
                name: "TimeBetweenDetonationAndKill",
                table: "KillWithOwnFlashAssist");

            migrationBuilder.DropColumn(
                name: "TimeFlashedAfterDeath",
                table: "KillWithOwnFlashAssist");

            migrationBuilder.AddColumn<long>(
                name: "GrenadeId",
                table: "TeamFlash",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "TimeFlashedTeammates",
                table: "TeamFlash",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "GrenadeId",
                table: "SmokeFail",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "GrenadeId",
                table: "SelfFlash",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "GrenadeId",
                table: "PushBeforeSmokeDetonated",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "AssistedKills",
                table: "KillWithOwnFlashAssist",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "GrenadeId",
                table: "KillWithOwnFlashAssist",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "TimeBetweenDetonationAndFirstKill",
                table: "KillWithOwnFlashAssist",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "GrenadeId",
                table: "EffectiveHeGrenade",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<float>(
                name: "ClosestTeammateDistance",
                table: "DeathInducedBombDrop",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GrenadeId",
                table: "TeamFlash");

            migrationBuilder.DropColumn(
                name: "TimeFlashedTeammates",
                table: "TeamFlash");

            migrationBuilder.DropColumn(
                name: "GrenadeId",
                table: "SmokeFail");

            migrationBuilder.DropColumn(
                name: "GrenadeId",
                table: "SelfFlash");

            migrationBuilder.DropColumn(
                name: "GrenadeId",
                table: "PushBeforeSmokeDetonated");

            migrationBuilder.DropColumn(
                name: "AssistedKills",
                table: "KillWithOwnFlashAssist");

            migrationBuilder.DropColumn(
                name: "GrenadeId",
                table: "KillWithOwnFlashAssist");

            migrationBuilder.DropColumn(
                name: "TimeBetweenDetonationAndFirstKill",
                table: "KillWithOwnFlashAssist");

            migrationBuilder.DropColumn(
                name: "GrenadeId",
                table: "EffectiveHeGrenade");

            migrationBuilder.AddColumn<int>(
                name: "FimeFlashedTeammates",
                table: "TeamFlash",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TimeBetweenDetonationAndKill",
                table: "KillWithOwnFlashAssist",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TimeFlashedAfterDeath",
                table: "KillWithOwnFlashAssist",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "ClosestTeammateDistance",
                table: "DeathInducedBombDrop",
                type: "int",
                nullable: false,
                oldClrType: typeof(float));
        }
    }
}

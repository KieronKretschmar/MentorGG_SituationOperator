using Microsoft.EntityFrameworkCore.Migrations;

namespace SituationDatabase.Migrations
{
    public partial class release1002 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FlashedTeammates",
                table: "FlashAssist");

            migrationBuilder.DropColumn(
                name: "FlashedTeammatesDeaths",
                table: "FlashAssist");

            migrationBuilder.AddColumn<int>(
                name: "FlashedEnemiesDeaths",
                table: "FlashAssist",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FlashedEnemiesDeaths",
                table: "FlashAssist");

            migrationBuilder.AddColumn<int>(
                name: "FlashedTeammates",
                table: "FlashAssist",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FlashedTeammatesDeaths",
                table: "FlashAssist",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}

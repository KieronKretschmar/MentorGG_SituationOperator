using Microsoft.EntityFrameworkCore.Migrations;

namespace SituationDatabase.Migrations
{
    public partial class release0401 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnemyEquipmentValue",
                table: "Clutch");

            migrationBuilder.DropColumn(
                name: "EquipmentValue",
                table: "Clutch");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EnemyEquipmentValue",
                table: "Clutch",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EquipmentValue",
                table: "Clutch",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}

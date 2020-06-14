using Microsoft.EntityFrameworkCore.Migrations;

namespace SituationDatabase.Migrations
{
    public partial class release0402 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PlayerEquipmentValue",
                table: "HasNotBoughtDefuseKit",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TeamEquipmentValue",
                table: "HasNotBoughtDefuseKit",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlayerEquipmentValue",
                table: "HasNotBoughtDefuseKit");

            migrationBuilder.DropColumn(
                name: "TeamEquipmentValue",
                table: "HasNotBoughtDefuseKit");
        }
    }
}

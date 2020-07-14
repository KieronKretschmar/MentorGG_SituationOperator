using Microsoft.EntityFrameworkCore.Migrations;

namespace SituationDatabase.Migrations
{
    public partial class feedback : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserFeedback",
                columns: table => new
                {
                    MatchId = table.Column<long>(nullable: false),
                    SituationType = table.Column<int>(nullable: false),
                    SituationId = table.Column<long>(nullable: false),
                    SteamId = table.Column<long>(nullable: false),
                    IsPositive = table.Column<bool>(nullable: false),
                    Comment = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFeedback", x => new { x.MatchId, x.SituationType, x.SituationId, x.SteamId });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserFeedback");
        }
    }
}

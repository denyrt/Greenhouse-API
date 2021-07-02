using Microsoft.EntityFrameworkCore.Migrations;

namespace Greenhouse.Data.Migrations
{
    public partial class ReportsFkUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_WetReports_ControllerId",
                table: "WetReports",
                column: "ControllerId");

            migrationBuilder.CreateIndex(
                name: "IX_LightReports_ControllerId",
                table: "LightReports",
                column: "ControllerId");

            migrationBuilder.AddForeignKey(
                name: "FK_LightReports_LightControllers_ControllerId",
                table: "LightReports",
                column: "ControllerId",
                principalTable: "LightControllers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WetReports_WetControllers_ControllerId",
                table: "WetReports",
                column: "ControllerId",
                principalTable: "WetControllers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LightReports_LightControllers_ControllerId",
                table: "LightReports");

            migrationBuilder.DropForeignKey(
                name: "FK_WetReports_WetControllers_ControllerId",
                table: "WetReports");

            migrationBuilder.DropIndex(
                name: "IX_WetReports_ControllerId",
                table: "WetReports");

            migrationBuilder.DropIndex(
                name: "IX_LightReports_ControllerId",
                table: "LightReports");
        }
    }
}

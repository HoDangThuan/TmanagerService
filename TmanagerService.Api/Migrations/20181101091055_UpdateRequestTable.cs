using Microsoft.EntityFrameworkCore.Migrations;

namespace TmanagerService.Api.Migrations
{
    public partial class UpdateRequestTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ReportUserId",
                table: "Requests",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Requests_ReportUserId",
                table: "Requests",
                column: "ReportUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_AspNetUsers_ReportUserId",
                table: "Requests",
                column: "ReportUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requests_AspNetUsers_ReportUserId",
                table: "Requests");

            migrationBuilder.DropIndex(
                name: "IX_Requests_ReportUserId",
                table: "Requests");

            migrationBuilder.AlterColumn<string>(
                name: "ReportUserId",
                table: "Requests",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}

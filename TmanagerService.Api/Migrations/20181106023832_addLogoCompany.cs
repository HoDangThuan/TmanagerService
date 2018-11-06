using Microsoft.EntityFrameworkCore.Migrations;

namespace TmanagerService.Api.Migrations
{
    public partial class addLogoCompany : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Logo",
                table: "Companys",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Logo",
                table: "Companys");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace TmanagerService.Api.Migrations
{
    public partial class addAvatarForUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Avatar",
                table: "AspNetUsers",
                nullable: true);
        }
    }
}

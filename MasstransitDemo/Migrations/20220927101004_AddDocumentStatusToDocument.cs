using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MasstransitDemo.Migrations
{
    public partial class AddDocumentStatusToDocument : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Documents",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Documents");
        }
    }
}

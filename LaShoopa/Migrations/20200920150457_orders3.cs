using Microsoft.EntityFrameworkCore.Migrations;

namespace LaShoopa.Migrations
{
    public partial class orders3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProductsSizes",
                table: "Orders",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductsSizes",
                table: "Orders");
        }
    }
}

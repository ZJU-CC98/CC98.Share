using Microsoft.EntityFrameworkCore.Migrations;

namespace cc98.share.Migrations
{
    public partial class total : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalSize",
                table: "Items");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "TotalSize",
                table: "Items",
                nullable: false,
                defaultValue: 0L);
        }
    }
}

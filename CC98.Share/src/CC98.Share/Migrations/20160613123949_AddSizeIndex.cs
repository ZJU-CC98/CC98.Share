using Microsoft.EntityFrameworkCore.Migrations;

namespace cc98.share.Migrations
{
    public partial class AddSizeIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "Items",
                nullable: false);

            migrationBuilder.CreateIndex(
                name: "IX_Items_UserName_Size",
                table: "Items",
                columns: new[] { "UserName", "Size" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Items_UserName_Size",
                table: "Items");

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "Items",
                nullable: false);
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace cc98.share.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "Items",
                table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(nullable: true),
                    DownloadCount = table.Column<int>(nullable: false),
                    IsShared = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Path = table.Column<string>(nullable: true),
                    Size = table.Column<long>(nullable: false),
                    TotalSize = table.Column<long>(nullable: false),
                    UploadTime = table.Column<DateTime>(nullable: false),
                    UserName = table.Column<string>(nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_Items", x => x.Id); });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "Items");
        }
    }
}
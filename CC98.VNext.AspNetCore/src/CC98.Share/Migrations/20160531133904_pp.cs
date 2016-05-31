using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CC98.Share.Migrations
{
    public partial class pp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "UploadTime",
                table: "Items",
                nullable: false);

            migrationBuilder.AlterColumn<long>(
                name: "Size",
                table: "Items",
                nullable: false);

            migrationBuilder.AddColumn<long>(
                name: "TotalSize",
                table: "Items",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalSize",
                table: "Items");

            migrationBuilder.AlterColumn<string>(
                name: "UploadTime",
                table: "Items",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Size",
                table: "Items",
                nullable: false);
        }
    }
}

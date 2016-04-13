using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace CC98.Sports.Migrations
{
    public partial class V11 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_EventMemberRegistration_Event_EventId", table: "EventMemberRegistration");
            migrationBuilder.DropForeignKey(name: "FK_EventMemberRegistration_Member_MemberId", table: "EventMemberRegistration");
            migrationBuilder.DropForeignKey(name: "FK_EventMemberRegistration_Team_TeamId", table: "EventMemberRegistration");
            migrationBuilder.DropForeignKey(name: "FK_EventMemberRegistration_EventTeamRegistration_EventId_TeamId", table: "EventMemberRegistration");
            migrationBuilder.DropForeignKey(name: "FK_EventTeamRegistration_Event_EventId", table: "EventTeamRegistration");
            migrationBuilder.DropForeignKey(name: "FK_EventTeamRegistration_Team_TeamId", table: "EventTeamRegistration");
            migrationBuilder.DropForeignKey(name: "FK_Game_Event_EventId", table: "Game");
            migrationBuilder.DropForeignKey(name: "FK_OfficerGameApplication_Game_GameId", table: "OfficerGameApplication");
            migrationBuilder.DropForeignKey(name: "FK_OfficerGameApplication_Member_MemberId", table: "OfficerGameApplication");
            migrationBuilder.DropTable("TeamMembmrRegistration");
            migrationBuilder.CreateTable(
                name: "TeamMemberRegistration",
                columns: table => new
                {
                    TeamId = table.Column<int>(nullable: false),
                    MemberId = table.Column<int>(nullable: false),
                    MemberAuditState = table.Column<int>(nullable: false),
                    TeamAuditState = table.Column<int>(nullable: false),
                    Time = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamMemberRegistration", x => new { x.TeamId, x.MemberId });
                    table.ForeignKey(
                        name: "FK_TeamMemberRegistration_Member_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Member",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamMemberRegistration_Team_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Team",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.AddForeignKey(
                name: "FK_EventMemberRegistration_Event_EventId",
                table: "EventMemberRegistration",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_EventMemberRegistration_Member_MemberId",
                table: "EventMemberRegistration",
                column: "MemberId",
                principalTable: "Member",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_EventMemberRegistration_Team_TeamId",
                table: "EventMemberRegistration",
                column: "TeamId",
                principalTable: "Team",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_EventMemberRegistration_EventTeamRegistration_EventId_TeamId",
                table: "EventMemberRegistration",
                columns: new[] { "EventId", "TeamId" },
                principalTable: "EventTeamRegistration",
                principalColumns: new[] { "EventId", "TeamId" },
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_EventTeamRegistration_Event_EventId",
                table: "EventTeamRegistration",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
            migrationBuilder.AddForeignKey(
                name: "FK_EventTeamRegistration_Team_TeamId",
                table: "EventTeamRegistration",
                column: "TeamId",
                principalTable: "Team",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
            migrationBuilder.AddForeignKey(
                name: "FK_Game_Event_EventId",
                table: "Game",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_OfficerGameApplication_Game_GameId",
                table: "OfficerGameApplication",
                column: "GameId",
                principalTable: "Game",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_OfficerGameApplication_Member_MemberId",
                table: "OfficerGameApplication",
                column: "MemberId",
                principalTable: "Member",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_EventMemberRegistration_Event_EventId", table: "EventMemberRegistration");
            migrationBuilder.DropForeignKey(name: "FK_EventMemberRegistration_Member_MemberId", table: "EventMemberRegistration");
            migrationBuilder.DropForeignKey(name: "FK_EventMemberRegistration_Team_TeamId", table: "EventMemberRegistration");
            migrationBuilder.DropForeignKey(name: "FK_EventMemberRegistration_EventTeamRegistration_EventId_TeamId", table: "EventMemberRegistration");
            migrationBuilder.DropForeignKey(name: "FK_EventTeamRegistration_Event_EventId", table: "EventTeamRegistration");
            migrationBuilder.DropForeignKey(name: "FK_EventTeamRegistration_Team_TeamId", table: "EventTeamRegistration");
            migrationBuilder.DropForeignKey(name: "FK_Game_Event_EventId", table: "Game");
            migrationBuilder.DropForeignKey(name: "FK_OfficerGameApplication_Game_GameId", table: "OfficerGameApplication");
            migrationBuilder.DropForeignKey(name: "FK_OfficerGameApplication_Member_MemberId", table: "OfficerGameApplication");
            migrationBuilder.DropTable("TeamMemberRegistration");
            migrationBuilder.CreateTable(
                name: "TeamMembmrRegistration",
                columns: table => new
                {
                    TeamId = table.Column<int>(nullable: false),
                    MemberId = table.Column<int>(nullable: false),
                    MemberAuditState = table.Column<int>(nullable: false),
                    TeamAuditState = table.Column<int>(nullable: false),
                    Time = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamMembmrRegistration", x => new { x.TeamId, x.MemberId });
                    table.ForeignKey(
                        name: "FK_TeamMembmrRegistration_Member_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Member",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeamMembmrRegistration_Team_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Team",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });
            migrationBuilder.AddForeignKey(
                name: "FK_EventMemberRegistration_Event_EventId",
                table: "EventMemberRegistration",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_EventMemberRegistration_Member_MemberId",
                table: "EventMemberRegistration",
                column: "MemberId",
                principalTable: "Member",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_EventMemberRegistration_Team_TeamId",
                table: "EventMemberRegistration",
                column: "TeamId",
                principalTable: "Team",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_EventMemberRegistration_EventTeamRegistration_EventId_TeamId",
                table: "EventMemberRegistration",
                columns: new[] { "EventId", "TeamId" },
                principalTable: "EventTeamRegistration",
                principalColumns: new[] { "EventId", "TeamId" },
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_EventTeamRegistration_Event_EventId",
                table: "EventTeamRegistration",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_EventTeamRegistration_Team_TeamId",
                table: "EventTeamRegistration",
                column: "TeamId",
                principalTable: "Team",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_Game_Event_EventId",
                table: "Game",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_OfficerGameApplication_Game_GameId",
                table: "OfficerGameApplication",
                column: "GameId",
                principalTable: "Game",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_OfficerGameApplication_Member_MemberId",
                table: "OfficerGameApplication",
                column: "MemberId",
                principalTable: "Member",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

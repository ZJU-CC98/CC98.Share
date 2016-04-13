using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace CC98.Sports.Migrations
{
    public partial class AddTeamClotheColor : Migration
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
            migrationBuilder.DropForeignKey(name: "FK_TeamMembmrRegistration_Member_MemberId", table: "TeamMembmrRegistration");
            migrationBuilder.DropForeignKey(name: "FK_TeamMembmrRegistration_Team_TeamId", table: "TeamMembmrRegistration");
            migrationBuilder.AddColumn<string>(
                name: "ClotheColor1",
                table: "Team",
                nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "ClotheColor2",
                table: "Team",
                nullable: true);
            migrationBuilder.AddForeignKey(
                name: "FK_EventMemberRegistration_Event_EventId",
                table: "EventMemberRegistration",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
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
                onDelete: ReferentialAction.NoAction);
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
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_EventTeamRegistration_Team_TeamId",
                table: "EventTeamRegistration",
                column: "TeamId",
                principalTable: "Team",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
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
            migrationBuilder.AddForeignKey(
                name: "FK_TeamMembmrRegistration_Member_MemberId",
                table: "TeamMembmrRegistration",
                column: "MemberId",
                principalTable: "Member",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_TeamMembmrRegistration_Team_TeamId",
                table: "TeamMembmrRegistration",
                column: "TeamId",
                principalTable: "Team",
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
            migrationBuilder.DropForeignKey(name: "FK_TeamMembmrRegistration_Member_MemberId", table: "TeamMembmrRegistration");
            migrationBuilder.DropForeignKey(name: "FK_TeamMembmrRegistration_Team_TeamId", table: "TeamMembmrRegistration");
            migrationBuilder.DropColumn(name: "ClotheColor1", table: "Team");
            migrationBuilder.DropColumn(name: "ClotheColor2", table: "Team");
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
            migrationBuilder.AddForeignKey(
                name: "FK_TeamMembmrRegistration_Member_MemberId",
                table: "TeamMembmrRegistration",
                column: "MemberId",
                principalTable: "Member",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_TeamMembmrRegistration_Team_TeamId",
                table: "TeamMembmrRegistration",
                column: "TeamId",
                principalTable: "Team",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;
using Microsoft.Data.Entity.Metadata;

namespace CC98.Sports.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Event",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AllowTeamRegistrations = table.Column<bool>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    ExternalMax = table.Column<int>(nullable: true),
                    ForeignMax = table.Column<int>(nullable: true),
                    Link = table.Column<string>(nullable: true),
                    MaxTeamCount = table.Column<int>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    PlayerMax = table.Column<int>(nullable: true),
                    PlayerMin = table.Column<int>(nullable: true),
                    ProfessionalMax = table.Column<int>(nullable: true),
                    State = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Event", x => x.Id);
                });
            migrationBuilder.CreateTable(
                name: "Member",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AuditCommitTime = table.Column<DateTime>(nullable: true),
                    AuditState = table.Column<int>(nullable: false),
                    CC98Id = table.Column<string>(nullable: true),
                    ContactInfo = table.Column<string>(nullable: true),
                    Department = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Gender = table.Column<int>(nullable: false),
                    IsProfessional = table.Column<bool>(nullable: false),
                    Location = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Nationality = table.Column<string>(nullable: true),
                    OfficerTypes = table.Column<int>(nullable: false),
                    PersonalId = table.Column<string>(nullable: true),
                    SchoolId = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    UploadImagePaths = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Member", x => x.Id);
                });
            migrationBuilder.CreateTable(
                name: "Message",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Content = table.Column<string>(nullable: true),
                    IsRead = table.Column<bool>(nullable: false),
                    Receiver = table.Column<string>(nullable: false),
                    Time = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Message", x => x.Id);
                });
            migrationBuilder.CreateTable(
                name: "Team",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CaptainId = table.Column<int>(nullable: true),
                    CoachId = table.Column<int>(nullable: true),
                    Department = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    IsLocked = table.Column<bool>(nullable: false),
                    Link = table.Column<string>(nullable: true),
                    Location = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    SkipperId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Team", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Team_Member_CaptainId",
                        column: x => x.CaptainId,
                        principalTable: "Member",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Team_Member_CoachId",
                        column: x => x.CoachId,
                        principalTable: "Member",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Team_Member_SkipperId",
                        column: x => x.SkipperId,
                        principalTable: "Member",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });
            migrationBuilder.CreateTable(
                name: "EventTeamRegistration",
                columns: table => new
                {
                    EventId = table.Column<int>(nullable: false),
                    TeamId = table.Column<int>(nullable: false),
                    AuditCommitTime = table.Column<DateTime>(nullable: true),
                    AuditState = table.Column<int>(nullable: false),
                    CaptainId = table.Column<int>(nullable: true),
                    CoachId = table.Column<int>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    EventState = table.Column<int>(nullable: false),
                    Group = table.Column<string>(nullable: true),
                    GroupNumber = table.Column<string>(nullable: true),
                    SkipperId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventTeamRegistration", x => new { x.EventId, x.TeamId });
                    table.ForeignKey(
                        name: "FK_EventTeamRegistration_Member_CaptainId",
                        column: x => x.CaptainId,
                        principalTable: "Member",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventTeamRegistration_Member_CoachId",
                        column: x => x.CoachId,
                        principalTable: "Member",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventTeamRegistration_Event_EventId",
                        column: x => x.EventId,
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventTeamRegistration_Member_SkipperId",
                        column: x => x.SkipperId,
                        principalTable: "Member",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventTeamRegistration_Team_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Team",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateTable(
                name: "Game",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Descrption = table.Column<string>(nullable: true),
                    EndTime = table.Column<DateTime>(nullable: true),
                    EventId = table.Column<int>(nullable: false),
                    Location = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Round = table.Column<int>(nullable: false),
                    StartTime = table.Column<DateTime>(nullable: true),
                    State = table.Column<int>(nullable: false),
                    Team1Id = table.Column<int>(nullable: true),
                    Team2Id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Game", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Game_Event_EventId",
                        column: x => x.EventId,
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Game_Team_Team1Id",
                        column: x => x.Team1Id,
                        principalTable: "Team",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Game_Team_Team2Id",
                        column: x => x.Team2Id,
                        principalTable: "Team",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });
            migrationBuilder.CreateTable(
                name: "Log",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ActionType = table.Column<int>(nullable: false),
                    CC98Id = table.Column<string>(nullable: true),
                    RelatedEventId = table.Column<int>(nullable: true),
                    RelatedMemberId = table.Column<int>(nullable: true),
                    RelatedTeamId = table.Column<int>(nullable: true),
                    Remark = table.Column<string>(nullable: true),
                    Time = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Log", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Log_Event_RelatedEventId",
                        column: x => x.RelatedEventId,
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Log_Member_RelatedMemberId",
                        column: x => x.RelatedMemberId,
                        principalTable: "Member",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Log_Team_RelatedTeamId",
                        column: x => x.RelatedTeamId,
                        principalTable: "Team",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });
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
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamMembmrRegistration_Team_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Team",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateTable(
                name: "EventMemberRegistration",
                columns: table => new
                {
                    EventId = table.Column<int>(nullable: false),
                    TeamId = table.Column<int>(nullable: false),
                    MemberId = table.Column<int>(nullable: false),
                    Number = table.Column<string>(nullable: true),
                    Remark = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventMemberRegistration", x => new { x.EventId, x.TeamId, x.MemberId });
                    table.ForeignKey(
                        name: "FK_EventMemberRegistration_Event_EventId",
                        column: x => x.EventId,
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventMemberRegistration_Member_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Member",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventMemberRegistration_Team_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Team",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventMemberRegistration_EventTeamRegistration_EventId_TeamId",
                        columns: x => new { x.EventId, x.TeamId },
                        principalTable: "EventTeamRegistration",
                        principalColumns: new[] { "EventId", "TeamId" },
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateTable(
                name: "OfficerGameApplication",
                columns: table => new
                {
                    GameId = table.Column<int>(nullable: false),
                    MemberId = table.Column<int>(nullable: false),
                    AuditState = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfficerGameApplication", x => new { x.GameId, x.MemberId });
                    table.ForeignKey(
                        name: "FK_OfficerGameApplication_Game_GameId",
                        column: x => x.GameId,
                        principalTable: "Game",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OfficerGameApplication_Member_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Member",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateIndex(
                name: "IX_Member_AuditState",
                table: "Member",
                column: "AuditState");
            migrationBuilder.CreateIndex(
                name: "IX_Message_IsRead",
                table: "Message",
                column: "IsRead");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("EventMemberRegistration");
            migrationBuilder.DropTable("Log");
            migrationBuilder.DropTable("Message");
            migrationBuilder.DropTable("OfficerGameApplication");
            migrationBuilder.DropTable("TeamMembmrRegistration");
            migrationBuilder.DropTable("EventTeamRegistration");
            migrationBuilder.DropTable("Game");
            migrationBuilder.DropTable("Event");
            migrationBuilder.DropTable("Team");
            migrationBuilder.DropTable("Member");
        }
    }
}

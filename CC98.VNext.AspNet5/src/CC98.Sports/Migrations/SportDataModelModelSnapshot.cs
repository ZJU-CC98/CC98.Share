using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations;
using CC98.Sports;

namespace CC98.Sports.Migrations
{
    [DbContext(typeof(SportDataModel))]
    partial class SportDataModelModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0-rc1-16348")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("CC98.Sports.Event", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("AllowTeamRegistrations");

                    b.Property<string>("Description");

                    b.Property<int?>("ExternalMax");

                    b.Property<int?>("ForeignMax");

                    b.Property<string>("Link");

                    b.Property<int?>("MaxTeamCount");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int?>("PlayerMax");

                    b.Property<int?>("PlayerMin");

                    b.Property<int?>("ProfessionalMax");

                    b.Property<int>("State");

                    b.Property<int>("Type");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("CC98.Sports.EventMemberRegistration", b =>
                {
                    b.Property<int>("EventId");

                    b.Property<int>("TeamId");

                    b.Property<int>("MemberId");

                    b.Property<string>("Number");

                    b.Property<string>("Remark");

                    b.HasKey("EventId", "TeamId", "MemberId");
                });

            modelBuilder.Entity("CC98.Sports.EventTeamRegistration", b =>
                {
                    b.Property<int>("EventId");

                    b.Property<int>("TeamId");

                    b.Property<DateTime?>("AuditCommitTime");

                    b.Property<int>("AuditState");

                    b.Property<int?>("CaptainId");

                    b.Property<int?>("CoachId");

                    b.Property<string>("Description");

                    b.Property<int>("EventState");

                    b.Property<string>("Group");

                    b.Property<string>("GroupNumber");

                    b.Property<int?>("SkipperId");

                    b.HasKey("EventId", "TeamId");
                });

            modelBuilder.Entity("CC98.Sports.Game", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Descrption");

                    b.Property<DateTime?>("EndTime");

                    b.Property<int>("EventId");

                    b.Property<string>("Location");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int>("Round");

                    b.Property<DateTime?>("StartTime");

                    b.Property<int>("State");

                    b.Property<int?>("Team1Id");

                    b.Property<int?>("Team2Id");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("CC98.Sports.Log", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ActionType");

                    b.Property<string>("CC98Id");

                    b.Property<int?>("RelatedEventId");

                    b.Property<int?>("RelatedMemberId");

                    b.Property<int?>("RelatedTeamId");

                    b.Property<string>("Remark");

                    b.Property<DateTime>("Time");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("CC98.Sports.Member", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("AuditCommitTime");

                    b.Property<int>("AuditState");

                    b.Property<string>("CC98Id");

                    b.Property<string>("ContactInfo");

                    b.Property<string>("Department");

                    b.Property<string>("Description");

                    b.Property<int>("Gender");

                    b.Property<bool>("IsProfessional");

                    b.Property<string>("Location");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("Nationality");

                    b.Property<int>("OfficerTypes");

                    b.Property<string>("PersonalId");

                    b.Property<string>("SchoolId");

                    b.Property<int>("Type");

                    b.Property<string>("UploadImagePaths");

                    b.HasKey("Id");

                    b.HasIndex("AuditState");
                });

            modelBuilder.Entity("CC98.Sports.Message", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Content");

                    b.Property<bool>("IsRead");

                    b.Property<string>("Receiver")
                        .IsRequired();

                    b.Property<DateTime>("Time");

                    b.HasKey("Id");

                    b.HasIndex("IsRead");
                });

            modelBuilder.Entity("CC98.Sports.OfficerGameApplication", b =>
                {
                    b.Property<int>("GameId");

                    b.Property<int>("MemberId");

                    b.Property<int>("AuditState");

                    b.HasKey("GameId", "MemberId");
                });

            modelBuilder.Entity("CC98.Sports.Team", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("CaptainId");

                    b.Property<string>("ClotheColor1");

                    b.Property<string>("ClotheColor2");

                    b.Property<int?>("CoachId");

                    b.Property<string>("Department");

                    b.Property<string>("Description");

                    b.Property<bool>("IsLocked");

                    b.Property<string>("Link");

                    b.Property<string>("Location");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int?>("SkipperId");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("CC98.Sports.TeamMemberRegistration", b =>
                {
                    b.Property<int>("TeamId");

                    b.Property<int>("MemberId");

                    b.Property<int>("MemberAuditState");

                    b.Property<int>("TeamAuditState");

                    b.Property<DateTime>("Time");

                    b.HasKey("TeamId", "MemberId");
                });

            modelBuilder.Entity("CC98.Sports.EventMemberRegistration", b =>
                {
                    b.HasOne("CC98.Sports.Event")
                        .WithMany()
                        .HasForeignKey("EventId");

                    b.HasOne("CC98.Sports.Member")
                        .WithMany()
                        .HasForeignKey("MemberId");

                    b.HasOne("CC98.Sports.Team")
                        .WithMany()
                        .HasForeignKey("TeamId");

                    b.HasOne("CC98.Sports.EventTeamRegistration")
                        .WithMany()
                        .HasForeignKey("EventId", "TeamId");
                });

            modelBuilder.Entity("CC98.Sports.EventTeamRegistration", b =>
                {
                    b.HasOne("CC98.Sports.Member")
                        .WithMany()
                        .HasForeignKey("CaptainId");

                    b.HasOne("CC98.Sports.Member")
                        .WithMany()
                        .HasForeignKey("CoachId");

                    b.HasOne("CC98.Sports.Event")
                        .WithMany()
                        .HasForeignKey("EventId");

                    b.HasOne("CC98.Sports.Member")
                        .WithMany()
                        .HasForeignKey("SkipperId");

                    b.HasOne("CC98.Sports.Team")
                        .WithMany()
                        .HasForeignKey("TeamId");
                });

            modelBuilder.Entity("CC98.Sports.Game", b =>
                {
                    b.HasOne("CC98.Sports.Event")
                        .WithMany()
                        .HasForeignKey("EventId");

                    b.HasOne("CC98.Sports.Team")
                        .WithMany()
                        .HasForeignKey("Team1Id");

                    b.HasOne("CC98.Sports.Team")
                        .WithMany()
                        .HasForeignKey("Team2Id");
                });

            modelBuilder.Entity("CC98.Sports.Log", b =>
                {
                    b.HasOne("CC98.Sports.Event")
                        .WithMany()
                        .HasForeignKey("RelatedEventId");

                    b.HasOne("CC98.Sports.Member")
                        .WithMany()
                        .HasForeignKey("RelatedMemberId");

                    b.HasOne("CC98.Sports.Team")
                        .WithMany()
                        .HasForeignKey("RelatedTeamId");
                });

            modelBuilder.Entity("CC98.Sports.OfficerGameApplication", b =>
                {
                    b.HasOne("CC98.Sports.Game")
                        .WithMany()
                        .HasForeignKey("GameId");

                    b.HasOne("CC98.Sports.Member")
                        .WithMany()
                        .HasForeignKey("MemberId");
                });

            modelBuilder.Entity("CC98.Sports.Team", b =>
                {
                    b.HasOne("CC98.Sports.Member")
                        .WithMany()
                        .HasForeignKey("CaptainId");

                    b.HasOne("CC98.Sports.Member")
                        .WithMany()
                        .HasForeignKey("CoachId");

                    b.HasOne("CC98.Sports.Member")
                        .WithMany()
                        .HasForeignKey("SkipperId");
                });

            modelBuilder.Entity("CC98.Sports.TeamMemberRegistration", b =>
                {
                    b.HasOne("CC98.Sports.Member")
                        .WithMany()
                        .HasForeignKey("MemberId");

                    b.HasOne("CC98.Sports.Team")
                        .WithMany()
                        .HasForeignKey("TeamId");
                });
        }
    }
}

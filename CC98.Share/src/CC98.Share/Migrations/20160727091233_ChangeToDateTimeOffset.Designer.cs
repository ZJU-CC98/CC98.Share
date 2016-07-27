using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using CC98.Share.Data;

namespace cc98.share.Migrations
{
    [DbContext(typeof(CC98ShareModel))]
    [Migration("20160727091233_ChangeToDateTimeOffset")]
    partial class ChangeToDateTimeOffset
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.0-rtm-21431")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("CC98.Share.Data.ShareItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<int>("DownloadCount");

                    b.Property<bool>("IsShared");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 50);

                    b.Property<string>("Path");

                    b.Property<long>("Size");

                    b.Property<DateTimeOffset>("UploadTime");

                    b.Property<string>("UserName")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserName", "Size");

                    b.ToTable("Items");
                });
        }
    }
}

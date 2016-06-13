using System;
using CC98.Share.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace cc98.share.Migrations
{
	[DbContext(typeof(CC98ShareModel))]
	internal class CC98ShareModelModelSnapshot : ModelSnapshot
	{
		protected override void BuildModel(ModelBuilder modelBuilder)
		{
			modelBuilder
				.HasAnnotation("ProductVersion", "1.0.0-rc2-20901")
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

				b.Property<long>("TotalSize");

				b.Property<DateTime>("UploadTime");

				b.Property<string>("UserName")
					.IsRequired();

				b.HasKey("Id");

				b.ToTable("Items");
			});
		}
	}
}
namespace CC98.Share.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddDownloadCountForShareItem : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ShareItems", "DownloadCount", c => c.Int(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.ShareItems", "DownloadCount");
        }
    }
}

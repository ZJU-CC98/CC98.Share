namespace CC98.Share.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUploadTimeForShareItem : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ShareItems", "UploadTime", c => c.DateTime(nullable: false));
            CreateIndex("dbo.ShareItems", "IsShared");
        }
        
        public override void Down()
        {
            DropIndex("dbo.ShareItems", new[] { "IsShared" });
            DropColumn("dbo.ShareItems", "UploadTime");
        }
    }
}

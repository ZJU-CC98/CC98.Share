namespace CC98.Share.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Intialize : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ShareItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Description = c.String(),
                        Path = c.String(),
                        UserName = c.String(nullable: false),
                        IsShared = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ShareItems");
        }
    }
}

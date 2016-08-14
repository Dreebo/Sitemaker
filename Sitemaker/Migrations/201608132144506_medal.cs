namespace Sitemaker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class medal : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Medals",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        Site_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Sites", t => t.Site_Id)
                .Index(t => t.Site_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Medals", "Site_Id", "dbo.Sites");
            DropIndex("dbo.Medals", new[] { "Site_Id" });
            DropTable("dbo.Medals");
        }
    }
}

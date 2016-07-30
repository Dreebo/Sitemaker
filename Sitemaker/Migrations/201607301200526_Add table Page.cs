namespace Sitemaker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddtablePage : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Pages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NamePage = c.String(),
                        HtmlCode = c.String(),
                        SiteId = c.String(),
                        Site_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Sites", t => t.Site_Id)
                .Index(t => t.Site_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Pages", "Site_Id", "dbo.Sites");
            DropIndex("dbo.Pages", new[] { "Site_Id" });
            DropTable("dbo.Pages");
        }
    }
}

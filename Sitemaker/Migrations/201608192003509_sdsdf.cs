namespace Sitemaker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class sdsdf : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        UserComment = c.String(),
                        Date = c.DateTime(nullable: false),
                        Site_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Sites", t => t.Site_Id)
                .Index(t => t.Site_Id);
            
            CreateTable(
                "dbo.Sites",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        Logo = c.String(),
                        Name = c.String(),
                        About = c.String(),
                        TemplateId = c.Int(nullable: false),
                        Pablish = c.Boolean(nullable: false),
                        Date = c.DateTime(nullable: false),
                        RatingCount = c.Int(nullable: false),
                        AverageRating = c.Double(nullable: false),
                        Menu_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Menus", t => t.Menu_Id)
                .Index(t => t.Menu_Id);
            
            CreateTable(
                "dbo.Medals",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        Site_Id = c.Int(),
                        UserRating_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Sites", t => t.Site_Id)
                .ForeignKey("dbo.UserRatings", t => t.UserRating_Id)
                .Index(t => t.Site_Id)
                .Index(t => t.UserRating_Id);
            
            CreateTable(
                "dbo.Menus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IsTopBarExicist = c.Boolean(nullable: false),
                        IsSideBarExicist = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MenuItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Link = c.String(),
                        Menu_Id = c.Int(),
                        Menu_Id1 = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Menus", t => t.Menu_Id)
                .ForeignKey("dbo.Menus", t => t.Menu_Id1)
                .Index(t => t.Menu_Id)
                .Index(t => t.Menu_Id1);
            
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
            
            CreateTable(
                "dbo.UserRatings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        Star = c.Int(nullable: false),
                        PhotoUrl = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Tags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 20),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "NameIndex");
            
            CreateTable(
                "dbo.SiteUserRating",
                c => new
                    {
                        SiteId = c.Int(nullable: false),
                        RatingId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.SiteId, t.RatingId })
                .ForeignKey("dbo.Sites", t => t.SiteId, cascadeDelete: true)
                .ForeignKey("dbo.UserRatings", t => t.RatingId, cascadeDelete: true)
                .Index(t => t.SiteId)
                .Index(t => t.RatingId);
            
            CreateTable(
                "dbo.TagSites",
                c => new
                    {
                        Tag_Id = c.Int(nullable: false),
                        Site_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Tag_Id, t.Site_Id })
                .ForeignKey("dbo.Tags", t => t.Tag_Id, cascadeDelete: true)
                .ForeignKey("dbo.Sites", t => t.Site_Id, cascadeDelete: true)
                .Index(t => t.Tag_Id)
                .Index(t => t.Site_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TagSites", "Site_Id", "dbo.Sites");
            DropForeignKey("dbo.TagSites", "Tag_Id", "dbo.Tags");
            DropForeignKey("dbo.SiteUserRating", "RatingId", "dbo.UserRatings");
            DropForeignKey("dbo.SiteUserRating", "SiteId", "dbo.Sites");
            DropForeignKey("dbo.Medals", "UserRating_Id", "dbo.UserRatings");
            DropForeignKey("dbo.Pages", "Site_Id", "dbo.Sites");
            DropForeignKey("dbo.Sites", "Menu_Id", "dbo.Menus");
            DropForeignKey("dbo.MenuItems", "Menu_Id1", "dbo.Menus");
            DropForeignKey("dbo.MenuItems", "Menu_Id", "dbo.Menus");
            DropForeignKey("dbo.Medals", "Site_Id", "dbo.Sites");
            DropForeignKey("dbo.Comments", "Site_Id", "dbo.Sites");
            DropIndex("dbo.TagSites", new[] { "Site_Id" });
            DropIndex("dbo.TagSites", new[] { "Tag_Id" });
            DropIndex("dbo.SiteUserRating", new[] { "RatingId" });
            DropIndex("dbo.SiteUserRating", new[] { "SiteId" });
            DropIndex("dbo.Tags", "NameIndex");
            DropIndex("dbo.Pages", new[] { "Site_Id" });
            DropIndex("dbo.MenuItems", new[] { "Menu_Id1" });
            DropIndex("dbo.MenuItems", new[] { "Menu_Id" });
            DropIndex("dbo.Medals", new[] { "UserRating_Id" });
            DropIndex("dbo.Medals", new[] { "Site_Id" });
            DropIndex("dbo.Sites", new[] { "Menu_Id" });
            DropIndex("dbo.Comments", new[] { "Site_Id" });
            DropTable("dbo.TagSites");
            DropTable("dbo.SiteUserRating");
            DropTable("dbo.Tags");
            DropTable("dbo.UserRatings");
            DropTable("dbo.Pages");
            DropTable("dbo.MenuItems");
            DropTable("dbo.Menus");
            DropTable("dbo.Medals");
            DropTable("dbo.Sites");
            DropTable("dbo.Comments");
        }
    }
}

namespace Sitemaker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class rating1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserRatings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
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
            
            DropColumn("dbo.Sites", "Rating");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Sites", "Rating", c => c.Int(nullable: false));
            DropForeignKey("dbo.SiteUserRating", "RatingId", "dbo.UserRatings");
            DropForeignKey("dbo.SiteUserRating", "SiteId", "dbo.Sites");
            DropIndex("dbo.SiteUserRating", new[] { "RatingId" });
            DropIndex("dbo.SiteUserRating", new[] { "SiteId" });
            DropTable("dbo.SiteUserRating");
            DropTable("dbo.UserRatings");
        }
    }
}

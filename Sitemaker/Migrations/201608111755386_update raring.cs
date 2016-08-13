namespace Sitemaker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateraring : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sites", "RatingCount", c => c.Int(nullable: false));
            AddColumn("dbo.Sites", "AverageRating", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Sites", "AverageRating");
            DropColumn("dbo.Sites", "RatingCount");
        }
    }
}

namespace Sitemaker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class werw : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserRatings", "Star", c => c.Int(nullable: false));
            DropColumn("dbo.UserRatings", "Rating");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserRatings", "Rating", c => c.Boolean(nullable: false));
            DropColumn("dbo.UserRatings", "Star");
        }
    }
}

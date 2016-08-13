namespace Sitemaker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class rating2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserRatings", "Rating", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserRatings", "Rating");
        }
    }
}

namespace Sitemaker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class medal1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Medals", "UserRating_Id", c => c.Int());
            CreateIndex("dbo.Medals", "UserRating_Id");
            AddForeignKey("dbo.Medals", "UserRating_Id", "dbo.UserRatings", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Medals", "UserRating_Id", "dbo.UserRatings");
            DropIndex("dbo.Medals", new[] { "UserRating_Id" });
            DropColumn("dbo.Medals", "UserRating_Id");
        }
    }
}

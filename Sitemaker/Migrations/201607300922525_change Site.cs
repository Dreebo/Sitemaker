namespace Sitemaker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeSite : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sites", "Logo", c => c.String());
            AlterColumn("dbo.Sites", "TemplateId", c => c.Int(nullable: false));
            AlterColumn("dbo.Sites", "MenuId", c => c.Int(nullable: false));
            DropColumn("dbo.Sites", "PathLogo");
            DropColumn("dbo.Sites", "Page");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Sites", "Page", c => c.String());
            AddColumn("dbo.Sites", "PathLogo", c => c.String());
            AlterColumn("dbo.Sites", "MenuId", c => c.String());
            AlterColumn("dbo.Sites", "TemplateId", c => c.String());
            DropColumn("dbo.Sites", "Logo");
        }
    }
}

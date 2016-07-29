namespace Sitemaker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addmenuIdandtemplateId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sites", "About", c => c.String());
            AddColumn("dbo.Sites", "TemplateId", c => c.String());
            AddColumn("dbo.Sites", "MenuId", c => c.String());
            DropColumn("dbo.Sites", "Logo");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Sites", "Logo", c => c.Binary());
            DropColumn("dbo.Sites", "MenuId");
            DropColumn("dbo.Sites", "TemplateId");
            DropColumn("dbo.Sites", "About");
        }
    }
}

namespace Sitemaker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class siteid1234556 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sites", "CreaterId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Sites", "CreaterId");
        }
    }
}

namespace Sitemaker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class migr11 : DbMigration
    {
        public override void Up()
        {
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
            
            AddColumn("dbo.Sites", "Menu_Id", c => c.Int());
            CreateIndex("dbo.Sites", "Menu_Id");
            AddForeignKey("dbo.Sites", "Menu_Id", "dbo.Menus", "Id");
            DropColumn("dbo.Sites", "MenuId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Sites", "MenuId", c => c.Int(nullable: false));
            DropForeignKey("dbo.Sites", "Menu_Id", "dbo.Menus");
            DropForeignKey("dbo.MenuItems", "Menu_Id1", "dbo.Menus");
            DropForeignKey("dbo.MenuItems", "Menu_Id", "dbo.Menus");
            DropIndex("dbo.MenuItems", new[] { "Menu_Id1" });
            DropIndex("dbo.MenuItems", new[] { "Menu_Id" });
            DropIndex("dbo.Sites", new[] { "Menu_Id" });
            DropColumn("dbo.Sites", "Menu_Id");
            DropTable("dbo.MenuItems");
            DropTable("dbo.Menus");
        }
    }
}

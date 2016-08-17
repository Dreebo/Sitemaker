namespace Sitemaker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class asdasd1 : DbMigration
    {
        public override void Up()
        {
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
            DropIndex("dbo.TagSites", new[] { "Site_Id" });
            DropIndex("dbo.TagSites", new[] { "Tag_Id" });
            DropIndex("dbo.Tags", "NameIndex");
            DropTable("dbo.TagSites");
            DropTable("dbo.Tags");
        }
    }
}

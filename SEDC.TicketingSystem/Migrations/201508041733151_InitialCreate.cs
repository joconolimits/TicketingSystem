namespace SEDC.TicketingSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        ModeratorID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.ModeratorID)
                .Index(t => t.ModeratorID);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        LastName = c.String(),
                        Username = c.String(nullable: false),
                        Email = c.String(nullable: false),
                        Password = c.String(nullable: false),
                        IsAdmin = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Replies",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TicketID = c.Int(nullable: false),
                        ReplyBody = c.String(nullable: false),
                        IsAdminMessage = c.Boolean(nullable: false),
                        UserID = c.Int(nullable: false),
                        TimeStamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Tickets", t => t.TicketID)
                .ForeignKey("dbo.Users", t => t.UserID)
                .Index(t => t.TicketID)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.Tickets",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        Body = c.String(nullable: false),
                        Status = c.Byte(nullable: false),
                        OwnerID = c.Int(nullable: false),
                        ModeratorID = c.Int(nullable: false),
                        CategoryID = c.Int(nullable: false),
                        OpenDate = c.DateTime(nullable: false),
                        CloseDate = c.DateTime(nullable: false),
                        WorkHours = c.Int(nullable: false),
                        User_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Categories", t => t.CategoryID)
                .ForeignKey("dbo.Users", t => t.ModeratorID)
                .ForeignKey("dbo.Users", t => t.OwnerID)
                .ForeignKey("dbo.Users", t => t.User_ID)
                .Index(t => t.OwnerID)
                .Index(t => t.ModeratorID)
                .Index(t => t.CategoryID)
                .Index(t => t.User_ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Categories", "ModeratorID", "dbo.Users");
            DropForeignKey("dbo.Tickets", "User_ID", "dbo.Users");
            DropForeignKey("dbo.Replies", "UserID", "dbo.Users");
            DropForeignKey("dbo.Replies", "TicketID", "dbo.Tickets");
            DropForeignKey("dbo.Tickets", "OwnerID", "dbo.Users");
            DropForeignKey("dbo.Tickets", "ModeratorID", "dbo.Users");
            DropForeignKey("dbo.Tickets", "CategoryID", "dbo.Categories");
            DropIndex("dbo.Tickets", new[] { "User_ID" });
            DropIndex("dbo.Tickets", new[] { "CategoryID" });
            DropIndex("dbo.Tickets", new[] { "ModeratorID" });
            DropIndex("dbo.Tickets", new[] { "OwnerID" });
            DropIndex("dbo.Replies", new[] { "UserID" });
            DropIndex("dbo.Replies", new[] { "TicketID" });
            DropIndex("dbo.Categories", new[] { "ModeratorID" });
            DropTable("dbo.Tickets");
            DropTable("dbo.Replies");
            DropTable("dbo.Users");
            DropTable("dbo.Categories");
        }
    }
}

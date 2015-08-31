namespace SEDC.TicketingSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Guid : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "Guid", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "Guid");
        }
    }
}

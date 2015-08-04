namespace SEDC.TicketingSystem.Migrations
{
    using SEDC.TicketingSystem.Models;
    using SEDC.TicketingSystem.Models.Enums;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<SEDC.TicketingSystem.Models.SEDCTicketingSystemContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "SEDC.TicketingSystem.Models.SEDCTicketingSystemContext";
        }

        protected override void Seed(SEDC.TicketingSystem.Models.SEDCTicketingSystemContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //Jordan Adding the Super admin directly in the database on its creation.
            context.Users.AddOrUpdate(
                p => p.Email,
                new User {  
                    Email = "blindcarrots1@gmail.com",
                    Name = "John",
                    LastName = "Doe",
                    Username = "John",
                    IsAdmin = AccessLevel.SuperAdmin,
                    Password = "JohnDoe"
                    }
               );
        }
    }
}

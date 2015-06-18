using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace SEDC.TicketingSystem.Models
{
    public class SEDCTicketingSystemContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public SEDCTicketingSystemContext() : base("name=SEDCTicketingSystemContext")
        {
        }

        // Jordan Remove the all the cascade deletes to avoid foreign key issues
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            base.OnModelCreating(modelBuilder);
        }

        public System.Data.Entity.DbSet<SEDC.TicketingSystem.Models.User> Users { get; set; }

        public System.Data.Entity.DbSet<SEDC.TicketingSystem.Models.Ticket> Tickets { get; set; }
    
    }
}

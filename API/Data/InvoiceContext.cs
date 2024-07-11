using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entity;
using API.Entity.InvoiceAggregate;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class InvoiceContext:IdentityDbContext<User, Role, int>
    {
        public InvoiceContext(DbContextOptions options) :base(options)
       {
        
       }
       public DbSet<Customer> Customers{get;set;}

       public DbSet<Invoice> Invoices{get;set;}

       public DbSet<Counter> Counters { get; set; }
    

        protected override void OnModelCreating(ModelBuilder builder)
       {
            base.OnModelCreating(builder);

            builder.Entity<Invoice>()
                 .HasOne(i => i.Customer)
                 .WithMany(c => c.Invoices)
                 .HasForeignKey(i => i.CustomerId)
                 .OnDelete(DeleteBehavior.Cascade);

            //Customer entity configuration     
            builder.Entity<Customer>()
                .HasMany(c => c.Invoices)
                .WithOne(i => i.Customer)
                .HasForeignKey(i => i.CustomerId);

            builder.Entity<Role>()
                .HasData(
                    new Role{Id=1,Name="Member",NormalizedName="MEMBER"},
                    new Role{Id=2, Name="Admin",NormalizedName="ADMIN"}
            );
       }
    }
}
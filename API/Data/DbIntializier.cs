using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entity;
using API.Entity.InvoiceAggregate;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DbIntializier
    {

        public static async Task IntializeAsync(InvoiceContext context, UserManager<User> userManager)
        {
            
            if(!userManager.Users.Any())
            {
                var user = new User
                {
                    FirstName = "John",
                    LastName = "Smuts",
                    UserName = "johnsmuts",
                    Email = "john@test.com",
                    Company = "Tobetsa"
                };

                await userManager.CreateAsync(user,"Pa$$w0rd");
                await userManager.AddToRoleAsync(user,"Member");

                var admin = new User
                {
                    FirstName = "Admin",
                    LastName = "Admin",
                    UserName = "Madmin223",
                    Email = "admin@tobetsa.co.za",
                    Company = "Tobetsa"
                };

                await userManager.CreateAsync(admin,"D1sser@345");
                await userManager.AddToRolesAsync(admin,new[] {"Member","Admin"});
            }

            //Customer
            if(context.Customers.Any())return;

            var customers = new List<Customer>
            {
                new Customer
                {
                    FullName = "John Smith",
                    Company= "VVET",
                    Email = "johnsmith@VVET.com",
                    Phone = "0823456789"
                },new Customer
                {
                    FullName = "Mark Peru",
                    Company= "OOER",
                    Email = "johnsmith@OOER.com",
                    Phone = "0823456789"
                }

            };
            foreach(var customer in customers)
            {
                context.Customers.Add(customer);
            }

            var counter = await context.Counters.FirstOrDefaultAsync();

              if (counter == null)
            {
            counter = new Counter { CounterValue = 100 };
            context.Counters.Add(counter);
            await context.SaveChangesAsync();
            }

            context.SaveChanges();
            
        }


    }
}
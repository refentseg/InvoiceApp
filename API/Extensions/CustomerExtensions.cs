using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTO;
using API.Entity;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class CustomerExtensions
    {
        public static IQueryable<CustomerDto> ProjectCustomerToCustomerDto(this IQueryable<Customer> query)
        {
            return query
            .Select(customer => new CustomerDto
            {
            Id=customer.Id,
            FullName=customer.FullName,
            Company = customer.Company,
            Email = customer.Email,
            Phone = customer.Phone
            }).AsNoTracking();
        }

        public static IQueryable<Customer> Sort(this IQueryable<Customer> query,string orderBy)
        {
            if(string.IsNullOrWhiteSpace(orderBy)) return query.OrderBy(c =>c.Id);//Alphabetical method
            query = orderBy switch
            {
                _ => query.OrderBy(c=>c.Id)
            };
            return query;
        }

        //Searching for a customer
        public static IQueryable<Customer> Search(this IQueryable<Customer> query,string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm)) return query;

            var lowerCaseSearchTerm = searchTerm.Trim().ToLower();

            return query.Where(c=>
            c.FullName.ToLower().Contains(lowerCaseSearchTerm) ||
            c.Company.ToLower().Contains(lowerCaseSearchTerm)
            );
        }
    }
}
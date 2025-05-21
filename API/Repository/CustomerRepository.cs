using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTO;
using API.Entity;
using API.Extensions;
using API.RequestHelpers;

namespace API.Repository
{
    public class CustomerRepository:ICustomerRepository
    {
        private readonly InvoiceContext _context;

        public CustomerRepository(InvoiceContext context)
        {
            _context = context;
        }

        // Get all Customers
        public async Task<PagedList<CustomerDto>> GetCustomersAsync(CustomerParams customerParams)
        {
            var query = _context.Customers
                .Sort(customerParams.OrderBy)
                .Search(customerParams.SearchTerm)
                .AsQueryable();

            return await PagedList<CustomerDto>.ToPagedList(
                query.ProjectCustomerToCustomerDto(),
                customerParams.PageNumber,
                customerParams.PageSize);
        }

        // Get Customer
        public async Task<Customer> GetCustomerByIdAsync(int id)
        {
            return await _context.Customers.FindAsync(id);
        }

        // Add Customer
        public async Task<Customer> AddCustomerAsync(Customer customer)
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        // Delete Customer
        public async Task<bool> DeleteCustomerAsync(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return false;
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
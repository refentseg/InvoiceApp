using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTO;
using API.Entity;
using API.Extensions;
using API.RequestHelpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    public class CustomerController:BaseApiController
    {
        private readonly InvoiceContext _context;
        public CustomerController(InvoiceContext context)
        {
            _context = context;

        }
        [HttpGet(Name ="GetCustomers")]

        public async Task<ActionResult<PagedList<CustomerDto>>> GetCustomers([FromQuery]CustomerParams customerParams)
        {
            var query = _context.Customers
                .Sort(customerParams.OrderBy)
                .Search(customerParams.SearchTerm)
                .AsQueryable();  
            
            var customers = await PagedList<CustomerDto>.ToPagedList(query.ProjectCustomerToCustomerDto(),customerParams.PageNumber,customerParams.PageSize);

            Response.AddPaginationHeader(customers.MetaData);
            return customers;
        }

        [HttpGet("{id}",Name ="GetCustomer")]

        public ActionResult<Customer> GetCustomer(int id)
        {
            var customer = _context.Customers.Find(id);

            if(customer == null) return NotFound();

            return customer;
        }

        [HttpPost(Name = "CreateCustomer")]
        public async Task<ActionResult<Customer>> CreateCustomer(CreateCustomerDto newCustomerDto)
        {
        if (newCustomerDto == null)
        {
        return BadRequest("Invalid customer data");
        }

        var newCustomer = new Customer
        {
            FullName = newCustomerDto.FullName,
            Company = newCustomerDto.Company,
            Email = newCustomerDto.Email,
            Phone = newCustomerDto.Phone
        
        };
        _context.Customers.Add(newCustomer);
        await _context.SaveChangesAsync();

        // Returning 201 Created status along with the created customer
        return CreatedAtRoute("GetCustomer", new { id = newCustomer.Id }, newCustomer);
        }
        [HttpDelete("{id}",Name = "DeleteCustomer")]
        public async Task<ActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if(customer == null){
                return NotFound();
            }
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            
            return NoContent();
        }
        
    }

    

}
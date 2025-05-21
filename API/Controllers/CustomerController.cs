using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTO;
using API.Entity;
using API.Extensions;
using API.Repository;
using API.RequestHelpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    /// <summary>
    /// Handles customer-related operations such as retrieving, creating, and deleting customers.
    /// Requires authentication.
    /// </summary>
    [Authorize]
    public class CustomerController:BaseApiController
    {
        private readonly ICustomerRepository _customerRepository;
        public CustomerController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;

        }
        
        /// <summary>
        /// Retrieves a paginated list of customers based on filter and pagination parameters.
        /// </summary>
        /// <param name="customerParams">Pagination and filter parameters.</param>
        /// <returns>A paginated list of customer DTOs.</returns>
        [HttpGet(Name = "GetCustomers")]
        public async Task<ActionResult<PagedList<CustomerDto>>> GetCustomers(
            [FromQuery]CustomerParams customerParams)
        {
            var customers = await _customerRepository.GetCustomersAsync(customerParams);
            Response.AddPaginationHeader(customers.MetaData);
            return customers;
        }

        /// <summary>
        /// Retrieves a specific customer by their unique identifier.
        /// </summary>
        /// <param name="id">The ID of the customer to retrieve.</param>
        /// <returns>The customer entity if found; otherwise, a 404 Not Found.</returns>
        [HttpGet("{id}",Name ="GetCustomer")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            var customer = await _customerRepository.GetCustomerByIdAsync(id);
            
            if (customer == null) return NotFound();

            return customer;
        }

        /// <summary>
        /// Creates a new customer using the provided customer data.
        /// </summary>
        /// <param name="newCustomerDto">DTO containing the new customer's information.</param>
        /// <returns>The created customer with a 201 Created status, or 400 Bad Request if input is invalid.</returns>
        [HttpPost(Name = "CreateCustomer")]
        public async Task<ActionResult<Customer>> CreateCustomer(
            CreateCustomerDto newCustomerDto)
        {
        // checks if the newCustomerDto is null
        if (newCustomerDto == null)
        {
            // Returning 400 Bad Request status if the newCustomerDto is null
            return BadRequest("Invalid customer data");
        }
        
        var newCustomer = new Customer
        {
            FullName = newCustomerDto.FullName,
            Company = newCustomerDto.Company,
            Email = newCustomerDto.Email,
            Phone = newCustomerDto.Phone
        
        };
        
        var result = await _customerRepository.AddCustomerAsync(newCustomer);

        // Returning 201 Created status along with the created customer
            return CreatedAtRoute("GetCustomer", new { id = result.Id },
            result);
        }

        /// <summary>
        /// Deletes a specific customer by their unique identifier.
        /// </summary>
        /// <param name="id">The ID of the customer to delete.</param>
        /// <returns>No content if successful, or 404 Not Found if the customer does not exist.</returns>
        [HttpDelete("{id}",Name = "DeleteCustomer")]
        public async Task<ActionResult> DeleteCustomer(int id)
        {
            var result = await _customerRepository.DeleteCustomerAsync(id);
            if (!result) return NotFound();    
            return NoContent();
        }
        
    }  

}
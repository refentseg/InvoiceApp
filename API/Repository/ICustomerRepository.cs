using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTO;
using API.Entity;
using API.RequestHelpers;

namespace API.Repository
{
    public interface ICustomerRepository
    {
        Task<PagedList<CustomerDto>> GetCustomersAsync(CustomerParams customerParams);
        Task<Customer> GetCustomerByIdAsync(int id);
        Task<Customer> AddCustomerAsync(Customer customer);
        Task<bool> DeleteCustomerAsync(int id);
    }
}
using MAUIClient.DTO.Customer;
using MAUIClient.Models;
using MAUIClient.Models.RequestHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIClient.Services
{
    public interface ICustomerService
    {
        Task<PagedResponse<Customer>> SearchCustomersAsync(string searchTerm, int page=1, int pageSize=10);
    }
}

using MAUIClient.Models.Auth;
using MAUIClient.Models.InvoiceAggregate;
using MAUIClient.Models.RequestHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIClient.Services
{
    public interface IRestService
    {
        Task<User> LoginAsync(string username, string password);
        Task<PagedResponse<Invoice>> RefreshDataAsync(int pageNumber = 1,
            int pageSize = 10,
            string orderBy = "orderDate");
    }
}

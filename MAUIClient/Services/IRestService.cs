using MAUIClient.Models.Auth;
using MAUIClient.Models.InvoiceAggregate;
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
        Task<List<Invoice>> RefreshDataAsync();
    }
}

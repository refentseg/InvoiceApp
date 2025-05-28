using MAUIClient.Models.InvoiceAggregate;
using MAUIClient.Models.RequestHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIClient.Services
{
    public interface IInvoiceService
    {
        Task<PagedResponse<Invoice>> GetInvoicesAsync();
    }
}

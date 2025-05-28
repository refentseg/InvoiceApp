using MAUIClient.Models.InvoiceAggregate;
using MAUIClient.Models.RequestHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIClient.Services
{
    public class InvoiceService:IInvoiceService
    {
        IRestService _restService;

        public InvoiceService(IRestService service)
        {
            _restService = service;
        }

        public Task<PagedResponse<Invoice>> GetInvoicesAsync()
        {
            return _restService.RefreshDataAsync();
        }
    }
}

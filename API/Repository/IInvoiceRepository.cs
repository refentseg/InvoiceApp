using API.DTO;
using API.Entity.InvoiceAggregate;
using API.RequestHelpers;
using System.Threading.Tasks;

namespace API.Repository
{
    public interface IInvoiceRepository
    {
        Task<PagedList<InvoiceDto>> GetInvoices(InvoiceParams invoiceParams);
        Task<InvoiceDto> GetInvoice(string id);
        Task<Invoice> GetInvoiceWithItemsAsync(string id);
        Task<List<string>> GetFilters();
        Task<string> GetNextInvoiceNumber();
        Task<string> CreateInvoice(CreateInvoiceDto invoiceDto);
        Task<Invoice> UpdateInvoice(string id, UpdateInvoiceDto updateDto);
        Task DeleteInvoice(string id);
       
    }
}

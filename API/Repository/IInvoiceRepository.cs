using API.DTO;
using API.Entity;
using API.Entity.InvoiceAggregate;
using API.RequestHelpers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API.Repository
{
    public interface IInvoiceRepository
    {
        Task<PagedList<InvoiceDto>> GetInvoices(InvoiceParams invoiceParams, User currentUser);
        Task<InvoiceDto> GetInvoice(string id);
        Task<Invoice> GetInvoiceWithItemsAsync(string id);
        Task<List<string>> GetFilters();
        Task<string> GetNextInvoiceNumber();
        Task<string> CreateInvoice(CreateInvoiceDto invoiceDto, User currentUser);
        Task<Invoice> UpdateInvoice(string id, UpdateInvoiceDto updateDto);
        Task DeleteInvoice(string id);
       
    }
}

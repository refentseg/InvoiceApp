using API.Data;
using API.DTO;
using API.Entity.InvoiceAggregate;
using API.Entity;
using API.Extensions;
using API.RequestHelpers;
using Microsoft.EntityFrameworkCore;

namespace API.Repository
{
    public class InvoiceRepository:IInvoiceRepository
    {
        private readonly InvoiceContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<InvoiceRepository> _logger;

        public InvoiceRepository(InvoiceContext context, IHttpContextAccessor httpContextAccessor, ILogger<InvoiceRepository> logger)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }
        //Get all Invoices
        public async Task<PagedList<InvoiceDto>> GetInvoices(InvoiceParams invoiceParams)
        {
            var query = _context.Invoices
                .Sort(invoiceParams.OrderBy)
                .Search(invoiceParams.SearchTerm)
                .Filter(invoiceParams.Status)
                .AsQueryable();

            var invoices = await PagedList<InvoiceDto>.ToPagedList(
                query.ProjectInvoiceToInvoiceDto(),
                invoiceParams.PageNumber,
                invoiceParams.PageSize);

            _httpContextAccessor.HttpContext.Response.AddPaginationHeader(invoices.MetaData);

            return invoices;
        }
        //Get Invoice
        public async Task<InvoiceDto> GetInvoice(string id)
        {
            return await _context.Invoices
                .ProjectInvoiceToInvoiceDto()
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
        }
        public async Task<Invoice> GetInvoiceWithItemsAsync(string id)
        {
            return await _context.Invoices
                .Include(i => i.Items)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        //Get Filters
        public async Task<List<string>> GetFilters()
        {
            return await _context.Invoices.Select(i => i.InvoiceStatus.ToString()).Distinct().ToListAsync();
        }

        //Geting next Invoice number
        public async Task<string> GetNextInvoiceNumber()
        {
            int currentIdCounter = await GetCurrentIdCounter();
            return "INVTBT" + currentIdCounter.ToString("D3");
        }

        //Creating Next Invoice
        public async Task<string> CreateInvoice(CreateInvoiceDto invoiceDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (invoiceDto == null || invoiceDto.Items == null || !invoiceDto.Items.Any())
                {
                    throw new ArgumentException("Invalid invoice data");
                }

                var invoiceItems = invoiceDto.Items.Select(itemDto => new InvoiceItem
                {
                    Name = itemDto.Name,
                    Amount = itemDto.Amount,
                    Quantity = itemDto.Quantity
                }).ToList();

                Customer customer;

                if (invoiceDto.ExistingCustomer)
                {
                    customer = await _context.Customers.FindAsync(invoiceDto.CustomerId);
                    if (customer == null)
                    {
                        throw new KeyNotFoundException("Existing customer not found");
                    }
                }
                else
                {
                    customer = new Customer
                    {
                        FullName = invoiceDto.Customer.FullName,
                        Company = invoiceDto.Customer.Company,
                        Email = invoiceDto.Customer.Email,
                        Phone = invoiceDto.Customer.Phone
                    };
                    _context.Customers.Add(customer);
                }

                var subtotal = invoiceItems.Sum(item => item.Amount * item.Quantity);

                var invoice = new Invoice
                {
                    Id = await GenerateCustomId(),
                    Items = invoiceItems,
                    SalesRep = _httpContextAccessor.HttpContext.User.Identity.Name,
                    CustomerId = customer.Id,
                    Customer = customer,
                    Subtotal = subtotal
                };

                _context.Invoices.Add(invoice);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return invoice.Id;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Unexpected error: {ErrorMessage}", ex.Message);
                throw;
            }
        }

        public async Task<Invoice> UpdateInvoice(string id, UpdateInvoiceDto updateDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var invoice = await _context.Invoices
                    .Include(i => i.Items)
                    .FirstOrDefaultAsync(i => i.Id == id);

                if (invoice == null)
                {
                    throw new KeyNotFoundException("Invoice not found");
                }

                Customer customer;
                if (updateDto.ExistingCustomer)
                {
                    customer = await _context.Customers.FindAsync(updateDto.CustomerId);
                    if (customer == null)
                    {
                        throw new KeyNotFoundException("Existing customer not found");
                    }
                }
                else
                {
                    customer = new Customer
                    {
                        FullName = updateDto.Customer.FullName,
                        Company = updateDto.Customer.Company,
                        Email = updateDto.Customer.Email,
                        Phone = updateDto.Customer.Phone
                    };
                    _context.Customers.Add(customer);
                }

                var updatedItems = new List<InvoiceItem>();
                foreach (var itemDto in updateDto.Items)
                {
                    var existingItem = invoice.Items.FirstOrDefault(i => i.Id == itemDto.Id);
                    if (existingItem != null)
                    {
                        existingItem.Name = itemDto.Name;
                        existingItem.Amount = itemDto.Amount;
                        existingItem.Quantity = itemDto.Quantity;
                        updatedItems.Add(existingItem);
                    }
                    else
                    {
                        var newItem = new InvoiceItem
                        {
                            Name = itemDto.Name,
                            Amount = itemDto.Amount,
                            Quantity = itemDto.Quantity
                        };
                        updatedItems.Add(newItem);
                    }
                }

                invoice.Items = updatedItems;

                if (!Enum.TryParse<InvoiceStatus>(updateDto.Status, true, out var invoiceStatus) ||
                    !EnumHelper.GetEnumValues<InvoiceStatus>().Contains(updateDto.Status))
                {
                    throw new ArgumentException("Invalid invoice status");
                }

                invoice.CustomerId = customer.Id;
                invoice.Customer = customer;
                invoice.InvoiceStatus = invoiceStatus;
                invoice.Subtotal = invoice.Items.Sum(item => item.Amount * item.Quantity);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return invoice;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Unexpected error: {ErrorMessage}", ex.Message);
                throw;
            }
        }
        public async Task DeleteInvoice(string id)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Items)
                .FirstOrDefaultAsync(i => i.Id == id);
            if (invoice == null)
            {
                throw new KeyNotFoundException("Invoice not found");
            }
            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();
        }

        private async Task<int> GetCurrentIdCounter()
        {
            var counter = await _context.Counters.FirstOrDefaultAsync();
            if (counter == null)
            {
                counter = new Counter { CounterValue = 0 };
                _context.Counters.Add(counter);
                await _context.SaveChangesAsync();
            }
            return counter.CounterValue;
        }

        private async Task IncrementIdCounter()
        {
            var counter = await _context.Counters.FirstOrDefaultAsync();
            if (counter != null)
            {
                counter.CounterValue++;
                await _context.SaveChangesAsync();
            }
        }

        private async Task<string> GenerateCustomId()
        {
            int currentIdCounter = await GetCurrentIdCounter();
            await IncrementIdCounter();
            return "INVTBT" + currentIdCounter.ToString("D3");
        }
    }
}

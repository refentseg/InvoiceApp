using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Schema;
using API.Data;
using API.DTO;
using API.Entity;
using API.Entity.InvoiceAggregate;
using API.Extensions;
using API.RequestHelpers;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers
{
    [Authorize]
    public class InvoicesController:BaseApiController
    {
        private readonly InvoiceContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<InvoicesController> _logger;
       
        public InvoicesController(InvoiceContext context,ILogger<InvoicesController> logger,UserManager<User> userManager)
        {
            _context = context;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userManager = userManager;
        }
    
    [HttpGet]
    public async Task<ActionResult<PagedList<InvoiceDto>>> GetInvoices([FromQuery]InvoiceParams invoiceParams)
    {
        var query = _context.Invoices
            .Sort(invoiceParams.OrderBy)
            .Search(invoiceParams.SearchTerm)
            .Filter(invoiceParams.Status)
            .AsQueryable();

            var invoices = await PagedList<InvoiceDto>.ToPagedList(query.ProjectInvoiceToInvoiceDto(), invoiceParams.PageNumber, invoiceParams.PageSize);

            Response.AddPaginationHeader(invoices.MetaData);

            return invoices;
    }

    [HttpGet("{id}",Name ="GetInvoice")]
    public async Task<ActionResult<InvoiceDto>> GetInvoice(string id)
    {
        return await _context.Invoices
            .ProjectInvoiceToInvoiceDto()
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync();

    }
    [HttpGet("filters")]
    public async Task<IActionResult> GetFilters()
    {
        var status = await _context.Invoices.Select(i =>i.InvoiceStatus).Distinct().ToListAsync();

        return Ok(new{status});
    }
    //Pre-Invoice Number
    [HttpGet("next")]
    public async Task<IActionResult> GetNextInvoiceNumber()
    {
        int currentIdCounter = await GetCurrentIdCounter();
        string invoiceNumber = "INVTBT" + currentIdCounter.ToString("D3");
        
        // Return the invoice number without updating the counter
        return Ok(new InvoiceNumberDto { InvoiceNumber = invoiceNumber });
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


    //Create Invoice

    [HttpPost(Name="CreateInvoice")]
    public async Task<ActionResult<Invoice>> CreateInvoice(CreateInvoiceDto invoiceDto)
    {
        
        try
        {
            using var transaction = _context.Database.BeginTransaction();
            if (invoiceDto == null || invoiceDto.Items == null || !invoiceDto.Items.Any())
            {
                return BadRequest(new ProblemDetails { Title = "Invalid invoice data" });
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
                        return NotFound(new ProblemDetails { Title = "Existing customer not found" });
                    }
                
                
            }
            else
            {
                // If it's a new customer, create a new one
                customer = new Customer
                {
                    FullName = invoiceDto.Customer.FullName,
                    Company = invoiceDto.Customer.Company,
                    Email = invoiceDto.Customer.Email,
                    Phone = invoiceDto.Customer.Phone
                };

                //  the new customer to the context
                _context.Customers.Add(customer);
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized(new ProblemDetails { Title = "Unable to identify the current user" });
            }

            string salesRepFullName = $"{currentUser.FirstName} {currentUser.LastName}".Trim();

            var subtotal = invoiceItems.Sum(item => item.Amount * item.Quantity);

                    var invoice = new Invoice
                    {
                        Id =  await GenerateCustomId(),
                        Items = invoiceItems,
                        SalesRep = salesRepFullName,
                        CustomerId = customer.Id,
                        Customer = customer,
                        Subtotal = subtotal
                    };

                    
                    _context.Invoices.Add(invoice);
                    
                    var result = await _context.SaveChangesAsync();

                    if (result > 0)
                    {
                        transaction.Commit();
                        return CreatedAtRoute("GetInvoice", new { id = invoice.Id }, invoice.Id);
                    }

                    transaction.Rollback();
                    return BadRequest("Problem creating invoice");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error: {ErrorMessage}", ex.Message);
            return StatusCode(500, ex.Message);
        }
    }
    [HttpDelete]
    public async Task<ActionResult> DeleteInvoice(string id)
    {
        var invoice = await _context.Invoices.FindAsync(id);
        if (invoice == null)
        {
            return NotFound();
        }
        _context.Invoices.Remove(invoice);

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPut(Name = "UpdateInvoice")]
    public async Task<ActionResult<Invoice>> UpdateInvoice(UpdateInvoiceDto updateDto)
    {
        try
        {
            using var transaction = _context.Database.BeginTransaction();

            // Retrieve the selected invoice
            var invoice = await _context.Invoices
                .Include(i => i.Items)
                .FirstOrDefaultAsync(i => i.Id == updateDto.Id);

            if (invoice == null)
            {
                return NotFound(new ProblemDetails { Title = "Invoice not found" });
            }

            // Update customer if needed
            Customer customer;
            if (updateDto.ExistingCustomer)
            {
                customer = await _context.Customers.FindAsync(updateDto.CustomerId);
                if (customer == null)
                {
                    return NotFound(new ProblemDetails { Title = "Existing customer not found" });
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

            var invoiceItems = updateDto.Items.Select(itemDto => new InvoiceItem
            {
                Name = itemDto.Name,
                Amount = itemDto.Amount,
                Quantity = itemDto.Quantity
            }).ToList();

            if (!Enum.TryParse<InvoiceStatus>(updateDto.Status, true, out var invoiceStatus) ||
            !EnumHelper.GetEnumValues<InvoiceStatus>().Contains(updateDto.Status))
            {
                return BadRequest(new ProblemDetails { Title = "Invalid invoice status" });
            }

            // Update invoice details
            invoice.CustomerId = customer.Id;
            invoice.Customer = customer;
            invoice.Items = invoiceItems;
            invoice.InvoiceStatus = invoiceStatus;

            var subtotal = invoice.Items.Sum(item => item.Amount * item.Quantity);
            invoice.Subtotal = subtotal;

            // Save changes
            await _context.SaveChangesAsync();

            transaction.Commit();

            return Ok(invoice);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error: {ErrorMessage}", ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    // private string CurrencyFormat(long amount)
    // {
    //     long randAmount = (long)amount / 100;

    //     string formattedAmount = string.Format("R{0:N2}", randAmount);

    //     return formattedAmount;
    // }
    }
}
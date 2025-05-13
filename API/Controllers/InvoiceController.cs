using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Xml.Schema;
using API.Data;
using API.DTO;
using API.Entity;
using API.Entity.InvoiceAggregate;
using API.Extensions;
using API.Repository;
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
    public class InvoiceController:BaseApiController
    {
        private readonly IInvoiceRepository _invoiceRepository;
        

        public InvoiceController(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository=invoiceRepository;
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<InvoiceDto>>> GetInvoices(
            [FromQuery]InvoiceParams invoiceParams)
        {
               // Get invoices for the current user
                var invoices = await _invoiceRepository.GetInvoices(
                    invoiceParams);
                // Add pagination headers to the response
                Response.AddPaginationHeader(invoices.MetaData);
                // Return the paginated invoices
                return Ok(invoices);
        }


        [HttpGet("{id}",Name ="GetInvoice")]
        public async Task<ActionResult<InvoiceDto>> GetInvoice(string id)
        {
            // Get the invoice with the specified ID
            var invoice = await _invoiceRepository.GetInvoice(id);
            // Return Invoice
            return Ok(invoice);

        }

        [HttpGet("filters")]
        public async Task<IActionResult> GetFilters()
        {
            // Get filters for the invoice
            var filters =await _invoiceRepository.GetFilters();
            return Ok(filters);
        }

        //Pre-Invoice Number
        [HttpGet("next")]
        public async Task<IActionResult> GetNextInvoiceNumber()
        {
             string invoiceNumber = await _invoiceRepository.GetNextInvoiceNumber(); 
            // Return the invoice number without updating the counter
            return Ok(new InvoiceNumberDto { InvoiceNumber = invoiceNumber });
        }


        //Create Invoice

        [HttpPost(Name="CreateInvoice")]
        public async Task<ActionResult<Invoice>> CreateInvoice(CreateInvoiceDto invoiceDto)
        {
            // Check if the invoice data is null
            if (invoiceDto == null)
            {
                return BadRequest("Invoice data is required.");
            }
            // Retrive id for the new invoice
            var invoiceId = await _invoiceRepository.CreateInvoice(invoiceDto);

            // Check if the invoice was created successfully
            if (invoiceId == null)
            {
                return BadRequest("Problem creating invoice.");
            }
            // Return the created invoice ID
            return Ok(new{id = invoiceId});
        }
        

        [HttpPut("{id}",Name = "UpdateInvoice")]
        public async Task<ActionResult<Invoice>> UpdateInvoice(string id,UpdateInvoiceDto updateDto)
        {
            // Check if the data to update is null
            if (updateDto == null)
            {
                return BadRequest(new ProblemDetails { Title = "Invalid update data" });
            }

            // Update the invoice with the specified ID
            try
            {
                var invoice = await _invoiceRepository.UpdateInvoice(id, updateDto);
                return Ok(new
                {
                    invoice.Id,
                    invoice.CustomerId,
                    Items = invoice.Items.Select(i => new { i.Id, i.Name, i.Amount, i.Quantity }),
                    invoice.InvoiceStatus,
                    invoice.Subtotal
                });
            }
            // Handle specific exceptions
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ProblemDetails { Title = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ProblemDetails { Title = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ProblemDetails { Title = $"An unexpected error occurred. {ex}" });
            }
        }
        [HttpDelete("{id}", Name = "Delete Invoice")]
        public async Task<ActionResult> DeleteInvoice(string id)
        {
            //Check if invoice exists
            var invoice = await _invoiceRepository.GetInvoiceWithItemsAsync(id);
            if (invoice == null)
            {
                return NotFound(); // Return 404 if invoice not found
            }

            // Delete the invoice
            await _invoiceRepository.DeleteInvoice(invoice.Id);
            
            Console.WriteLine($"Invoice {invoice.Id} deleted successfully.");
            // Return 204 No Content if deletion is successful
            return NoContent();
        }
    }
}
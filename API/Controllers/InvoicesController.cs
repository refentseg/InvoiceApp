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
    public class InvoicesController:BaseApiController
    {
        private readonly IInvoiceRepository _invoiceRepository;
        

        public InvoicesController(InvoiceContext context,UserManager<User> userManager, IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository=invoiceRepository;
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<InvoiceDto>>> GetInvoices([FromQuery]InvoiceParams invoiceParams)
        {
                return await _invoiceRepository.GetInvoices(invoiceParams);
        }


        [HttpGet("{id}",Name ="GetInvoice")]
        public async Task<ActionResult<InvoiceDto>> GetInvoice(string id)
        {
            return await _invoiceRepository.GetInvoice(id);

        }

        [HttpGet("filters")]
        public async Task<IActionResult> GetFilters()
        {
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

            if (invoiceDto == null)
            {
                return BadRequest("Invoice data is required.");
            }

            var invoiceId = await _invoiceRepository.CreateInvoice(invoiceDto);

            if (invoiceId == null)
            {
                return BadRequest("Problem creating invoice.");
            }

            return Ok(new{id = invoiceId});
        }
        

        [HttpPut("{id}",Name = "UpdateInvoice")]
        public async Task<ActionResult<Invoice>> UpdateInvoice(string id,UpdateInvoiceDto updateDto)
        {

            if (updateDto == null)
            {
                return BadRequest(new ProblemDetails { Title = "Invalid update data" });
            }

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
            var invoice = await _invoiceRepository.GetInvoiceWithItemsAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }

            await _invoiceRepository.DeleteInvoice(invoice.Id);

            return NoContent();
        }
    }
}
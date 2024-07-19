using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTO;
using API.Entity.InvoiceAggregate;
using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class ChartsController:BaseApiController
    {
        private readonly InvoiceContext _context;

        public ChartsController(InvoiceContext context)
        {
            _context = context;
        }

        [HttpGet("total-sum")]
        public async Task<long> GetInvoicesSum()
        {
            var invoices = await _context.Invoices.ToListAsync();
            long totalSum = invoices.Select(i => i.GetTotal()/100).Sum();
            return totalSum;
        }
        [HttpGet("total-sum-per-month")]
        public async Task<ActionResult<IDictionary<string, long>>> GetTotalSumPerMonth()
        {
            var invoices = await _context.Invoices.ToListAsync();
            var totalSumPerMonth = invoices
                .GroupBy(i => new { Year = i.OrderDate.Year, Month = i.OrderDate.Month })
                .Select(g => new
                {
                    MonthYear = $"{g.Key.Month}/{g.Key.Year}",
                    TotalSum = g.Sum(i => i.GetTotal()/100)
                })
                .ToDictionary(g => g.MonthYear, g => g.TotalSum);

            return Ok(totalSumPerMonth);
        }

        [HttpGet("sum-by-status")]
        public async Task<ActionResult<IDictionary<string, long>>> GetTotalSumByInvoiceStatus()
        {
            var statusValues = Enum.GetValues(typeof(InvoiceStatus)).Cast<InvoiceStatus>().ToArray();
            var invoices = await _context.Invoices.ToListAsync();
            var sumByStatus = invoices
                .GroupBy(i => i.InvoiceStatus)
                .Where(g => statusValues.Contains(g.Key))
                .Select(g => new { Status = g.Key.ToString(), TotalSum = g.Sum(i => i.GetTotal()/100) })
                .ToDictionary(g => g.Status, g => g.TotalSum);
                
            return Ok(sumByStatus);
        }
    }
}
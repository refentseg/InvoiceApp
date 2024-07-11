using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTO;
using API.Entity;
using API.Entity.InvoiceAggregate;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace API.Extensions
{
    public static class InvoiceExtensions
    {
        public static CustomerDto MapToDto(Customer customer)
        {
            return new CustomerDto{
            Id=customer.Id,
            FullName=customer.FullName,
            Company = customer.Company,
            Email = customer.Email,
            Phone = customer.Phone
            };
        }

        //Converting Enity to readable data
        public static IQueryable<InvoiceDto> ProjectInvoiceToInvoiceDto(this IQueryable<Invoice> query)
        {
            return query
                .Select(invoice => new InvoiceDto
                {
                    Id = invoice.Id,
                    CustomerId = invoice.CustomerId,
                    Customer = invoice.Customer != null ? MapToDto(invoice.Customer) : new CustomerDto(),
                    
                    SalesRep = invoice.SalesRep,
                    OrderDate = invoice.OrderDate,
                    Subtotal = invoice.Subtotal,
                    Vat = invoice.GetVat(),
                    Total = invoice.GetTotal(),
                    InvoiceStatus = invoice.InvoiceStatus.ToString(),

                    Items = invoice.Items.Select(item => new InvoiceItemDto
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Quantity =item.Quantity,
                        Amount = item.Amount
                    }).ToList()

                }).AsNoTracking();
        }

        public static IQueryable<Invoice> Sort(this IQueryable<Invoice> query,string orderBy)
        {
            if(string.IsNullOrWhiteSpace(orderBy)) return query.OrderBy(i =>i.Id);//Alphabetical method
            query = orderBy switch
            {
                "price" => query.OrderBy(p=>p.GetTotal()),
                "priceDesc" =>query.OrderByDescending(p=>p.GetTotal()),
                "date" => query.OrderBy(i => i.OrderDate),
                "dateDesc" => query.OrderByDescending(i => i.OrderDate),
                _ => query.OrderBy(i=>i.Id)
            };
            return query;
        }

        //For filtering
        public static IQueryable<Invoice> Filter(this IQueryable<Invoice> query, string status)
        {
            var statusList = new List<InvoiceStatus>();

            if(!string.IsNullOrEmpty(status))
            {
                var statusStrings = status.ToLower().Split(",");

                foreach(var statusString in statusStrings)
                {
                    if (Enum.TryParse(typeof(InvoiceStatus), statusString, true, out var parsedStatus))
                    {
                    statusList.Add((InvoiceStatus)parsedStatus);
                    }
                }
            }
                


            query = query.Where(i =>statusList.Count ==0 || statusList.Contains(i.InvoiceStatus));
            
            return query;
        }

        //For searching

        public static IQueryable<Invoice> Search(this IQueryable<Invoice> query, string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm)) return query;

            var lowerCaseSearchTerm = searchTerm.Trim().ToLower();

            return query.Where(i=>i.Id.ToLower().Contains(lowerCaseSearchTerm) ||
            i.Customer.FullName.ToLower().Contains(lowerCaseSearchTerm) ||
            i.Customer.Company.ToLower().Contains(lowerCaseSearchTerm) ||
            i.SalesRep.ToLower().Contains(lowerCaseSearchTerm)
            );
        }

        public static string CurrencyFormat(long amount)
        {
        double randAmount = (double)amount / 100;

        string formattedAmount = string.Format("R{0:N2}", randAmount);

        return formattedAmount;
        }

    }



}

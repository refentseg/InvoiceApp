using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entity;
using API.Entity.InvoiceAggregate;

namespace API.DTO
{
    public class UpdateInvoiceDto
    {
        public string Id{get;set;}
        public Customer Customer {get;set;}

        public List<InvoiceItemDto> Items {get;set;} 

        public int CustomerId { get; set; }
        public bool ExistingCustomer { get; set; }
        public string Status {get;set;}
    }
}
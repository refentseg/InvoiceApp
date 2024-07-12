using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entity;
using API.Entity.InvoiceAggregate;

namespace API.DTO
{
    public class CreateInvoiceDto
    {
        public bool ExistingCustomer { get; set; }
        public CustomerDto Customer {get;set;}

        public int CustomerId { get; set; }

        public List<InvoiceItemDto> Items {get;set;} 
        
    }
}
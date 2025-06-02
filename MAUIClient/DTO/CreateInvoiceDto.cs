using MAUIClient.DTO.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MAUIClient.DTO
{
    public class CreateInvoiceDto
    {
        public bool ExistingCustomer { get; set; }
        public CustomerDto Customer {get;set;} = new CustomerDto();

        public int CustomerId { get; set; }

        public List<InvoiceItemDto> Items { get; set; } = [];
        
    }
}
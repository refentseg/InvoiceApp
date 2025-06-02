using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MAUIClient.DTO.Customer;
using MAUIClient.Models;
using MAUIClient.Models.InvoiceAggregate;

namespace MAUIClient.DTO
{
    public class InvoiceDto
    {
        public string Id {get;set;} ="";

        public int CustomerId { get; set; } 

        public CustomerDto Customer{get;set;} = new CustomerDto();

        public string SalesRep {get;set;} = "";
        public DateTime OrderDate {get;set;} = DateTime.UtcNow;

        public List<InvoiceItemDto> Items {get;set;} = [];
    
        public long Subtotal {get;set;}
        
        public long Vat{get;set;}

        public long Total{get;set;}

        public string InvoiceStatus {get;set;} = "";
    }
}
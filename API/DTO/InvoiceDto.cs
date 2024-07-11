using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using API.Entity;
using API.Entity.InvoiceAggregate;

namespace API.DTO
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
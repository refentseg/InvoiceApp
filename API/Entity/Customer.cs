using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using API.Entity.InvoiceAggregate;

namespace API.Entity
{
    public class Customer
    {
        public int Id{get;set;}
        
        public string FullName { get; set; } ="";

        public List<Invoice> Invoices { get; set; }= [];

        public string Company { get; set; } ="";

        public string Email {get;set;} ="";

        public string Phone {get;set;} ="";
    }
}
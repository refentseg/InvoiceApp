using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using API.Entity.InvoiceAggregate;

namespace API.DTO
{
    public class CustomerDto
    {
        public int Id{get;set;}
        public string FullName { get; set; } = "";

        public string Company { get; set; } = "";
        [Required]
        public string Email {get;set;} = "";

        public string Phone {get;set;} ="";

        List<Invoice> Invoices{get;set;} = [];
    }
}
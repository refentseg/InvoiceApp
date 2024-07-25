using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entity.InvoiceAggregate
{
    [Table("InvoiceItems")]
    public class InvoiceItem
    {
        public int Id{get;set;}

        // public ProductItemOrdered ItemOrdered{get;set;}
        public string Name{get;set;} = "";


        public long Amount{get;set;}

        public int Quantity{get;set;}

        public string InvoiceId { get; set; }
        public Invoice Invoice { get; set; }

    }
}
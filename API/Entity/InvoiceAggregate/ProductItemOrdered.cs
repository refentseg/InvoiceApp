using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace API.Entity.InvoiceAggregate
{
    [Owned]
    public class ProductItemOrdered
    {
        public int ProductId{get;set;}//Pk,FK

        public string Name{get;set;}

        public long Amount{get;set;}

        public Product Product { get; set;}
    }
}
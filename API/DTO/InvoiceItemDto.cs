using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTO
{
    public class InvoiceItemDto
    {
        public int Id{get;set;} 

        public string Name {get;set;} = "";

        public long Amount{get;set;}

        public int Quantity{get;set;}
    }
}
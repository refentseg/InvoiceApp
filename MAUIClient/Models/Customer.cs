using MAUIClient.Models.InvoiceAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIClient.Models
{
    public class Customer
    {
        public int Id { get; set; }

        public string FullName { get; set; } = "";

        public ICollection<Invoice> Invoices { get; set; } = [];

        public string Company { get; set; } = "";

        public string Email { get; set; } = "";

        public string Phone { get; set; } = "";
    }
}

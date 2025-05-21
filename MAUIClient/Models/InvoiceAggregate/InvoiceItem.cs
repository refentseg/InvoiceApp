using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIClient.Models.InvoiceAggregate
{
    public class InvoiceItem
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";

        public long Amount { get; set; }

        public int Quantity { get; set; }

        public string InvoiceId { get; set; } = "";
    }
}

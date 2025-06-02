using MAUIClient.Methods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIClient.Models.InvoiceAggregate
{
    public class Invoice
    {
        public string Id { get; set; } = "";

        public int CustomerId { get; set; }

        public Customer Customer { get; set; } = new Customer();

        public string SalesRep { get; set; } = "";
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public List<InvoiceItem> Items { get; set; } = [];

        public long Subtotal { get; set; }

        public long Vat { get; set; }

        public long Total { get; set; }

        public string TotalFormatted => CurrencyHelper.CurrencyFormat(Total);

        public string InvoiceStatus { get; set; } = "";
    }
}


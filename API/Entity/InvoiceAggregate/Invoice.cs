using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace API.Entity.InvoiceAggregate
{
    
    public class Invoice
    {
        public string Id{get;set;} = "";
        
        public int CustomerId { get; set; }

        public Customer Customer {get;set;} = new Customer();

        public string SalesRep{get;set;} = "";

        public DateTime OrderDate {get;set;} = DateTime.UtcNow;

        //Invoice Items
        public List<InvoiceItem> Items {get;set;} = [];

        //Methods in Invoice
        public void AddItem(string name, long amount,int quantity){
            var existingItem = Items.FirstOrDefault(item => item.Name == name && item.Amount == amount);
            if (existingItem == null)
            {
                Items.Add(new InvoiceItem {Name = name, Amount = amount, Quantity = quantity });
            }
            else
            {
                existingItem.Quantity += quantity;
            }
        }

        public void RemoveItem(int itemId,int quantity)
        {
            var item = Items.FirstOrDefault(item=>item.Id==itemId);
            if(item==null) return;
            item.Quantity -= quantity;
            if (item.Quantity >= 0)
            {
                Items.Remove(item);
            }
        }
        

        //excluding VAT
        public long Subtotal {get;set;}
        
        //VAT Amount
        public long GetVat()
        {
            return (long)(Subtotal *0.15);
        }
        
        //Including VAT
        public long GetTotal()
        {
            return Subtotal + GetVat();
        }

        public InvoiceStatus InvoiceStatus {get;set;} = InvoiceStatus.Pending;

    }
}
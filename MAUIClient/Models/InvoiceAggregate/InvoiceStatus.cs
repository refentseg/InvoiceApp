using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIClient.Models.InvoiceAggregate
{
    public enum InvoiceStatus
    {
        Pending,
        Approved,
        Sent,
        Rejected,
        DepositPaid,
        Completed,
        Paid,
        Overdue,
    }

    public static class EnumHelper
    {
        public static string[] GetEnumValues<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>().Select(v => v.ToString()).ToArray();
        }
    }
}

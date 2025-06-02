using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIClient.Methods
{
    public static class CurrencyHelper
    {
        public static string CurrencyFormat(long cents)
        {
            decimal amount = cents / 100m;
            return amount.ToString("C", new CultureInfo("en-ZA"));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIClient.Models.RequestHelpers
{
    public class PagedResponse<T>
    {
       public MetaData MetaData { get; set; } 
       public List<T> Items { get; set; }
        
    }
}

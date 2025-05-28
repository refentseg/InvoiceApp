using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace MAUIClient.Models.RequestHelpers
{
    public class PagedList<T>
    {
        public List<T> Items { get; set; }
        public MetaData MetaData { get; set; }

    }
}
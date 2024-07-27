using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.RequestHelpers
{
    public class CustomerParams:PaginationParams
    {
        public string OrderBy{get;set;} ="";
        public string SearchTerm{get;set;} ="";

        
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace API.Entity
{
    public class User:IdentityUser<int>
    {
        public string FirstName{get;set;} = "";

        public string LastName{get;set;} ="";
        public string Company {get;set;} ="";
    }
}
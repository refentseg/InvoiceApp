using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTO
{
    public class RegisterDto:LoginDto
    {
        public string Email{get;set;} = "";

        public string FirstName { get; set; } = "";

        public string LastName { get; set; } = "";

        public string Comapny{get;set;} = "";

        public string ConfirmPassword { get; set; } = "";
    }
}
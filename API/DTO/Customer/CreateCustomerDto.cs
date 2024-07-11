using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTO
{
    public class CreateCustomerDto
    {

        [Required]
        public string FullName { get; set; }

        public string Company { get; set; }
        [Required]
        public string Email {get;set;}

        public string Phone {get;set;}
    }
}
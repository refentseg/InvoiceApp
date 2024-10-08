using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entity
{
    public class Address
    {
        [Required]
        public string FullName{get;set;} = "" ;
        [Required]
        public string Address1 {get;set;} = "" ;
        public string Surburb {get;set;} = "" ;
        [Required]
        public string City{get;set;} = "" ;
        [Required]
        public string Province{get;set;} = "" ;
        [Required]
        public string PostalCode {get;set;} = "" ;
        [Required]
        public string Country {get;set;} = "" ;
    }
}
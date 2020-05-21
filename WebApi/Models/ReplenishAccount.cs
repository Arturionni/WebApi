using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.ViewModels
{
    public class ReplenishAccount
    {
        [Required]
        public string AccountNumber { get; set; }
        
        [Required]
        public decimal Value { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.ViewModels
{
    public class ReplenishAccount
    {
        public int Id { get; set; }

        [Required]
        public uint AccountNumber { get; set; }
        
        [Required]
        public float Value { get; set; }

    }
}

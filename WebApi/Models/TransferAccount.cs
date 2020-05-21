using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.ViewModels
{
    public class TransferAccount
    {
        [Required]
        public string AccountNumberReceiver { get; set; }
        [Required]
        public string AccountNumberCurrent { get; set; }
        [Required]
        public decimal Value { get; set; }
    }
}

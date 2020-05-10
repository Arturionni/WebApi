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
        public ulong AccountNumberReceiver { get; set; }
        [Required]
        public ulong AccountNumberCurrent { get; set; }
        [Required]
        public float Value { get; set; }
    }
}

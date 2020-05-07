using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.ViewModels
{
    public class TransferAccount
    {
        public string Id { get; set; }
        [Required]
        public uint AccountNumberReceiver { get; set; }
        [Required]
        public uint AccountNumberCurrent { get; set; }
        [Required]
        public float Value { get; set; }
    }
}

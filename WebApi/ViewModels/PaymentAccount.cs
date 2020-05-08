using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.ViewModels
{
    public class PaymentAccount
    {
        public string Id { get; set; }
        [Required]
        public uint AccountNumberReceiver { get; set; }
        [Required]
        public uint AccountNumberCurrent { get; set; }
        [Required]
        public float Value { get; set; }
        [Required]
        public bool UseTemplate { get; set; }
        public string PaymentName { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverEmail { get; set; }
        public string PaymentPurpose { get; set; }

    }
}

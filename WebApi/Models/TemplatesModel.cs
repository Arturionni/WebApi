using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.ViewModels
{
    public class TemplatesModel
    {
        public Guid Id { get; set; }
        [Required]
        public string AccountNumberCurrent { get; set; }
        [Required]
        public string AccountNumberReceiver { get; set; }
        public string PaymentName { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverEmail { get; set; }
        public string PaymentPurpose { get; set; }
        [Required]
        public decimal PaymentValue { get; set; }
    }
}

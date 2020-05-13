using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.ViewModels
{
    public class TemplatesModel
    {
        public string Id { get; set; }
        [Required]
        public ulong AccountNumberCurrent { get; set; }
        [Required]
        public ulong AccountNumberReceiver { get; set; }
        public string PaymentName { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverEmail { get; set; }
        public string PaymentPurpose { get; set; }
        [Required]
        public float PaymentValue { get; set; }
    }
}

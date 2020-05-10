using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.ViewModels
{
    public class TemplatesModel
    {
        public string Id { get; set; }
        public ulong AccountNumber { get; set; }
        public string PaymentName { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverEmail { get; set; }
        public string PaymentPurpose { get; set; }
        public float PaymentValue { get; set; }
    }
}

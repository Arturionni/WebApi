using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.ViewModels
{
    public class AccountsModel
    {
        public string Id { get; set; }
        public ulong AccountNumber { get; set; }
        public float AccountBalance { get; set; }
        public string UserId { get; set; }
        public bool Status { get; set; }

    }
}

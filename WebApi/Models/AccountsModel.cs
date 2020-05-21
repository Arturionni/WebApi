using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.ViewModels
{
    public class AccountsModel
    {
        public Guid Id { get; set; }
        public string AccountNumber { get; set; }
        public decimal AccountBalance { get; set; }
        public string DateCreated { get; set; }
        public Guid UserId { get; set; }
        public bool Status { get; set; }

    }
}

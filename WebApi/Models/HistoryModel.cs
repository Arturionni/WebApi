using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.ViewModels
{
    public class HistoryModel
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public string Type { get; set; }
        public string Date { get; set; }
        public decimal Value { get; set; }

    }
}

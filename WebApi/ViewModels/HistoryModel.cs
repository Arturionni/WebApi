using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.ViewModels
{
    public class HistoryModel
    {
        public string Id { get; set; }
        public string AccountId { get; set; }
        public string Type { get; set; }
        public string Date { get; set; }
        public float Value { get; set; }

    }
}

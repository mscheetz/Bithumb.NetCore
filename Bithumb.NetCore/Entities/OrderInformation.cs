using System;
using System.Collections.Generic;
using System.Text;

namespace Bithumb.NetCore.Entities
{
    public class OrderInformation
    {
        public string order_id { get; set; }
        public string symbol { get; set; }
        public long order_date { get; set; }
        public string pair { get; set; }
        public TransactionType type { get; set; }
        public string status { get; set; }
        public decimal units { get; set; }
        public decimal? units_remaining { get; set; }
        public decimal price { get; set; }
        public decimal? fee { get; set; }
        public decimal? total { get; set; }
        public long? date_completed { get; set; }
    }
}

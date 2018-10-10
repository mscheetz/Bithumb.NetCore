using System;
using System.Collections.Generic;
using System.Text;

namespace Bithumb.NetCore.Entities
{
    public class Transaction
    {
        public long cont_no { get; set; }
        public DateTime transaction_date { get; set; }
        public TransactionType type { get; set; }
        public decimal units_traded { get; set; }
        public decimal price { get; set; }
        public decimal total { get; set; }
    }
}

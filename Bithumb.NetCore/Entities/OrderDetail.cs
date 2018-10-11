using System;
using System.Collections.Generic;
using System.Text;

namespace Bithumb.NetCore.Entities
{
    public class OrderDetail
    {
        public long transaction_date { get; set; }
        public TransactionType type { get; set; }
        public string order_currency { get; set; }
        public string payment_currency { get; set; }
        public decimal units_traded { get; set; }
        public decimal price { get; set; }
        public decimal fee { get; set; }
        public decimal total { get; set; }
    }
}

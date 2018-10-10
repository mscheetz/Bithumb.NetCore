using System;
using System.Collections.Generic;
using System.Text;

namespace Bithumb.NetCore.Entities
{
    public class OrderBook
    {
        public long timestamp { get; set; }
        public string order_currency { get; set; }
        public string payment_currency { get; set; }
        public OpenOrder[] bids { get; set; }
        public OpenOrder[] asks { get; set; }
    }
}

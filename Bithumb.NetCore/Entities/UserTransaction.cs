using System;
using System.Collections.Generic;
using System.Text;

namespace Bithumb.NetCore.Entities
{
    public class UserTransaction
    {
        public string symbol { get; set; }
        public Search search { get; set; }
        public long transferDate { get; set; }
        public decimal units { get; set; }
        public decimal price { get; set; }
        public decimal pricePer { get; set; }
        public string fee { get; set; }
        public decimal qtyRemaining { get; set; }
        public decimal krwRemaining { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Bithumb.NetCore.Entities
{
    public class Balance
    {
        public string symbol { get; set; }
        public decimal total { get; set; }
        public decimal inUse { get; set; }
        public decimal available { get; set; }
        public decimal lastTrx { get; set; }
    }
}

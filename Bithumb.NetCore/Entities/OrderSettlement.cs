using System;
using System.Collections.Generic;
using System.Text;

namespace Bithumb.NetCore.Entities
{
    public class OrderSettlement
    {
        public string cont_id { get; set; }
        public decimal units { get; set; }
        public decimal price { get; set; }
        public decimal total { get; set; }
        public decimal fee { get; set; }
    }
}

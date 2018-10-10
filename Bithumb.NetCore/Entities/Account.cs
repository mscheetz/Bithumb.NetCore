using System;
using System.Collections.Generic;
using System.Text;

namespace Bithumb.NetCore.Entities
{
    public class Account
    {
        public long created { get; set; }
        public string account_id { get; set; }
        public decimal trade_fee { get; set; }
        public decimal balance { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Bithumb.NetCore.Entities
{
    public enum TransactionType
    {
        ask,
        bid
    }

    public enum Search
    {
        entire = 0,
        purchase_completed = 1,
        sales_completed = 2,
        withdrawing = 3,
        deposit = 4,
        withdrawal = 5,
        krw_depositing = 9
    }
}

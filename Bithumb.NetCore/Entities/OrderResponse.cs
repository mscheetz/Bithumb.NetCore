using System;
using System.Collections.Generic;
using System.Text;

namespace Bithumb.NetCore.Entities
{
    public class OrderResponse<T> : ApiResponse<T>
    {
        public string order_id { get; set; }
    }
}

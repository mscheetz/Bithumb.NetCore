using System;
using System.Collections.Generic;
using System.Text;

namespace Bithumb.NetCore.Entities
{
    public class ApiResponse<T>
    {
        public string status { get; set; }
        public T data { get; set; }
        public string message { get; set; }
    }
}

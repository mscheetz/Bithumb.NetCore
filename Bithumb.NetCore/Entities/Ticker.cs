using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bithumb.NetCore.Entities
{
    public class Ticker : PrivateTicker
    {
        [JsonProperty(PropertyName = "buy_price")]
        public decimal buy_price { get; set; }
        [JsonProperty(PropertyName = "sell_price")]
        public decimal sell_price { get; set; }
        [JsonProperty(PropertyName = "24H_fluctuate")]
        public decimal fluctuate { get; set; }
        [JsonProperty(PropertyName = "24H_fluctuate_rate")]
        public decimal fluctuateRate { get; set; }
    }
}

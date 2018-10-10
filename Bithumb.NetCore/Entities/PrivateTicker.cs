using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bithumb.NetCore.Entities
{
    public class PrivateTicker
    {
        [JsonProperty(PropertyName = "opening_price")]
        public decimal opening_price { get; set; }
        [JsonProperty(PropertyName = "closing_price")]
        public decimal closing_price { get; set; }
        [JsonProperty(PropertyName = "min_price")]
        public decimal min_price { get; set; }
        [JsonProperty(PropertyName = "max_price")]
        public decimal max_price { get; set; }
        [JsonProperty(PropertyName = "average_price")]
        public decimal average_price { get; set; }
        [JsonProperty(PropertyName = "units_traded")]
        public decimal units_traded { get; set; }
        [JsonProperty(PropertyName = "volume_1day")]
        public decimal volume_1day { get; set; }
        [JsonProperty(PropertyName = "volume_7day")]
        public decimal volume_7day { get; set; }
        public long date { get; set; }
    }
}

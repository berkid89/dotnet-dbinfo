using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace dotnet_dbinfo.InfoCollectors.DynamoDb.Models
{
    public class DbInfo
    {
        [JsonProperty(Order = 0)]
        public string ServiceName { get; set; }

        [JsonProperty(Order = 1)]
        public string Region { get; set; }

        [JsonProperty(Order = 2)]
        public string ServiceVersion { get; set; }
    }
}

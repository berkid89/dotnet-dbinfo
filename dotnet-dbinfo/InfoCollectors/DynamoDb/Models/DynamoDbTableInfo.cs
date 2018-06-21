using dotnet_dbinfo.InfoCollectors.Models;
using Newtonsoft.Json;
using System;

namespace dotnet_dbinfo.InfoCollectors.DynamoDb.Models
{
    public class DynamoDbTableInfo : TableInfo
    {
        [JsonProperty(Order = 2)]
        public double TableSizeInMb { get; set; }

        [JsonProperty(Order = 3)]
        public DateTime CreateDate { get; set; }

        [JsonProperty(Order = 4)]
        public string Status { get; set; }
    }
}

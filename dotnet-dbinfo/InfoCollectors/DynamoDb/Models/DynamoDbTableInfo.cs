using dotnet_dbinfo.InfoCollectors.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

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

        [JsonProperty(Order = 5)]
        public IEnumerable<object> GlobalSecondaryIndexes { get; set; }
    }
}

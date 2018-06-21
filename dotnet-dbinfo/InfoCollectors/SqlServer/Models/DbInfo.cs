using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace dotnet_dbinfo.InfoCollectors.SqlServer.Models
{
    public class DbInfo
    {
        [Key]
        [JsonProperty(Order = 0)]
        public int DatabaseId { get; set; }

        [JsonProperty(Order = 1)]
        public string Name { get; set; }

        [JsonProperty(Order = 2)]
        public DateTime CreateDate { get; set; }

        [JsonProperty(Order = 3)]
        public string Collation { get; set; }

        [JsonProperty(Order = 4)]
        public string State { get; set; }
    }
}

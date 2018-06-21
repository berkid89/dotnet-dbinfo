using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace dotnet_dbinfo.InfoCollectors.Models
{
    public class TableInfo
    {
        [Key]
        [JsonProperty(Order = 0)]
        public string TableName { get; set; }

        [JsonProperty(Order = 1)]
        public long ItemCount { get; set; }
    }
}

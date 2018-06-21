using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace dotnet_dbinfo.InfoCollectors.SqlServer.Models
{
    public class IndexInfo
    {
        [Key]
        [JsonProperty(Order = 0)]
        public string Index { get; set; }

        [JsonProperty(Order = 1)]
        public string TableName { get; set; }

        [JsonProperty(Order = 2)]
        public double AvgFragmentationInPercent { get; set; }
    }
}

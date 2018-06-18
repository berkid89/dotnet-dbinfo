using System.ComponentModel.DataAnnotations;

namespace dotnet_dbinfo.Models
{
    public class IndexInfo
    {
        [Key]
        public string Index { get; set; }

        public string TableName { get; set; }

        public double AvgFragmentationInPercent { get; set; }
    }
}

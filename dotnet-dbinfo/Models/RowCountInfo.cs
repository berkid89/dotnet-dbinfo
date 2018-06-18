using System.ComponentModel.DataAnnotations;

namespace dotnet_dbinfo.Models
{
    public class RowCountInfo
    {
        [Key]
        public string TableName { get; set; }

        public int RowCount { get; set; }
    }
}

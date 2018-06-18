using System;
using System.ComponentModel.DataAnnotations;

namespace dotnet_dbinfo.Models
{
    public class DbInfo
    {
        [Key]
        public int DatabaseId { get; set; }

        public string Name { get; set; }

        public DateTime CreateDate { get; set; }

        public string Collation { get; set; }

        public string State { get; set; }
    }
}

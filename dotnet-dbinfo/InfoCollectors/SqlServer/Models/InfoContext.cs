using dotnet_dbinfo.Arguments;
using dotnet_dbinfo.InfoCollectors.Models;
using Microsoft.EntityFrameworkCore;

namespace dotnet_dbinfo.InfoCollectors.SqlServer.Models
{
    public class InfoContext : DbContext
    {
        private readonly SqlServerArguments options;

        public DbSet<DbInfo> DbInfo { get; set; }

        public DbSet<TableInfo> TableInfo { get; set; }

        public DbSet<IndexInfo> IndexInfo { get; set; }

        public InfoContext(SqlServerArguments options)
        {
            this.options = options;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer($"data source={options.Server};initial catalog={options.Database};User Id={options.User};Password ={options.Password};");
        }
    }
}

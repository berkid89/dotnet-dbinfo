using dotnet_dbinfo.Enums;
using dotnet_dbinfo.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace dotnet_dbinfo
{
    public class InfoContext : DbContext
    {
        private readonly Options options;

        public DbSet<DbInfo> DbInfo { get; set; }

        public DbSet<RowCountInfo> RowCountInfo { get; set; }

        public DbSet<IndexInfo> IndexInfo { get; set; }

        public InfoContext(Options options)
        {
            this.options = options;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            switch (options.SupportedDatabaseType)
            {
                case SupportedDatabaseType.SQLSERVER:
                    optionsBuilder.UseSqlServer($"data source={options.Server};initial catalog={options.Database};User Id={options.User};Password ={options.Password};");
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}

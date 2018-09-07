using Amazon.DynamoDBv2;
using System.Collections.Generic;
using System.Linq;
using static dotnet_dbinfo.Helpers;

namespace dotnet_dbinfo.InfoCollectors
{
    public static class DynamoDbInfoCollector
    {
        public static object CollectDynamoDbInfo(AmazonDynamoDBClient conn) =>
            new
            {
                general = GetGeneralInfo(conn),
                tables = GetTableInfo(conn),
            };

        private static object GetGeneralInfo(AmazonDynamoDBClient conn) =>
            new
            {
                ServiceName = conn.Config.RegionEndpointServiceName,
                Region = $"{conn.Config.RegionEndpoint.DisplayName} ({conn.Config.RegionEndpoint.SystemName})",
                conn.Config.ServiceVersion
            };

        private static IEnumerable<object> GetTableInfo(AmazonDynamoDBClient conn) =>
            conn.ListTablesAsync().Result.TableNames.Select(p => conn.DescribeTableAsync(p).Result.Table).ToList().Select(table =>
            {
                return new
                {
                    table.TableName,
                    table.ItemCount,
                    TableSizeInMb = ConvertBytesToMegabytes(table.TableSizeBytes),
                    CreateDate = table.CreationDateTime,
                    Status = table.TableStatus.Value,
                    GlobalSecondaryIndexes = table.GlobalSecondaryIndexes.Select(p => new
                    {
                        p.IndexName,
                        IndexItemCount = p.ItemCount,
                        IndexSizeInMb = ConvertBytesToMegabytes(p.IndexSizeBytes),
                        IndexStatus = p.IndexStatus.Value
                    })
                };
            }).ToList();
    }
}

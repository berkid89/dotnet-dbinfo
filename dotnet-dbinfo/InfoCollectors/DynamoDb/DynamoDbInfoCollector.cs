using Amazon.DynamoDBv2;
using dotnet_dbinfo.Arguments;
using dotnet_dbinfo.InfoCollectors.DynamoDb.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace dotnet_dbinfo.InfoCollectors.DynamoDb
{
    public class DynamoDbInfoCollector : InfoCollector
    {
        private readonly AmazonDynamoDBClient db;
        private readonly DynamoDbArguments args;

        public DynamoDbInfoCollector(DynamoDbArguments args)
        {
            this.args = args;

            var clientConfig = new AmazonDynamoDBConfig()
            {
                RegionEndpoint = args.RegionEndpoint
            };

            db = args.Credentials == null ? new AmazonDynamoDBClient(clientConfig) : new AmazonDynamoDBClient(args.Credentials, clientConfig);
        }

        public override IArguments GetArgs()
        {
            return args;
        }

        public override string Collect()
        {
            return serialize(new
            {
                general = getGeneralInfo(),
                tables = getTableInfo(),
            });
        }

        public override void Dispose()
        {
            db.Dispose();
        }

        private DbInfo getGeneralInfo()
        {
            return new DbInfo()
            {
                ServiceName = db.Config.RegionEndpointServiceName,
                Region = $"{args.RegionEndpoint.DisplayName} ({args.RegionEndpoint.SystemName})",
                ServiceVersion = db.Config.ServiceVersion
            };
        }

        private IEnumerable<DynamoDbTableInfo> getTableInfo()
        {
            var result = new List<DynamoDbTableInfo>();

            foreach (var tableName in db.ListTablesAsync().Result.TableNames)
            {
                var table = db.DescribeTableAsync(tableName).Result.Table;

                result.Add(new DynamoDbTableInfo()
                {
                    TableName = table.TableName,
                    ItemCount = table.ItemCount,
                    TableSizeInMb = convertBytesToMegabytes(table.TableSizeBytes),
                    CreateDate = table.CreationDateTime,
                    Status = table.TableStatus.Value
                });
            }

            return result;
        }

        private double convertBytesToMegabytes(long bytes)
        {
            return (bytes / 1024f) / 1024f;
        }
    }
}

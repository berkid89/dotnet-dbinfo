using Amazon.DynamoDBv2;
using dotnet_dbinfo.Arguments;
using System.Collections.Generic;
using System.Linq;

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

        private object getGeneralInfo()
        {
            return new
            {
                ServiceName = db.Config.RegionEndpointServiceName,
                Region = $"{args.RegionEndpoint.DisplayName} ({args.RegionEndpoint.SystemName})",
                db.Config.ServiceVersion
            };
        }

        private IEnumerable<object> getTableInfo()
        {
            var result = new List<object>();

            foreach (var tableName in db.ListTablesAsync().Result.TableNames)
            {
                var table = db.DescribeTableAsync(tableName).Result.Table;

                result.Add(new
                {
                    table.TableName,
                    table.ItemCount,
                    TableSizeInMb = convertBytesToMegabytes(table.TableSizeBytes),
                    CreateDate = table.CreationDateTime,
                    Status = table.TableStatus.Value,
                    GlobalSecondaryIndexes = table.GlobalSecondaryIndexes.Select(p => new
                    {
                        p.IndexName,
                        IndexItemCount = p.ItemCount,
                        IndexSizeInMb = convertBytesToMegabytes(p.IndexSizeBytes),
                        IndexStatus = p.IndexStatus.Value
                    })
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

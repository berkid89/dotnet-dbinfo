using dotnet_dbinfo.Arguments;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace dotnet_dbinfo.InfoCollectors.SqlServer
{
    public class SqlServerInfoCollector : InfoCollector
    {
        private readonly SqlConnection conn;
        private readonly SqlServerArguments args;

        public SqlServerInfoCollector(SqlServerArguments args)
        {
            this.args = args;
            conn = new SqlConnection($"data source={args.Server};initial catalog={args.Database};User Id={args.User};Password ={args.Password};");
            conn.Open();
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
                fragmentedIndexes = getFragmentedIndexes()
            });
        }

        public override void Dispose()
        {
            conn.Dispose();
        }

        private object getGeneralInfo()
        {
            var command = new SqlCommand($@"
                                SELECT [database_id] [DatabaseId], [name] [Name], [create_date] [CreateDate], [collation_name] [Collation], [state_desc] [State]
                                FROM sys.databases
                                WHERE [name] = '{args.Database}'", conn);

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    return new
                    {
                        DatabaseId = reader[0],
                        Name = reader[1],
                        CreateDate = reader[2],
                        Collation = reader[3],
                        State = reader[4]
                    };
                }
            }

            return new { };
        }

        private IEnumerable<object> getFragmentedIndexes()
        {
            var result = new List<object>();

            var command = new SqlCommand(@"
                                SELECT
                                dbindexes.[object_id] [ObjectId],
                                dbindexes.[name] [Index],
								dbindexes.[type_desc] [Type],
                                '[' + dbschemas.[name] + '].[' + dbtables.[name] + ']' [TableName],
                                indexstats.[avg_fragmentation_in_percent] as [AvgFragmentationInPercent]
                                    FROM sys.dm_db_index_physical_stats (DB_ID(), NULL, NULL, NULL, NULL) AS indexstats
                                    INNER JOIN sys.tables dbtables on dbtables.[object_id] = indexstats.[object_id]
                                    INNER JOIN sys.schemas dbschemas on dbtables.[schema_id] = dbschemas.[schema_id]
                                    INNER JOIN sys.indexes AS dbindexes ON dbindexes.[object_id] = indexstats.[object_id] AND indexstats.index_id = dbindexes.index_id
                                WHERE indexstats.database_id = DB_ID() and indexstats.avg_fragmentation_in_percent > 5
                                ORDER BY indexstats.avg_fragmentation_in_percent desc", conn);

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add(new
                    {
                        ObjectId = reader[0],
                        Index = reader[1],
                        Type = reader[2],
                        TableName = reader[3],
                        AvgFragmentationInPercent = reader[4]
                    });
                }
            }

            return result;
        }

        private IEnumerable<object> getTableInfo()
        {
            var result = new List<object>();

            var command = new SqlCommand(@"
                                DECLARE @TableRowCounts TABLE ([TableName] VARCHAR(128), [ItemCount] BIGINT) ;
                                INSERT INTO @TableRowCounts([TableName], [ItemCount])
                                EXEC sp_MSforeachtable 'SELECT ''?'' [TableName], COUNT(*) [ItemCount] FROM ?';
                                SELECT [TableName], [ItemCount]
                                FROM @TableRowCounts
                                ORDER BY[TableName]", conn);

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add(new
                    {
                        TableName = reader[0],
                        ItemCount = reader[1],
                    });
                }
            }

            return result;
        }
    }
}

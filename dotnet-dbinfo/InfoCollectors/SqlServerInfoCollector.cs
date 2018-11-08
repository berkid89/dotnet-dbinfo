using System.Collections.Generic;
using System.Data.SqlClient;

namespace dotnet_dbinfo.InfoCollectors.SqlServer
{
    public static class SqlServerInfoCollector
    {
        public static object CollectSqlServerDbInfo(SqlConnection conn, bool isAzureSQL) =>
            new
            {
                general = GetGeneralInfo(conn),
                tables = GetTableInfo(conn, isAzureSQL),
                fragmentedIndexes = GetFragmentedIndexes(conn)
            };


        private static object GetGeneralInfo(SqlConnection conn)
        {
            var command = new SqlCommand($@"
                                SELECT [database_id] [DatabaseId], [name] [Name], [create_date] [CreateDate], [collation_name] [Collation], [state_desc] [State]
                                FROM sys.databases
                                WHERE [name] = '{conn.Database}'", conn);

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

        private static IEnumerable<object> GetFragmentedIndexes(SqlConnection conn)
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

        private static IEnumerable<object> GetTableInfo(SqlConnection conn, bool isAzureSQL)
        {
            var result = new List<object>();
            var sql = "";
            if (isAzureSQL)
            {
                sql = @"DECLARE @TableRowCounts TABLE ([TableName] VARCHAR(128), [ItemCount] BIGINT) ;
                        INSERT INTO @TableRowCounts([TableName], [ItemCount])
                        SELECT t.name ,s.row_count 
                        FROM sys.tables t
                        JOIN sys.dm_db_partition_stats s ON t.object_id = s.object_id
                        SELECT [TableName], [ItemCount]
                        FROM @TableRowCounts
                        ORDER BY[TableName]";
            }
            else
            {
                sql = @"DECLARE @TableRowCounts TABLE ([TableName] VARCHAR(128), [ItemCount] BIGINT) ;
                        INSERT INTO @TableRowCounts([TableName], [ItemCount])
                        EXEC sp_MSforeachtable 'SELECT ''?'' [TableName], COUNT(*) [ItemCount] FROM ?';
                        SELECT [TableName], [ItemCount]
                        FROM @TableRowCounts
                        ORDER BY[TableName]";
            }

            var command = new SqlCommand(sql, conn);
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

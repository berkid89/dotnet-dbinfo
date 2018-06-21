using dotnet_dbinfo.Arguments;
using dotnet_dbinfo.InfoCollectors.Models;
using dotnet_dbinfo.InfoCollectors.SqlServer.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace dotnet_dbinfo.InfoCollectors.SqlServer
{
    public class SqlServerInfoCollector : InfoCollector
    {
        private readonly InfoContext db;
        private readonly SqlServerArguments args;

        public SqlServerInfoCollector(SqlServerArguments args)
        {
            this.args = args;
            db = new InfoContext(args);
        }

        public override IArguments GetArgs()
        {
            return args;
        }

        public override string Collect()
        {
            return serialize(new
            {
                general = getGeneralInfo(db.DbInfo),
                tables = getTableInfo(db.TableInfo),
                fragmentedIndexes = getFragmentedIndexes(db.IndexInfo)
            });
        }

        public override void Dispose()
        {
            db.Dispose();
        }

        private DbInfo getGeneralInfo(DbSet<DbInfo> set)
        {
            return set.FromSql(@"
                                SELECT [name] [Name], [database_id] [DatabaseId], [create_date] [CreateDate], [collation_name] [Collation], [state_desc] [State]
                                FROM sys.databases
                                WHERE [name] = {0}", args.Database).First();
        }

        private IEnumerable<IndexInfo> getFragmentedIndexes(DbSet<IndexInfo> set)
        {
            return set.FromSql(@"
                                SELECT
                                dbindexes.[name] [Index],
                                '[' + dbschemas.[name] + '].[' + dbtables.[name] + ']' [TableName],
                                indexstats.[avg_fragmentation_in_percent] as [AvgFragmentationInPercent]
                                    FROM sys.dm_db_index_physical_stats (DB_ID(), NULL, NULL, NULL, NULL) AS indexstats
                                    INNER JOIN sys.tables dbtables on dbtables.[object_id] = indexstats.[object_id]
                                    INNER JOIN sys.schemas dbschemas on dbtables.[schema_id] = dbschemas.[schema_id]
                                    INNER JOIN sys.indexes AS dbindexes ON dbindexes.[object_id] = indexstats.[object_id] AND indexstats.index_id = dbindexes.index_id
                                WHERE indexstats.database_id = DB_ID() and indexstats.avg_fragmentation_in_percent > 5
                                ORDER BY indexstats.avg_fragmentation_in_percent desc").ToList();
        }

        private IEnumerable<TableInfo> getTableInfo(DbSet<TableInfo> set)
        {
            return set.FromSql(@"
                                DECLARE @TableRowCounts TABLE ([TableName] VARCHAR(128), [RowCount] INT) ;
                                INSERT INTO @TableRowCounts([TableName], [RowCount])
                                EXEC sp_MSforeachtable 'SELECT ''?'' [TableName], COUNT(*) [RowCount] FROM ?';
                                SELECT[TableName], [RowCount]
                                FROM @TableRowCounts
                                ORDER BY[TableName]").ToList();
        }
    }
}

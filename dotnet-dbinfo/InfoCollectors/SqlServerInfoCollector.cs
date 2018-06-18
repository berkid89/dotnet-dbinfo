using dotnet_dbinfo.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace dotnet_dbinfo.InfoCollectors
{
    public class SqlServerInfoCollector : IInfoCollector
    {
        private readonly InfoContext db;
        private readonly Options options;

        public SqlServerInfoCollector(InfoContext db, Options options)
        {
            this.db = db;
            this.options = options;
        }

        public DbInfo GetGeneralInfo()
        {
            return db.DbInfo.FromSql(@"
                                SELECT [name] [Name], [database_id] [DatabaseId], [create_date] [CreateDate], [collation_name] [Collation], [state_desc] [State]
                                FROM sys.databases
                                WHERE [name] = {0}", options.Database).First();
        }

        public IEnumerable<IndexInfo> GetFragmentedIndexes()
        {
            return db.IndexInfo.FromSql(@"
                                SELECT
                                dbindexes.[name] [Index],
                                '[' + dbschemas.[name] + '].[' + dbtables.[name] + ']' [TableName],
                                indexstats.[avg_fragmentation_in_percent] as [AvgFragmentationInPercent]
                                    FROM sys.dm_db_index_physical_stats (DB_ID(), NULL, NULL, NULL, NULL) AS indexstats
                                    INNER JOIN sys.tables dbtables on dbtables.[object_id] = indexstats.[object_id]
                                    INNER JOIN sys.schemas dbschemas on dbtables.[schema_id] = dbschemas.[schema_id]
                                    INNER JOIN sys.indexes AS dbindexes ON dbindexes.[object_id] = indexstats.[object_id] AND indexstats.index_id = dbindexes.index_id
                                WHERE indexstats.database_id = DB_ID() and indexstats.avg_fragmentation_in_percent > 5
                                ORDER BY indexstats.avg_fragmentation_in_percent desc");
        }

        public IEnumerable<RowCountInfo> GetRowcounts()
        {
            return db.RowCountInfo.FromSql(@"
                                DECLARE @TableRowCounts TABLE ([TableName] VARCHAR(128), [RowCount] INT) ;
                                INSERT INTO @TableRowCounts([TableName], [RowCount])
                                EXEC sp_MSforeachtable 'SELECT ''?'' [TableName], COUNT(*) [RowCount] FROM ?';
                                SELECT[TableName], [RowCount]
                                FROM @TableRowCounts
                                ORDER BY[TableName]").ToList();
        }
    }
}

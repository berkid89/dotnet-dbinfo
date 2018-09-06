using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using static dotnet_dbinfo.Helpers;

namespace dotnet_dbinfo.InfoCollectors
{
    public static class MongoDbInfoCollector
    {
        public static object CollectMongoDbInfo(MongoClient conn, string database)
        {
            var db = conn.GetDatabase(database);

            return new
            {
                general = GetGeneralInfo(db),
                collections = GetCollectionInfo(db),
            };
        }

        private static object GetGeneralInfo(IMongoDatabase db) =>
            new
            {
                DatabaseId = db.DatabaseNamespace.DatabaseName,
            };

        private static IEnumerable<object> GetCollectionInfo(IMongoDatabase db) =>
            db.ListCollectionNames().ToList().Select(p => MapStats(db.RunCommand<dynamic>($"{{collstats: '{p}'}}")));

        private static object MapStats(dynamic stats) =>
        new
        {
            stats.ns,
            SizeInMb = ConvertBytesToMegabytes(stats.size),
            stats.count,
            StorageSizeInMb = ConvertBytesToMegabytes(stats.storageSize),
            stats.capped,
            stats.nindexes,
            TotalIndexSizeInMb = ConvertBytesToMegabytes(stats.totalIndexSize),
            stats.ok
        };
    }
}

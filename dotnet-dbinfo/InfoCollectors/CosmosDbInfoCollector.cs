using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace dotnet_dbinfo.InfoCollectors
{
    public static class CosmosDbInfoCollector
    {
        public static object CollectCosmosDbInfo(DocumentClient conn, string database) =>
            new
            {
                general = GetGeneralInfo(conn, database),
                collections = GetCollectionInfo(conn, UriFactory.CreateDatabaseUri(database)),
            };

        private static object GetGeneralInfo(DocumentClient conn, string database)
        {
            Database result = conn.CreateDatabaseQuery($"SELECT * FROM dbs d WHERE d.id = '{database}'").ToList().First();

            return new
            {
                result.Id,
                result.AltLink,
            };
        }

        private static IEnumerable<object> GetCollectionInfo(DocumentClient conn, Uri databaseUri) =>
            conn.CreateDocumentCollectionQuery(databaseUri).ToList().Select(p => new
            {
                p.Id,
                p.AltLink,
                DocumentCount = conn.CreateDocumentQuery<int>(p.SelfLink, "SELECT VALUE COUNT(1) FROM c").ToList().First(),
                StoredProcedures = conn.CreateStoredProcedureQuery(p.SelfLink).ToList().Select(sp => sp.Id),
                UserDefinedFunctions = conn.CreateUserDefinedFunctionQuery(p.SelfLink).ToList().Select(uf => uf.Id),
                Triggers = conn.CreateTriggerQuery(p.SelfLink).ToList().Select(t => t.Id)
            }).ToList();
    }
}

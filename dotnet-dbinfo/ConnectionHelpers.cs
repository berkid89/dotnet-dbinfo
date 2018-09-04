using Amazon;
using Amazon.DynamoDBv2;
using Microsoft.Azure.Documents.Client;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace dotnet_dbinfo
{
    public static class ConnectionHelpers
    {
        public static R ConnectToSqlServer<R>(string connStr, Func<SqlConnection, R> f) => Using(new SqlConnection(connStr), conn => { conn.Open(); return f(conn); });

        public static R ConnectToDynamoDb<R>(string awsAccessKeyId, string awsSecretAccessKey, string regionEndpoint, Func<AmazonDynamoDBClient, R> f) => Using(
            GetDynamoDbClient(awsAccessKeyId, awsSecretAccessKey, regionEndpoint)
        , conn => { return f(conn); });

        public static R ConnectToCosmosDb<R>(string endpointUri, string primaryKey, string database, Func<DocumentClient, string, R> f) => Using(new DocumentClient(new Uri(endpointUri), primaryKey), conn => { return f(conn, database); });

        static AmazonDynamoDBClient GetDynamoDbClient(string awsAccessKeyId, string awsSecretAccessKey, string regionEndpoint)
        {
            var region = RegionEndpoint.EnumerableAllRegions.First(p => p.SystemName == regionEndpoint);

            if (string.IsNullOrEmpty(awsAccessKeyId) || string.IsNullOrEmpty(awsSecretAccessKey))
                return new AmazonDynamoDBClient(region);
            else
                return new AmazonDynamoDBClient(awsAccessKeyId, awsSecretAccessKey, region);
        }

        static R Using<TDisp, R>(TDisp client, Func<TDisp, R> func) where TDisp : IDisposable
        {
            using (var disp = client) return func(disp);
        }
    }
}

using dotnet_dbinfo.InfoCollectors;
using dotnet_dbinfo.InfoCollectors.SqlServer;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq;
using static dotnet_dbinfo.Helpers;

namespace dotnet_dbinfo
{
    static class Program
    {
        private const int ERROR = 2;
        private const int OK = 0;

        static int Main(string[] args)
        {
            try
            {
                switch (GetDbType(GetArg(args, 0)))
                {
                    case SupportedDatabaseType.SQLSERVER:
                        Console.Write(
                            Serialize(
                                ConnectToSqlServer($"data source={GetArg(args, 1)};initial catalog={GetArg(args, 2)};User Id={GetArg(args, 3)};Password ={GetArg(args, 4)};", SqlServerInfoCollector.CollectSqlServerDbInfo)
                                ));
                        break;
                    case SupportedDatabaseType.DYNAMODB:
                        Console.Write(
                            Serialize(
                                ConnectToDynamoDb(GetArg(args, 2), GetArg(args, 3), GetArg(args, 1), DynamoDbInfoCollector.CollectDynamoDbInfo)
                                ));
                        break;
                    case SupportedDatabaseType.COSMOSDB:
                        Console.Write(
                            Serialize(
                                ConnectToCosmosDb(GetArg(args, 1), GetArg(args, 2), GetArg(args, 3), CosmosDbInfoCollector.CollectCosmosDbInfo)
                                ));
                        break;
                    case SupportedDatabaseType.MONGODB:
                        Console.WriteLine(
                            Serialize(
                                ConnectToMongoDb($"mongodb://{GetArg(args, 3)}:{GetArg(args, 4)}@{GetArg(args, 1)}/{GetArg(args, 2)}", GetArg(args, 2), MongoDbInfoCollector.CollectMongoDbInfo)
                                ));
                        break;
                }
#if DEBUG
                Console.ReadKey();
#endif

                return OK;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine("Unexpected error: " + ex.ToString());
                Console.ResetColor();

#if DEBUG
                Console.ReadKey();
#endif

                return ERROR;
            }
        }

        static SupportedDatabaseType GetDbType(string type) =>
            Enum.Parse<SupportedDatabaseType>(type, true);

        static string Serialize(object obj) =>
            JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            });

        static string GetArg(string[] args, int index)
        {
            if (args.Count() > index)
                return args[index];
            else
                return null;
        }
    }
}

﻿using dotnet_dbinfo.Arguments;
using dotnet_dbinfo.Enums;
using dotnet_dbinfo.InfoCollectors;
using dotnet_dbinfo.InfoCollectors.DynamoDb;
using dotnet_dbinfo.InfoCollectors.SqlServer;
using System;
using System.IO;

namespace dotnet_dbinfo
{
    class Program
    {
        static void Main(string[] args)
        {
            IInfoCollector collector = null;

            switch (Enum.Parse<SupportedDatabaseType>(args[0], true))
            {
                case SupportedDatabaseType.SQLSERVER:
                    collector = new SqlServerInfoCollector(new SqlServerArguments(args));
                    break;
                case SupportedDatabaseType.DYNAMODB:
                    collector = new DynamoDbInfoCollector(new DynamoDbArguments(args));
                    break;
            }

            var result = collector.Collect();

            collector.Dispose();

            var resultPath = collector.GetArgs().ResultPath;

            if (!string.IsNullOrEmpty(resultPath))
                File.WriteAllText(resultPath, result);
            else
                Console.Write(result);

#if DEBUG
            Console.ReadKey();
#endif
        }
    }
}

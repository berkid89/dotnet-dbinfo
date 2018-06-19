using dotnet_dbinfo.Enums;
using dotnet_dbinfo.InfoCollectors;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;

namespace dotnet_dbinfo
{
    class Program
    {
        static ServiceProvider serviceProvider;

        static void Main(string[] args)
        {
            var options = new Options(args);

            var services = new ServiceCollection()
                .AddSingleton(p => options)
                .AddDbContext<InfoContext>();

            switch (options.SupportedDatabaseType)
            {
                case SupportedDatabaseType.SQLSERVER:
                    services.AddSingleton<IInfoCollector, SqlServerInfoCollector>();
                    break;
            }

            serviceProvider = services.BuildServiceProvider();

            var infoCollector = serviceProvider.GetService<IInfoCollector>();

            var general = infoCollector.GetGeneralInfo();

            var rowCounts = infoCollector.GetRowcounts();

            var fragmentedIndexes = infoCollector.GetFragmentedIndexes();

            var result = JsonConvert.SerializeObject(new
            {
                general,
                rowCounts,
                fragmentedIndexes
            }, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver(), Formatting = Formatting.Indented });

            if (!string.IsNullOrEmpty(options.ResultPath))
                File.WriteAllText(options.ResultPath, result);
            else
                Console.Write(result);
        }
    }
}

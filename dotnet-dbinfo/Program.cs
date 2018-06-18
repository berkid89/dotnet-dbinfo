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
            var options = new Options()
            {
                Server = args[0],
                Database = args[1],
                User = args[2],
                Password = args[3],
                ResultPath = args[4]
            };

            serviceProvider = new ServiceCollection()
                .AddSingleton(p => options)
                .AddDbContext<InfoContext>()
                .AddSingleton<IInfoCollector, SqlServerInfoCollector>()
                .BuildServiceProvider();

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

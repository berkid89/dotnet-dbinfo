using dotnet_dbinfo.Arguments;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace dotnet_dbinfo.InfoCollectors
{
    public interface IInfoCollector : IDisposable
    {
        IArguments GetArgs();

        string Collect();
    }

    public abstract class InfoCollector : IInfoCollector
    {
        public abstract string Collect();

        public abstract void Dispose();

        public abstract IArguments GetArgs();

        protected string serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver(), Formatting = Formatting.Indented });
        }
    }
}

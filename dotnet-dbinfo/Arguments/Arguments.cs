using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace dotnet_dbinfo.Arguments
{
    public interface IArguments
    {
        string ResultPath { get; }
    }

    public abstract class Arguments : IArguments
    {
        public string ResultPath { get; protected set; }
    }
}

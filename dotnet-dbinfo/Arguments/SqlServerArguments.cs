using System;
using System.Collections.Generic;
using System.Text;

namespace dotnet_dbinfo.Arguments
{
    public class SqlServerArguments : Arguments
    {
        public string Server { get; }

        public string Database { get; }

        public string User { get; }

        public string Password { get; }

        public SqlServerArguments(string[] args)
        {
            Server = args[1];
            Database = args[2];
            User = args[3];
            Password = args[4];
            ResultPath = args.Length > 5 ? args[5] : null;
        }
    }
}

using dotnet_dbinfo.Enums;
using System;

namespace dotnet_dbinfo
{
    public class Options
    {
        public SupportedDatabaseType SupportedDatabaseType { get; }

        public string Server { get; }

        public string Database { get; }

        public string User { get; }

        public string Password { get; }

        public string ResultPath { get; }

        public Options(string[] args)
        {
            SupportedDatabaseType = Enum.Parse<SupportedDatabaseType>(args[0], true);
            Server = args[1];
            Database = args[2];
            User = args[3];
            Password = args[4];
            ResultPath = args.Length > 5 ? args[5] : null;
        }
    }
}

using dotnet_dbinfo.Enums;

namespace dotnet_dbinfo
{
    public class Options
    {
        public SupportedDatabaseType SupportedDatabaseType { get; } = SupportedDatabaseType.SQL_SERVER;

        public string Server { get; set; }

        public string Database { get; set; }

        public string User { get; set; }

        public string Password { get; set; }

        public string ResultPath { get; set; }
    }
}

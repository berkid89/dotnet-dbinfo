using dotnet_dbinfo.Models;
using System.Collections.Generic;

namespace dotnet_dbinfo.InfoCollectors
{
    public interface IInfoCollector
    {
        DbInfo GetGeneralInfo();

        IEnumerable<RowCountInfo> GetRowcounts();

        IEnumerable<IndexInfo> GetFragmentedIndexes();
    }
}

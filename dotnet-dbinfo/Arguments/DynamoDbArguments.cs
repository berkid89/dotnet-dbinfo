using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dotnet_dbinfo.Arguments
{
    public class DynamoDbArguments : Arguments
    {
        public RegionEndpoint RegionEndpoint { get; }

        public AWSCredentials Credentials { get; }

        public DynamoDbArguments(string[] args)
        {
            RegionEndpoint = RegionEndpoint.EnumerableAllRegions.First(p => p.SystemName == args[1]);

            Credentials = args.Length > 3 ? new BasicAWSCredentials("AKIAINQOC5I4J75FAOXA", "TyAwIY6UiQGGzV30LbkfRulcRrJAO5usLky21cXQ") : null;
            //args[2];
            //args[3];

            ResultPath = args.Length > 4 ? args[4] : null;
        }
    }
}

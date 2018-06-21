# dotnet-dbinfo

[![NuGet][main-nuget-badge]][main-nuget]

[main-nuget]: https://www.nuget.org/packages/dotnet-dbinfo/
[main-nuget-badge]: https://img.shields.io/nuget/v/dotnet-dbinfo.svg?style=flat-square&label=nuget

A simple cross-platform command-line tool for get useful database information (in json format).

## Get started

Download the .NET Core SDK [2.1.300](https://aka.ms/DotNetCore21) or newer.
Once installed, run this command:

```
dotnet tool install -g dotnet-dbinfo
```

## Usage

### Microsoft SQL Server

```
Usage: dotnet dbinfo sqlserver server database user password

Arguments:

  server                    Name of the database server

  database                  Name of the database instance

  user                      Login name of the user

  password                  Password of the user        
```

Example:
```
dotnet dbinfo sqlserver .\SQLEXPRESS master sa Pass1234
```

### AWS DynamoDb

```
Usage: dotnet dbinfo dynamodb regionendpoint [accesskey] [secretkey]

Arguments:

  regionendpoint            Region (for example: us-east-1)

  accesskey/secretkey       (OPTIONAL) If not set, tool uses AWS credentials profile file on your local system         
  
```

Example:
```
dotnet dbinfo dynamodb us-east-1
```

> **Hint:** The result path **must be exist** and the **user** must have the necessary **permissions** for the database!


# dotnet-dbinfo

[![NuGet][main-nuget-badge]][main-nuget]

[main-nuget]: https://www.nuget.org/packages/dotnet-dbinfo/
[main-nuget-badge]: https://img.shields.io/nuget/v/dotnet-dbinfo.svg?style=flat-square&label=nuget

A simple command-line tool for get useful database information (in json format).

## Get started

Download the .NET Core SDK [2.1.300](https://aka.ms/DotNetCore21) or newer.
Once installed, run this command:

```
dotnet tool install -g dotnet-dbinfo
```

## Usage

```
Usage: dotnet dbinfo databasetype server database user password [resultpath]

Arguments:

  databasetype              Type of the database server (now only sqlserver supported)

  server                    Name of the database server

  database                  Name of the database instance

  user                      Login name of the user

  password                  Password of the user

  resultpath                (OPTIONAL) Full path of the result text file              
  
```

Example:
```
dotnet dbinfo sqlserver .\SQLEXPRESS master sa Pass1234
```

or

```
dotnet dbinfo sqlserver .\SQLEXPRESS master sa Pass1234 c:\temp\res.json
```

> **Hint:** The result path **must be exist** and the **user** must have the necessary **permissions** for the database!


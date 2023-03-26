[<img align="right" width="250" height="auto" src="https://searchpioneer.com/assets/svg/logos/logo.svg">](https://searchpioneer.com/)

Learn more about our services and expertise: [https://searchpioneer.com/](https://searchpioneer.com/)

# Weaviate .NET client

A Microsoft .NET native client for [Weaviate](https://weaviate.io/)

## Build status

![Tests status](https://github.com/searchpioneer/weaviate-dotnet-client/actions/workflows/tests.yml/badge.svg)

## Packages

The .NET assembly is published to NuGet under the package name [SearchPioneer.Weaviate.Client](http://nuget.org/packages/SearchPioneer.Weaviate.Client)

## Versioning

The version of the SearchPioneer.Weaviate.Client package matches the published Weaviate server version.

A verification check takes place on creation of the client to ensure that the client and server versions match.

## Getting started

### Installing

You can install SearchPioneer.Weaviate.Client from the package manager console:

    PM> Install-Package SearchPioneer.Weaviate.Client

Alternatively, simply search for SearchPioneer.Weaviate.Client in the package manager UI.

### Usage

```csharp
using SearchPioneer.Weaviate.Client;
using Flurl.Http;

public class Main
{
    public void Main()
    {
        var flurlClient = new FlurlClient();
        var weaviateClient = new WeaviateClient(new Config("http", "localhost:8080"), flurlClient);
        var meta = weaviateClient.Misc.Meta();
        Console.WriteLine(meta.Error != null ? meta.Error.Message : meta.Result.Version);
    }
}
```

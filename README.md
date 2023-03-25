[<img align="right" width="250" height="auto" src="https://searchpioneer.com/assets/svg/logos/logo.svg">](https://searchpioneer.com/)

Learn more about our services and expertise: [https://searchpioneer.com/](https://searchpioneer.com/)

# Weaviate .NET client

A Microsoft .NET native client for [Weaviate](https://weaviate.io/)

![Tests status](https://github.com/searchpioneer/weaviate-dotnet-client/actions/workflows/tests.yml/badge.svg)

## Usage

You need to add the dependancy: `SearchPioneer.Weaviate.Client`.

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

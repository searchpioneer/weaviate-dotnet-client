![Tests status](https://github.com/searchpioneer/weaviate-dotnet-client/actions/workflows/tests.yml/badge.svg)

# Search Pioneer

## Weaviate .NET client

A Microsoft .NET native client for [Weaviate](https://weaviate.io/)

### Usage

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

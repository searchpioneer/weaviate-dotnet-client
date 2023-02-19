![Tests status](https://github.com/searchpioneer/weaviate-dotnet-client/actions/workflows/tests.yml/badge.svg)

# Weaviate .NET client

A Microsoft .NET native client for [Weaviate](https://weaviate.io/)

## Usage

You need to add the dependancy: `SearchPioneer.Weaviate.Client`.

```csharp
using SearchPioneer.Weaviate.Client;

public class Main
{
    public void Main()
    {
        var httpClient = new HttpClient();
        var weaviateClient = new WeaviateClient(new Config("http", "localhost:8080"), httpClient);
        var meta = weaviateClient.Misc.Meta();
        Console.WriteLine(meta.Error != null ? meta.Error.Message : meta.Result.Version);
    }
}
```

### HttpClient Best Practices

See the [NET documentation](https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/http/httpclient-guidelines)
for best practices.

### Authorisation

Should you need to authenticate to the server, you should use a delegating handler to augment the requests.

```csharp
using SearchPioneer.Weaviate.Client;

public class Main
{
    public class AuthorisationHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Add("Authorization", "Bearer <put your token here>");
            return await base.SendAsync(request, cancellationToken);
        }
    }
    
    public void Main()
    {
        var httpClient = new HttpClient(new AuthorisationHandler()); // Use the AuthorisationHandler
        var weaviateClient = new WeaviateClient(new Config("http", "localhost:8080"), httpClient);
        var meta = weaviateClient.Misc.Meta();
        Console.WriteLine(meta.Error != null ? meta.Error.Message : meta.Result.Version);
    }
}
```
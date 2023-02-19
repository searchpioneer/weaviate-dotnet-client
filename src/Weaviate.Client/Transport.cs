// Copyright (C) 2023 Search Pioneer - https://www.searchpioneer.com
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//         http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Text;
using SearchPioneer.Weaviate.Client.Api.Graph.Response;

namespace SearchPioneer.Weaviate.Client;

public class Transport
{
    private readonly Config _config;
    private readonly HttpClient _httpClient;
    private readonly Serializer _serializer = new();

    public Transport(Config config, HttpClient httpClient)
    {
        _config = config;
        _httpClient = httpClient;
    }

    public ApiResponse<TResult> Get<TResult>(string endpoint)
    {
        return SendRequest<TResult>(endpoint, null, HttpMethod.Get);
    }

    public ApiResponse<TResult> Post<TRequest, TResult>(string endpoint, TRequest payload)
    {
        return SendRequest<TResult>(endpoint, payload, HttpMethod.Post);
    }

    public ApiResponse<TResult> Put<TRequest, TResult>(string endpoint, TRequest payload)
    {
        return SendRequest<TResult>(endpoint, payload, HttpMethod.Put);
    }

    public ApiResponse<TResult> Patch<TRequest, TResult>(string endpoint, TRequest payload)
    {
        return SendRequest<TResult>(endpoint, payload, HttpMethod.Patch);
    }

    public ApiResponse<bool> Delete(string endpoint, object? payload = null)
    {
        var response = SendRequest<object>(endpoint, payload, HttpMethod.Delete);
        return response.MapToBool(response.HttpStatusCode == 200);
    }

    public ApiResponse<TResult> Delete<TResult>(string endpoint, object? payload = null)
    {
        var response = SendRequest<TResult>(endpoint, payload, HttpMethod.Delete);
        return response;
    }

    public ApiResponse<TResult> Head<TResult>(string endpoint)
    {
        return SendRequest<TResult>(endpoint, null, HttpMethod.Head);
    }

    private ApiResponse<TResult> SendRequest<TResult>(string endpoint, object? payload, HttpMethod method)
    {
        var result = new ApiResponse<TResult>
        {
            Uri = new(_config.GetBaseUrl() + endpoint),
            HttpMethod = method,
            RequestBody = payload != null ? _serializer.Serialize(payload) : null
        };

        var response = SendHttpRequest(result.Uri, result.RequestBody, result.HttpMethod);

        result.HttpStatusCode = (int)response.StatusCode;
        var bytes = response.Content.ReadAsByteArrayAsync().Result;
        if (bytes.Length > 0) result.ResponseBody = Encoding.UTF8.GetString(bytes);

        if (result.HttpStatusCode < 399)
        {
            if (!string.IsNullOrEmpty(result.ResponseBody))
            {
                result.Result = _serializer.Deserialize<TResult>(result.ResponseBody);

                if (result.Result is GraphResponse graphResponse)
                    if (graphResponse.Errors is { Length: > 0 })
                    {
                        result.Error = new()
                        {
                            Message = string.Join(",", graphResponse.Errors.Select(e => e.Message).ToArray()),
                            Error = graphResponse.Errors.Select(e => new ApiErrorMessage
                            {
                                Message = e.Message
                            }).ToList()
                        };

                        // TODO: Should we do this and remap if errors?
                        result.HttpStatusCode = 500;
                    }
            }

            return result;
        }

        if (!string.IsNullOrEmpty(result.ResponseBody))
            result.Error = _serializer.Deserialize<ApiError>(result.ResponseBody);

        return result;
    }

    private HttpResponseMessage SendHttpRequest(Uri uri, string? requestBody, HttpMethod method)
    {
        if (method == HttpMethod.Delete && requestBody == null)
            return _httpClient.Send(new(HttpMethod.Delete, uri));
        if (method == HttpMethod.Head)
            return _httpClient.Send(new(HttpMethod.Head, uri));
        if (method == HttpMethod.Get)
            return _httpClient.GetAsync(uri).Result;

        ArgumentNullException.ThrowIfNull(requestBody, nameof(requestBody));

        var content = new JsonContent(requestBody);
        if (method == HttpMethod.Delete)
            return _httpClient.Send(new(HttpMethod.Delete, uri) { Content = content });
        if (method == HttpMethod.Post)
            return _httpClient.PostAsync(uri, content).Result;
        if (method == HttpMethod.Put)
            return _httpClient.PutAsync(uri, content).Result;
        if (method == HttpMethod.Patch)
            return _httpClient.PatchAsync(uri, content).Result;

        throw new ArgumentOutOfRangeException(nameof(method));
    }

    private class JsonContent : ByteArrayContent
    {
        public JsonContent(string content)
            : base(Encoding.UTF8.GetBytes(content))
        {
            Headers.ContentType = new("application/json");
            Headers.Add("charset", "utf-8");
        }
    }
}
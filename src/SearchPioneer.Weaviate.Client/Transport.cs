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

using Flurl.Http;

// ReSharper disable once CheckNamespace
namespace SearchPioneer.Weaviate.Client;

public class Transport
{
    private readonly Config _config;
    private readonly IFlurlClient _flurlClient;
    private readonly Serializer _serializer = new();

    public Transport(Config config, IFlurlClient flurlClient)
    {
        _config = config;
        _flurlClient = flurlClient;
        _flurlClient.Settings.AllowedHttpStatusRange = "*";
        _flurlClient.Headers.Add("Content-Type", "application/json");
        foreach (var header in _config.GetHeaders())
        {
	        _flurlClient.Headers.Add(header.Key, header.Value);
        }
        if (config.Timeout.HasValue)
        {
	        _flurlClient.WithTimeout(config.Timeout.Value);
        }
    }

    private async Task EnrichResult<TResult>(ApiResponse<TResult> result, IFlurlResponse response)
    {
	    result.HttpStatusCode = response.StatusCode;
	    result.ResponseBody = await response.GetStringAsync().ConfigureAwait(false);

	    if (result.HttpStatusCode < 399)
	    {
		    if (!string.IsNullOrEmpty(result.ResponseBody))
		    {
			    result.Result = _serializer.Deserialize<TResult>(result.ResponseBody);

			    if (result.Result is GraphResponse graphResponse)
			    {
				    if (graphResponse.Errors is { Length: > 0 })
				    {
					    result.Error = new()
					    {
						    Message = string.Join(",", graphResponse.Errors.Select(e => e.Message).ToArray()),
						    Error = graphResponse.Errors.Select(e => new ApiErrorMessage { Message = e.Message })
							    .ToList()
					    };

					    // TODO: Should we do this and remap if errors?
					    result.HttpStatusCode = 500;
				    }
			    }
		    }
	    }

	    if (!string.IsNullOrEmpty(result.ResponseBody) && result.HttpStatusCode >= 399)
		    result.Error = _serializer.Deserialize<ApiError>(result.ResponseBody);
    }

    private ApiResponse GetResult<TRequest>(string endpoint, TRequest payload, string httpMethod)
    {
	    var result = new ApiResponse
	    {
		    Uri = _config.GetBaseUrl() + endpoint,
		    HttpMethod = httpMethod,
		    RequestBody = _serializer.Serialize(payload)
	    };
	    return result;
    }

    private ApiResponse<TResult> GetResult<TRequest, TResult>(string endpoint, TRequest payload, string httpMethod)
    {
	    var result = new ApiResponse<TResult>
	    {
		    Uri = _config.GetBaseUrl() + endpoint,
		    HttpMethod = httpMethod,
		    RequestBody = _serializer.Serialize(payload)
	    };
	    return result;
    }

    private ApiResponse<TResult> GetResult<TResult>(string endpoint, string httpMethod)
    {
	    var result = new ApiResponse<TResult>
	    {
		    Uri = _config.GetBaseUrl() + endpoint,
		    HttpMethod = httpMethod,
		    RequestBody = null
	    };
	    return result;
    }

    private ApiResponse GetResult(string endpoint, string httpMethod)
    {
	    var result = new ApiResponse
	    {
		    Uri = _config.GetBaseUrl() + endpoint,
		    HttpMethod = httpMethod,
		    RequestBody = null
	    };
	    return result;
    }

    public async Task<ApiResponse<TResult>> GetAsync<TResult>(string endpoint, CancellationToken cancellationToken = default)
    {
	    var result = GetResult<TResult>(endpoint, "GET");
	    var response = await _flurlClient.Request(result.Uri).GetAsync(cancellationToken).ConfigureAwait(false);
	    await EnrichResult(result, response).ConfigureAwait(false);
	    return result;
    }

    public async Task<ApiResponse> GetAsync(string endpoint, CancellationToken cancellationToken = default)
    {
	    var result = GetResult(endpoint, "GET");
	    var response = await _flurlClient.Request(result.Uri).GetAsync(cancellationToken).ConfigureAwait(false);
	    await MapHttpStatusCodeAndErrors(result, response).ConfigureAwait(false);
	    return result;
    }

    public async Task<ApiResponse<TResult>> PostAsync<TRequest, TResult>(string endpoint, TRequest payload, CancellationToken cancellationToken = default)
    {
	    var result = GetResult<TRequest, TResult>(endpoint, payload, "POST");
	    var response = await _flurlClient.Request(result.Uri).PostStringAsync(result.RequestBody, cancellationToken).ConfigureAwait(false);
	    await EnrichResult(result, response).ConfigureAwait(false);
	    return result;
    }

    public async Task<ApiResponse> PostAsync<TRequest>(string endpoint, TRequest payload, CancellationToken cancellationToken = default)
    {
	    var result = GetResult(endpoint, payload, "POST");
	    var response = await _flurlClient.Request(result.Uri).PostStringAsync(result.RequestBody, cancellationToken).ConfigureAwait(false);
	    await MapHttpStatusCodeAndErrors(result, response).ConfigureAwait(false);
	    return result;
    }

    private async Task MapHttpStatusCodeAndErrors(ApiResponse result, IFlurlResponse response)
    {
	    result.HttpStatusCode = response.StatusCode;
	    result.ResponseBody = await response.GetStringAsync().ConfigureAwait(false);
	    if (!string.IsNullOrEmpty(result.ResponseBody) && result.HttpStatusCode >= 399)
		    result.Error = _serializer.Deserialize<ApiError>(result.ResponseBody);
    }

    public async Task<ApiResponse<TResult>> PutAsync<TRequest, TResult>(string endpoint, TRequest payload, CancellationToken cancellationToken = default)
    {
	    var result = GetResult<TRequest, TResult>(endpoint, payload, "PUT");
	    var response = await _flurlClient.Request(result.Uri).PutStringAsync(result.RequestBody, cancellationToken).ConfigureAwait(false);
	    await EnrichResult(result, response).ConfigureAwait(false);
	    return result;
    }

    public async Task<ApiResponse> PutAsync<TRequest>(string endpoint, TRequest payload, CancellationToken cancellationToken = default)
    {
	    var result = GetResult(endpoint, payload, "PUT");
	    var response = await _flurlClient.Request(result.Uri).PutStringAsync(result.RequestBody, cancellationToken).ConfigureAwait(false);
	    await MapHttpStatusCodeAndErrors(result, response).ConfigureAwait(false);
	    return result;
    }

    public async Task<ApiResponse<TResult>> PatchAsync<TRequest, TResult>(string endpoint, TRequest payload, CancellationToken cancellationToken = default)
    {
	    var result = GetResult<TRequest, TResult>(endpoint, payload, "PATCH");
	    var response = await _flurlClient.Request(result.Uri).PatchStringAsync(result.RequestBody, cancellationToken).ConfigureAwait(false);
	    await EnrichResult(result, response).ConfigureAwait(false);
	    return result;
    }

    public async Task<ApiResponse<TResult>> HeadAsync<TResult>(string endpoint, CancellationToken cancellationToken = default)
    {
	    var result = GetResult<TResult>(endpoint, "HEAD");
	    var response = await _flurlClient.Request(result.Uri).HeadAsync(cancellationToken).ConfigureAwait(false);
	    await EnrichResult(result, response).ConfigureAwait(false);
	    return result;
    }

    public async Task<ApiResponse> HeadAsync(string endpoint, CancellationToken cancellationToken = default)
    {
	    var result = GetResult(endpoint, "HEAD");
	    var response = await _flurlClient.Request(result.Uri).HeadAsync(cancellationToken).ConfigureAwait(false);
	    await MapHttpStatusCodeAndErrors(result, response).ConfigureAwait(false);
	    return result;
    }

    public async Task<ApiResponse<TResult>> DeleteAsync<TRequest, TResult>(string endpoint, TRequest payload, CancellationToken cancellationToken = default)
    {
	    var result = GetResult<TRequest, TResult>(endpoint, payload, "DELETE");
	    var response = await _flurlClient.Request(result.Uri).SendStringAsync(HttpMethod.Delete, result.RequestBody, cancellationToken).ConfigureAwait(false);
	    await EnrichResult(result, response).ConfigureAwait(false);
	    return result;
    }

    public async Task<ApiResponse> DeleteAsync<TRequest>(string endpoint, TRequest payload, CancellationToken cancellationToken = default)
    {
	    var result = GetResult(endpoint, payload, "DELETE");
	    var response = await _flurlClient.Request(result.Uri).SendStringAsync(HttpMethod.Delete, result.RequestBody, cancellationToken).ConfigureAwait(false);
	    await MapHttpStatusCodeAndErrors(result, response).ConfigureAwait(false);
	    return result;
    }

    public async Task<ApiResponse> DeleteAsync(string endpoint, CancellationToken cancellationToken = default)
    {
	    var result = GetResult(endpoint, "DELETE");
	    var response = await _flurlClient.Request(result.Uri).DeleteAsync(cancellationToken).ConfigureAwait(false);
	    await MapHttpStatusCodeAndErrors(result, response).ConfigureAwait(false);
	    return result;
    }
}

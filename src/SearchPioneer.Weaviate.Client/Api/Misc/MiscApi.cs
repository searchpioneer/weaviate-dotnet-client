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

// ReSharper disable once CheckNamespace
namespace SearchPioneer.Weaviate.Client;

/// <summary>
/// Miscellaneous operations
/// </summary>
public class MiscApi
{
    private readonly Transport _transport;

    internal MiscApi(Transport transport) => _transport = transport;

    /// <summary>
    ///     The RESTful meta endpoint gives information about the current Weaviate instance.
    ///     It can be used to learn about your current Weaviate instance and to provide information to
    ///     another Weaviate instants that wants to interact with this instance.
    /// </summary>
    public ApiResponse<Meta> Meta()
    {
        var response = _transport.GetAsync<Meta>("/meta").GetAwaiter().GetResult();
        return response;
    }

    /// <summary>
    ///     The RESTful meta endpoint gives information about the current Weaviate instance.
    ///     It can be used to learn about your current Weaviate instance and to provide information to
    ///     another Weaviate instants that wants to interact with this instance.
    /// </summary>
    public async Task<ApiResponse<Meta>> MetaAsync(CancellationToken cancellationToken = default)
    {
	    var response = await _transport.GetAsync<Meta>("/meta", cancellationToken).ConfigureAwait(false);
        return response;
    }

    /// <summary>
    ///     The RESTful API discovery gives information if OpenID Connect (OIDC) authentication is enabled.
    ///     The endpoint redirects to the token issued if one is configured.
    /// </summary>
    public ApiResponse<OpenIDConfiguration> OpenIdConfiguration()
    {
        var response = _transport.GetAsync<OpenIDConfiguration>("/.well-known/openid-configuration").GetAwaiter().GetResult();
        return response;
    }

    /// <summary>
    ///     The RESTful API discovery gives information if OpenID Connect (OIDC) authentication is enabled.
    ///     The endpoint redirects to the token issued if one is configured.
    /// </summary>
    public async Task<ApiResponse<OpenIDConfiguration>> OpenIdConfigurationAsync(CancellationToken cancellationToken = default)
    {
	    var response = await _transport
		    .GetAsync<OpenIDConfiguration>("/.well-known/openid-configuration", cancellationToken)
		    .ConfigureAwait(false);
        return response;
    }

    /// <summary>
    ///     Determines whether the application is alive. It can be used for Kubernetes live-ness probe.
    /// </summary>
    public ApiResponse Live()
    {
        var response = _transport.GetAsync("/.well-known/live").GetAwaiter().GetResult();
        return response;
    }

    /// <summary>
    ///     Determines whether the application is alive. It can be used for Kubernetes live-ness probe.
    /// </summary>
    public async Task<ApiResponse> LiveAsync(CancellationToken cancellationToken = default)
    {
        var response = await _transport.GetAsync<object>("/.well-known/live", cancellationToken).ConfigureAwait(false);
        return response;
    }

    /// <summary>
    ///     Determines whether the application is ready to receive traffic. It can be used for Kubernetes readiness probe.
    /// </summary>
    public ApiResponse Ready()
    {
        var response = _transport.GetAsync("/.well-known/ready").GetAwaiter().GetResult();
        return response;
    }

    /// <summary>
    ///     Determines whether the application is ready to receive traffic. It can be used for Kubernetes readiness probe.
    /// </summary>
    public async Task<ApiResponse> ReadyAsync(CancellationToken cancellationToken = default)
    {
	    var response = await _transport.GetAsync("/.well-known/ready", cancellationToken).ConfigureAwait(false);
        return response;
    }
}

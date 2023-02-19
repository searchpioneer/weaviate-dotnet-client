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

using SearchPioneer.Weaviate.Client.Api.Misc.Model;

namespace SearchPioneer.Weaviate.Client.Api.Misc;

public class MiscApi
{
    private readonly Transport _transport;

    internal MiscApi(Transport transport)
    {
        _transport = transport;
    }

    /// <summary>
    ///     The RESTful meta endpoint gives information about the current Weaviate instance.
    ///     It can be used to learn about your current Weaviate instance and to provide information to
    ///     another Weaviate instants that wants to interact with this instance.
    /// </summary>
    public ApiResponse<Meta> Meta()
    {
        var response = _transport.Get<Meta>("/meta");
        return response;
    }

    /// <summary>
    ///     The RESTful API discovery gives information if OpenID Connect (OIDC) authentication is enabled.
    ///     The endpoint redirects to the token issued if one is configured.
    /// </summary>
    public ApiResponse<OpenIDConfiguration> OpenIdConfiguration()
    {
        var response = _transport.Get<OpenIDConfiguration>("/.well-known/openid-configuration");
        return response;
    }

    /// <summary>
    ///     Live determines whether the application is alive. It can be used for Kubernetes liveness probe.
    /// </summary>
    public ApiResponse<bool> Live()
    {
        var response = _transport.Get<object>("/.well-known/live");
        return response.MapToBool(response.HttpStatusCode == 200);
    }

    /// <summary>
    ///     Live determines whether the application is ready to receive traffic. It can be used for Kubernetes readiness probe.
    /// </summary>
    public ApiResponse<bool> Ready()
    {
        var response = _transport.Get<object>("/.well-known/ready");
        return response.MapToBool(response.HttpStatusCode == 200);
    }
}
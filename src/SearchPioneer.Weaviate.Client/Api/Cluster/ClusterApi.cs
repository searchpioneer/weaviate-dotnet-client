﻿// Copyright (C) 2023 Search Pioneer - https://www.searchpioneer.com
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
/// Cluster operations
/// </summary>
public class ClusterApi
{
    private readonly Transport _transport;

    internal ClusterApi(Transport transport) => _transport = transport;

    /// <summary>
    ///     Gets the status of all Weaviate nodes
    /// </summary>
    public ApiResponse<NodesStatusResponse> NodeStatus()
    {
        var response = _transport.GetAsync<NodesStatusResponse>("/nodes").GetAwaiter().GetResult();
        return response;
    }

    /// <summary>
    ///     Gets the status of all Weaviate nodes
    /// </summary>
    public async Task<ApiResponse<NodesStatusResponse>> NodeStatusAsync(CancellationToken cancellationToken = default)
    {
        var response = await _transport.GetAsync<NodesStatusResponse>("/nodes", cancellationToken).ConfigureAwait(false);
        return response;
    }
}

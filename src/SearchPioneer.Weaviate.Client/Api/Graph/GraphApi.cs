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
/// Graph operations
/// </summary>
public class GraphApi
{
    private readonly Transport _transport;

    internal GraphApi(Transport transport) => _transport = transport;

    public ApiResponse<GraphResponse> Explore(GraphExploreRequest request) => Graph(request.ToString());

    public async Task<ApiResponse<GraphResponse>> ExploreAsync(GraphExploreRequest request, CancellationToken cancellationToken = default)
	    => await GraphAsync(request.ToString(), cancellationToken).ConfigureAwait(false);

    public ApiResponse<GraphResponse> Aggregate(GraphAggregateRequest request) => Graph(request.ToString());

    public async Task<ApiResponse<GraphResponse>> AggregateAsync(GraphAggregateRequest request, CancellationToken cancellationToken = default)
	    => await GraphAsync(request.ToString(), cancellationToken).ConfigureAwait(false);

    public ApiResponse<GraphResponse> Get(GraphGetRequest request) => Graph(request.ToString());

    public async Task<ApiResponse<GraphResponse>> GetAsync(GraphGetRequest request, CancellationToken cancellationToken = default)
	    => await GraphAsync(request.ToString(), cancellationToken).ConfigureAwait(false);

    private ApiResponse<GraphResponse> Graph(string queryText)
    {
        var query = new GraphQuery
        {
            Query = queryText
        };
        var response = _transport.PostAsync<GraphQuery, GraphResponse>("/graphql", query).GetAwaiter().GetResult();
        return response;
    }

    private async Task<ApiResponse<GraphResponse>> GraphAsync(string queryText, CancellationToken cancellationToken = default)
    {
        var query = new GraphQuery
        {
            Query = queryText
        };
        var response = await _transport.PostAsync<GraphQuery, GraphResponse>("/graphql", query, cancellationToken).ConfigureAwait(false);
        return response;
    }

    private class GraphQuery
    {
        public string? OperationName { get; set; } // TODO! never used?
        public string? Query { get; set; }
        public object? Variables { get; set; } // TODO! never used?
    }
}

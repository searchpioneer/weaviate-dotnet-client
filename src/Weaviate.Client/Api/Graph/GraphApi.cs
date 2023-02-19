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

using SearchPioneer.Weaviate.Client.Api.Graph.Request;
using SearchPioneer.Weaviate.Client.Api.Graph.Response;

namespace SearchPioneer.Weaviate.Client.Api.Graph;

public class GraphApi
{
    private readonly Transport _transport;

    internal GraphApi(Transport transport)
    {
        _transport = transport;
    }

    public ApiResponse<GraphResponse> Explore(GraphExploreRequest request)
    {
        return Graph(request.ToString());
    }

    public ApiResponse<GraphResponse> Aggregate(GraphAggregateRequest request)
    {
        return Graph(request.ToString());
    }

    public ApiResponse<GraphResponse> Get(GraphGetRequest request)
    {
        return Graph(request.ToString());
    }

    private ApiResponse<GraphResponse> Graph(string queryText)
    {
        var query = new GraphQuery
        {
            Query = queryText
        };
        var response = _transport.Post<GraphQuery, GraphResponse>("/graphql", query);
        return response;
    }

    private class GraphQuery
    {
        public string OperationName { get; set; } // TODO! never used?
        public string Query { get; set; }
        public object Variables { get; set; } // TODO! never used?
    }
}
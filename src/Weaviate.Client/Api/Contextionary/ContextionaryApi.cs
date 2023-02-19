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

using SearchPioneer.Weaviate.Client.Api.Contextionary.Request;
using SearchPioneer.Weaviate.Client.Api.Contextionary.Response;

namespace SearchPioneer.Weaviate.Client.Api.Contextionary;

public class ContextionaryApi
{
    private readonly Transport _transport;

    internal ContextionaryApi(Transport transport)
    {
        _transport = transport;
    }

    public ApiResponse<WordsResponse> GetConcepts(GetConceptsRequest request)
    {
        var path = $"/modules/text2vec-contextionary/concepts/{request.Concept}";
        var response = _transport.Get<WordsResponse>(path);
        return response;
    }

    public ApiResponse<bool> CreateExtension(CreateExtensionRequest request)
    {
        // TODO! move to constructor parameter with validation?
        if (request.Weight is > 1f or < 0f)
            throw new ArgumentOutOfRangeException(nameof(request.Weight), "weight has to be between 0 and 1");

        const string endpoint = "/modules/text2vec-contextionary/extensions";
        var response = _transport.Post<CreateExtensionRequest, object>(endpoint, request);
        return response.MapToBool(response.HttpStatusCode == 200);
    }
}
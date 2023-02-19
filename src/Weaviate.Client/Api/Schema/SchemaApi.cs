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

using SearchPioneer.Weaviate.Client.Api.Schema.Model;
using SearchPioneer.Weaviate.Client.Api.Schema.Requests;

namespace SearchPioneer.Weaviate.Client.Api.Schema;

public class SchemaApi
{
    private readonly Transport _transport;

    public SchemaApi(Transport transport)
    {
        _transport = transport;
    }

    public ApiResponse<WeaviateSchema> GetSchema()
    {
        return _transport.Get<WeaviateSchema>("/schema");
    }

    public ApiResponse<WeaviateClass> GetClass(GetClassRequest request)
    {
        return _transport.Get<WeaviateClass>($"/schema/{request.Class}");
    }

    public ApiResponse<Shard[]> GetShards(GetShardsRequest request)
    {
        return _transport.Get<Shard[]>($"/schema/{request.Class}/shards");
    }

    public ApiResponse<WeaviateClass> CreateClass(CreateClassRequest request)
    {
        return _transport.Post<WeaviateClass, WeaviateClass>("/schema", request);
    }

    public void UpdateShards(UpdateShardsRequest request)
    {
        // TODO! this needs cleaning up
        var shards = GetShards(new(request.Class));
        foreach (var shard in shards.Result)
            UpdateShard(new(request.Class, shard.Name, request.Status));
    }

    public ApiResponse<ShardStatus> UpdateShard(UpdateShardRequest request)
    {
        var response = _transport.Put<StatusContainer, StatusContainer>(
            $"/schema/{request.Class}/shards/{request.Shard}", new()
            {
                Status = request.Status
            });
        return new()
        {
            Error = response.Error,
            Uri = response.Uri,
            HttpStatusCode = response.HttpStatusCode,
            HttpMethod = response.HttpMethod,
            RequestBody = response.RequestBody,
            ResponseBody = response.ResponseBody,
            Result = response.Result.Status
        };
    }

    public ApiResponse<bool> DeleteClass(DeleteClassRequest request)
    {
        return _transport.Delete($"/schema/{request.Class}");
    }

    public void DeleteAllClasses()
    {
        // TODO! this needs cleaning up
        var schema = GetSchema();
        foreach (var weaviateClass in schema.Result.Classes)
            DeleteClass(new(weaviateClass.Class));
    }

    public ApiResponse<Property> CreateProperty(CreatePropertyRequest request)
    {
        return _transport.Post<Property, Property>($"/schema/{request.Class}/properties", request.Property);
    }

    private class StatusContainer
    {
        public ShardStatus Status { get; set; }
    }
}
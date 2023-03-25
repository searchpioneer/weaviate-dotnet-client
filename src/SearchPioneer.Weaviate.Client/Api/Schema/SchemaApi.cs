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
/// Schema API operations
/// </summary>
public class SchemaApi
{
    private readonly Transport _transport;

    internal SchemaApi(Transport transport) => _transport = transport;

    public ApiResponse<WeaviateSchema> GetSchema()
	    => _transport.GetAsync<WeaviateSchema>("/schema").GetAwaiter().GetResult();

    public async Task<ApiResponse<WeaviateSchema>> GetSchemaAsync(CancellationToken cancellationToken = default)
	    => await _transport.GetAsync<WeaviateSchema>("/schema", cancellationToken).ConfigureAwait(false);

    public ApiResponse<WeaviateClass> GetClass(GetClassRequest request)
	    => _transport.GetAsync<WeaviateClass>($"/schema/{request.Class}").GetAwaiter().GetResult();

    public async Task<ApiResponse<WeaviateClass>> GetClassAsync(GetClassRequest request, CancellationToken cancellationToken = default)
	    => await _transport.GetAsync<WeaviateClass>($"/schema/{request.Class}", cancellationToken).ConfigureAwait(false);

    public ApiResponse<Shard[]> GetShards(GetShardsRequest request)
	    => _transport.GetAsync<Shard[]>($"/schema/{request.Class}/shards").GetAwaiter().GetResult();

    public async Task<ApiResponse<Shard[]>> GetShardsAsync(GetShardsRequest request, CancellationToken cancellationToken = default)
	    => await _transport.GetAsync<Shard[]>($"/schema/{request.Class}/shards", cancellationToken).ConfigureAwait(false);

    public ApiResponse<WeaviateClass> CreateSchemaClass(CreateClassRequest request)
	    => _transport.PostAsync<WeaviateClass, WeaviateClass>("/schema", request).GetAwaiter().GetResult();

    public async Task<ApiResponse<WeaviateClass>> CreateSchemaClassAsync(CreateClassRequest request, CancellationToken cancellationToken = default)
	    => await _transport.PostAsync<WeaviateClass, WeaviateClass>("/schema", request, cancellationToken).ConfigureAwait(false);

    public ApiResponse<ShardStatus> UpdateShard(UpdateShardRequest request)
    {
	    var response = _transport.PutAsync<StatusContainer, StatusContainer>(
            $"/schema/{request.Class}/shards/{request.Shard}", new()
            {
                Status = request.Status
            }).GetAwaiter().GetResult();
	    return GetResponse(response);
    }

    public async Task<ApiResponse<ShardStatus>> UpdateShardAsync(UpdateShardRequest request, CancellationToken cancellationToken = default)
    {
	    var response = await _transport.PutAsync<StatusContainer, StatusContainer>(
			    $"/schema/{request.Class}/shards/{request.Shard}", new() { Status = request.Status }, cancellationToken).ConfigureAwait(false);
	    return GetResponse(response);
    }

    public ApiResponse DeleteClass(DeleteClassRequest request)
	    => _transport.DeleteAsync($"/schema/{request.Class}").GetAwaiter().GetResult();

    public async Task<ApiResponse> DeleteClassAsync(DeleteClassRequest request, CancellationToken cancellationToken = default)
	    => await _transport.DeleteAsync($"/schema/{request.Class}", cancellationToken).ConfigureAwait(false);

    public ApiResponse<Property> CreateProperty(CreatePropertyRequest request)
	    => _transport.PostAsync<Property, Property>($"/schema/{request.Class}/properties", request.Property).GetAwaiter().GetResult();

    public async Task<ApiResponse<Property>> CreatePropertyAsync(CreatePropertyRequest request, CancellationToken cancellationToken = default)
	    => await _transport.PostAsync<Property, Property>($"/schema/{request.Class}/properties", request.Property, cancellationToken).ConfigureAwait(false);

    public void UpdateShards(UpdateShardsRequest request)
    {
	    // TODO! this needs cleaning up
	    var shards = GetShards(new(request.Class));
	    foreach (var shard in shards.Result!)
		    UpdateShard(new(request.Class, shard.Name, request.Status));
    }

    public async Task UpdateShardsAsync(UpdateShardsRequest request, CancellationToken cancellationToken = default)
    {
	    var shards = await GetShardsAsync(new(request.Class), cancellationToken).ConfigureAwait(false);
	    foreach (var shard in shards.Result!)
		    await UpdateShardAsync(new(request.Class, shard.Name, request.Status), cancellationToken).ConfigureAwait(false);
    }

    public void DeleteAllClasses()
    {
        var schema = GetSchema();
        foreach (var weaviateClass in schema.Result!.Classes)
            DeleteClass(new(weaviateClass.Class));
    }

    public async Task DeleteAllClassesAsync(CancellationToken cancellationToken = default)
    {
        var schema = await GetSchemaAsync(cancellationToken).ConfigureAwait(false);
        foreach (var weaviateClass in schema.Result!.Classes)
            await DeleteClassAsync(new(weaviateClass.Class), cancellationToken).ConfigureAwait(false);
    }

    private static ApiResponse<ShardStatus> GetResponse(ApiResponse<StatusContainer> response) =>
	    new()
	    {
		    Error = response.Error,
		    Uri = response.Uri,
		    HttpStatusCode = response.HttpStatusCode,
		    HttpMethod = response.HttpMethod,
		    RequestBody = response.RequestBody,
		    ResponseBody = response.ResponseBody,
		    Result = response.Result!.Status
	    };

    private class StatusContainer
    {
        public ShardStatus Status { get; set; }
    }
}

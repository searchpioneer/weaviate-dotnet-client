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
/// Batch operations
/// </summary>
public class BatchApi
{
    private readonly BeaconPath _beaconPath;
    private readonly Transport _transport;

    internal BatchApi(Transport transport, DbVersionSupport dbVersionSupport)
    {
        _transport = transport;
        _beaconPath = new(dbVersionSupport);
    }

    public ApiResponse<ObjectResponse[]> CreateObjects(CreateObjectsBatchRequest request)
    {
        var batchRequest = new ObjectsBatch(new[] { "ALL" }, request.Objects.ToArray());
        var endpoint = request.ConsistencyLevel.HasValue ? "/batch/objects?consistency_level=" + ConsistencyLevelJsonConverter.ToString(request.ConsistencyLevel.Value) :  "/batch/objects";
        var response = _transport.PostAsync<ObjectsBatch, ObjectResponse[]>(endpoint, batchRequest).GetAwaiter().GetResult();
        return response;
    }

    public async Task<ApiResponse<ObjectResponse[]>> CreateObjectsAsync(CreateObjectsBatchRequest request, CancellationToken cancellationToken = default)
    {
        var batchRequest = new ObjectsBatch(new[] { "ALL" }, request.Objects.ToArray());
        var endpoint = request.ConsistencyLevel.HasValue ? "/batch/objects?consistency_level=" + ConsistencyLevelJsonConverter.ToString(request.ConsistencyLevel.Value) :  "/batch/objects";
        var response = await _transport.PostAsync<ObjectsBatch, ObjectResponse[]>(endpoint, batchRequest, cancellationToken).ConfigureAwait(false);
        return response;
    }

    public ApiResponse<DeleteBatchResponse> DeleteObjects(DeleteObjectsBatchRequest request)
    {
        var match = request.Where != null || request.Class != null
            ? new BatchDeleteMatch(request.Class, request.Where)
            : null;
        var batchDelete = new BatchDelete(request.DryRun, request.Output, match);
        var endpoint = request.ConsistencyLevel.HasValue ? "/batch/objects?consistency_level=" + ConsistencyLevelJsonConverter.ToString(request.ConsistencyLevel.Value) :  "/batch/objects";
        var response = _transport.DeleteAsync<BatchDelete, DeleteBatchResponse>(endpoint, batchDelete).GetAwaiter().GetResult();
        return response;
    }

    public async Task<ApiResponse<DeleteBatchResponse>> DeleteObjectsAsync(DeleteObjectsBatchRequest request, CancellationToken cancellationToken = default)
    {
        var match = request.Where != null || request.Class != null
            ? new BatchDeleteMatch(request.Class, request.Where)
            : null;
        var batchDelete = new BatchDelete(request.DryRun, request.Output, match);
        var endpoint = request.ConsistencyLevel.HasValue ? "/batch/objects?consistency_level=" + ConsistencyLevelJsonConverter.ToString(request.ConsistencyLevel.Value) :  "/batch/objects";
        var response = await _transport.DeleteAsync<BatchDelete, DeleteBatchResponse>(endpoint, batchDelete, cancellationToken).ConfigureAwait(false);
        return response;
    }

    public ApiResponse<CreateReferencesBatchResponse[]> CreateReferences(CreateReferencesBatchRequest request)
    {
	    var endpoint = request.ConsistencyLevel.HasValue ? "/batch/references?consistency_level=" + ConsistencyLevelJsonConverter.ToString(request.ConsistencyLevel.Value) :  "/batch/references";
	    var response = _transport.PostAsync<BatchReference[], CreateReferencesBatchResponse[]>(endpoint, request.References).GetAwaiter().GetResult();
        return response;
    }

    public async Task<ApiResponse<CreateReferencesBatchResponse[]>> CreateReferencesAsync(CreateReferencesBatchRequest request, CancellationToken cancellationToken = default)
    {
	    var endpoint = request.ConsistencyLevel.HasValue ? "/batch/references?consistency_level=" + ConsistencyLevelJsonConverter.ToString(request.ConsistencyLevel.Value) :  "/batch/references";
	    var response = await _transport.PostAsync<BatchReference[], CreateReferencesBatchResponse[]>(endpoint, request.References, cancellationToken).ConfigureAwait(false);
        return response;
    }

    public BatchReference? Reference(string? fromClassName = null,
        string? toClassName = null,
        string? fromPropertyName = null,
        string? fromUuid = null,
        string? toUuid = null) =>
	    Reference(out _, _beaconPath, fromClassName, toClassName, fromPropertyName, fromUuid, toUuid);

    public static BatchReference? Reference(
        out List<string> warnings,
        BeaconPath? beaconPath = null,
        string? fromClassName = null,
        string? toClassName = null,
        string? fromPropertyName = null,
        string? fromUuid = null,
        string? toUuid = null)
    {
        var warns = new List<string>();
        if (string.IsNullOrWhiteSpace(fromClassName) ||
            string.IsNullOrWhiteSpace(fromPropertyName) ||
            string.IsNullOrWhiteSpace(fromUuid) ||
            string.IsNullOrWhiteSpace(toUuid))
        {
            warnings = warns;
            return null;
        }

        string from;
        string to;
        if (beaconPath != null)
        {
            from = beaconPath.BuildBatchFrom(new()
            {
                Id = fromUuid,
                Class = fromClassName,
                Property = fromPropertyName
            }, out var fromWarns);
            warns.AddRange(fromWarns);

            to = beaconPath.BuildBatchTo(new()
            {
                Id = toUuid,
                Class = toClassName
            }, out var toWarns);
            warns.AddRange(toWarns);
        }
        else
        {
            from = $"weaviate://localhost/{fromClassName}/{fromUuid}/{fromPropertyName}";
            to = $"weaviate://localhost/{toUuid}";
        }

        warnings = warns;
        return new(from, to);
    }

    private class ObjectsBatch
    {
        public ObjectsBatch(string[] fields, WeaviateObject[] objects)
        {
            Fields = fields;
            Objects = objects;
        }
        public string[] Fields { get; }
        public WeaviateObject[] Objects { get; }
    }

    private class BatchDelete
    {
        public BatchDelete(bool? dryRun, BatchOutput? output, BatchDeleteMatch? match)
        {
            DryRun = dryRun;
            Match = match;
            Output = output;
        }
        public bool? DryRun { get; }
        public BatchDeleteMatch? Match { get; }
        public BatchOutput? Output { get; }
    }

    private class BatchDeleteMatch
    {
        public BatchDeleteMatch(string? @class, Where? where)
        {
            Class = @class;
            Where = where;
        }
        public string? Class { get; }
        public Where? Where { get; }
    }
}

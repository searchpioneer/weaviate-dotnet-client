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

using SearchPioneer.Weaviate.Client.Api.Data.Model;
using SearchPioneer.Weaviate.Client.Api.Data.Request;
using SearchPioneer.Weaviate.Client.Api.Data.Response;
using SearchPioneer.Weaviate.Client.Paths;

namespace SearchPioneer.Weaviate.Client.Api.Data;

public class DataApi
{
    private readonly ObjectsPath _objectsPath;
    private readonly Transport _transport;

    internal DataApi(Transport transport, DbVersionSupport dbVersionSupport)
    {
        _transport = transport;
        _objectsPath = new(dbVersionSupport);
    }

    public ApiResponse<bool> Validate(ValidateObjectRequest request)
    {
        var obj = new WeaviateObject
        {
            Class = request.Class,
            Id = request.Id,
            Properties = request.Properties
        };

        var response = _transport.Post<WeaviateObject, object>("/objects/validate", obj);
        return response.MapToBool(response.HttpStatusCode == 200);
    }

    public ApiResponse<bool> Check(CheckObjectRequest request)
    {
        var path = _objectsPath.BuildCheck(new()
        {
            Id = request.Id,
            Class = request.Class
        }, out var warnings);

        var response = _transport.Head<object>(path);
        response.Warnings.AddRange(warnings);
        return response.MapToBool(response.HttpStatusCode == 204);
    }

    public ApiResponse<bool> Delete(DeleteObjectRequest request)
    {
        var path = _objectsPath.BuildDelete(new()
        {
            Id = request.Id,
            Class = request.Class
        }, out var warnings);
        var response = _transport.Delete(path);
        response.Warnings.AddRange(warnings);
        return response.MapToBool(response.HttpStatusCode == 204);
    }

    public ApiResponse<WeaviateObjectResponse> Update(UpdateObjectRequest request)
    {
        var param = new ObjectPathParams
        {
            Id = request.Id,
            Class = request.Class
        };

        var path = _objectsPath.BuildUpdate(param, out var warnings);

        var obj = new WeaviateObject
        {
            Id = request.Id,
            Class = request.Class,
            Properties = request.Properties
        };

        if (request.WithMerge != null && request.WithMerge.Value)
        {
            var patchResponse = _transport.Patch<WeaviateObject, WeaviateObjectResponse>(path, obj);
            patchResponse.Warnings.AddRange(warnings);
            return patchResponse;
        }

        var putResponse = _transport.Put<WeaviateObject, WeaviateObjectResponse>(path, obj);
        putResponse.Warnings.AddRange(warnings);
        return putResponse;
    }

    public ApiResponse<WeaviateObjectResponse> Create(CreateObjectRequest request)
    {
        var param = new ObjectPathParams();
        var path = _objectsPath.BuildCreate(param, out var warnings);
        var obj = new WeaviateObject
        {
            Id = string.IsNullOrEmpty(request.Id) // TODO! is this the best allocation algorithm?
                ? Guid.NewGuid().ToString()
                : request.Id,
            Class = request.Class,
            Properties = request.Properties,
            Vector = request.Vector
        };

        var response = _transport.Post<WeaviateObject, WeaviateObjectResponse>(path, obj);
        response.Warnings.AddRange(warnings);
        return response;
    }

    public ApiResponse<WeaviateObjectResponse[]> GetAll()
    {
        return Get(new());
    }

    public ApiResponse<WeaviateObjectResponse[]> Get(GetObjectRequest request)
    {
        var param = new ObjectPathParams
        {
            Id = request.Id,
            Class = request.Class,
            Limit = request.Limit,
            Additional = request.Additional != null
                ? request.Additional.ToArray()
                : Enumerable.Empty<string>().ToArray(),
            ConsistencyLevel = request.ConsistencyLevel,
            NodeName = request.NodeName
        };

        if (!string.IsNullOrWhiteSpace(request.Id))
        {
            var path = _objectsPath.BuildGetOne(param, out var getOneWarnings);
            var getOneResponse = _transport.Get<WeaviateObjectResponse>(path);
            var transformed = new ApiResponse<WeaviateObjectResponse[]>
            {
                Error = getOneResponse.Error,
                Uri = getOneResponse.Uri,
                HttpMethod = getOneResponse.HttpMethod,
                RequestBody = getOneResponse.RequestBody,
                ResponseBody = getOneResponse.ResponseBody,
                HttpStatusCode = getOneResponse.HttpStatusCode,
                Result = new[] { getOneResponse.Result }
            };
            transformed.Warnings.AddRange(getOneWarnings);
            return transformed;
        }

        var paths = _objectsPath.BuildGet(param, out var getResponseWarnings);
        var getListResponse = _transport.Get<ObjectsListResponse>(paths);
        var getApiResponse = new ApiResponse<WeaviateObjectResponse[]>
        {
            Error = getListResponse.Error,
            Uri = getListResponse.Uri,
            HttpMethod = getListResponse.HttpMethod,
            RequestBody = getListResponse.RequestBody,
            ResponseBody = getListResponse.ResponseBody,
            HttpStatusCode = getListResponse.HttpStatusCode,
            Result = getListResponse.Result
                .Objects // TODO! this is hiding information and we shouldnt just return this property
        };
        getApiResponse.Warnings.AddRange(getResponseWarnings);
        return getApiResponse;
    }
}
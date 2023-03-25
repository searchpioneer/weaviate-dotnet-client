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
/// Data API operations
/// </summary>
public class DataApi
{
    private readonly ObjectsPath _objectsPath;
    private readonly Transport _transport;

    internal DataApi(Transport transport, DbVersionSupport dbVersionSupport)
    {
        _transport = transport;
        _objectsPath = new(dbVersionSupport);
    }

    public ApiResponse Validate(ValidateObjectRequest request)
    {
        var weaviateObject = GetWeaviateObject(request);
        var response = _transport.PostAsync("/objects/validate", weaviateObject).GetAwaiter().GetResult();
        return response;
    }

    public async Task<ApiResponse> ValidateAsync(ValidateObjectRequest request, CancellationToken cancellationToken = default)
    {
	    var weaviateObject = GetWeaviateObject(request);
	    var response = await _transport.PostAsync("/objects/validate", weaviateObject, cancellationToken).ConfigureAwait(false);
        return response;
    }

    public ApiResponse Check(CheckObjectRequest request)
    {
        var path = GetCheckPath(request, out var warnings);
        var response = _transport.HeadAsync(path).GetAwaiter().GetResult();
        response.Warnings.AddRange(warnings);
        return response;
    }

    public async Task<ApiResponse> CheckAsync(CheckObjectRequest request, CancellationToken cancellationToken = default)
    {
        var path = GetCheckPath(request, out var warnings);
        var response = await _transport.HeadAsync(path, cancellationToken).ConfigureAwait(false);
        response.Warnings.AddRange(warnings);
        return response;
    }

    public ApiResponse Delete(DeleteObjectRequest request)
    {
        var path = GetDeletePath(request, out var warnings);
        var response = _transport.DeleteAsync(path).GetAwaiter().GetResult();
        response.Warnings.AddRange(warnings);
        return response;
    }

    public async Task<ApiResponse> DeleteSync(DeleteObjectRequest request, CancellationToken cancellationToken = default)
    {
        var path = GetDeletePath(request, out var warnings);
        var response = await _transport.DeleteAsync(path, cancellationToken).ConfigureAwait(false);
        response.Warnings.AddRange(warnings);
        return response;
    }

    public ApiResponse<WeaviateObjectResponse> Update(UpdateObjectRequest request)
    {
	    var path = GetUpdatePath(request, out var warnings);
        var weaviateObject = GetWeaviateObject(request);

        if (request.WithMerge != null && request.WithMerge.Value)
        {
            var patchResponse = _transport.PatchAsync<WeaviateObject, WeaviateObjectResponse>(path, weaviateObject).GetAwaiter().GetResult();
            patchResponse.Warnings.AddRange(warnings);
            return patchResponse;
        }

        var putResponse = _transport.PutAsync<WeaviateObject, WeaviateObjectResponse>(path, weaviateObject).GetAwaiter().GetResult();
        putResponse.Warnings.AddRange(warnings);
        return putResponse;
    }

    public async Task<ApiResponse<WeaviateObjectResponse>> UpdateAsync(UpdateObjectRequest request, CancellationToken cancellationToken = default)
    {
	    var path = GetUpdatePath(request, out var warnings);
        var weaviateObject = GetWeaviateObject(request);

        if (request.WithMerge != null && request.WithMerge.Value)
        {
            var patchResponse = await _transport.PatchAsync<WeaviateObject, WeaviateObjectResponse>(path, weaviateObject, cancellationToken).ConfigureAwait(false);
            patchResponse.Warnings.AddRange(warnings);
            return patchResponse;
        }

        var putResponse = await _transport.PutAsync<WeaviateObject, WeaviateObjectResponse>(path, weaviateObject, cancellationToken).ConfigureAwait(false);
        putResponse.Warnings.AddRange(warnings);
        return putResponse;
    }

    public ApiResponse<WeaviateObjectResponse> Create(CreateObjectRequest request)
    {
	    var path = _objectsPath.BuildCreate(new(), out var warnings);
        var weaviateObject = GetWeaviateObject(request);
        var response = _transport.PostAsync<WeaviateObject, WeaviateObjectResponse>(path, weaviateObject).GetAwaiter().GetResult();
        response.Warnings.AddRange(warnings);
        return response;
    }

    public async Task<ApiResponse<WeaviateObjectResponse>> CreateAsync(CreateObjectRequest request, CancellationToken cancellationToken = default)
    {
	    var path = _objectsPath.BuildCreate(new(), out var warnings);
        var weaviateObject = GetWeaviateObject(request);
        var response = await _transport.PostAsync<WeaviateObject, WeaviateObjectResponse>(path, weaviateObject, cancellationToken).ConfigureAwait(false);
        response.Warnings.AddRange(warnings);
        return response;
    }

    public ApiResponse<WeaviateObjectResponse[]> GetAll() => Get(new());

    public async Task<ApiResponse<WeaviateObjectResponse[]>> GetAllAsync(CancellationToken cancellationToken = default) =>
	    await GetAsync(new(), cancellationToken).ConfigureAwait(false);

    public ApiResponse<WeaviateObjectResponse[]> Get(GetObjectRequest request)
    {
        var param = GetObjectPathParams(request);

        if (!string.IsNullOrWhiteSpace(request.Id))
        {
            var path = _objectsPath.BuildGetOne(param, out var getOneWarnings);
            var getOneResponse = _transport.GetAsync<WeaviateObjectResponse>(path).GetAwaiter().GetResult();
            var transformed = new ApiResponse<WeaviateObjectResponse[]>
            {
                Error = getOneResponse.Error,
                Uri = getOneResponse.Uri,
                HttpMethod = getOneResponse.HttpMethod,
                RequestBody = getOneResponse.RequestBody,
                ResponseBody = getOneResponse.ResponseBody,
                HttpStatusCode = getOneResponse.HttpStatusCode,
                Result = getOneResponse.Result == null ? null : new[] { getOneResponse.Result }
            };
            transformed.Warnings.AddRange(getOneWarnings);
            return transformed;
        }

        var paths = _objectsPath.BuildGet(param, out var getResponseWarnings);
        var getListResponse = _transport.GetAsync<ObjectsListResponse>(paths).GetAwaiter().GetResult();
        var getApiResponse = new ApiResponse<WeaviateObjectResponse[]>
        {
            Error = getListResponse.Error,
            Uri = getListResponse.Uri,
            HttpMethod = getListResponse.HttpMethod,
            RequestBody = getListResponse.RequestBody,
            ResponseBody = getListResponse.ResponseBody,
            HttpStatusCode = getListResponse.HttpStatusCode,
            Result = getListResponse.Result?
                .Objects // TODO! this is hiding information and shouldn't we just return this property?
        };
        getApiResponse.Warnings.AddRange(getResponseWarnings);
        return getApiResponse;
    }

    public async Task<ApiResponse<WeaviateObjectResponse[]>> GetAsync(GetObjectRequest request, CancellationToken cancellationToken = default)
    {
        var param = GetObjectPathParams(request);

        if (!string.IsNullOrWhiteSpace(request.Id))
        {
            var path = _objectsPath.BuildGetOne(param, out var getOneWarnings);
            var getOneResponse = await _transport.GetAsync<WeaviateObjectResponse>(path, cancellationToken).ConfigureAwait(false);
            var transformed = new ApiResponse<WeaviateObjectResponse[]>
            {
                Error = getOneResponse.Error,
                Uri = getOneResponse.Uri,
                HttpMethod = getOneResponse.HttpMethod,
                RequestBody = getOneResponse.RequestBody,
                ResponseBody = getOneResponse.ResponseBody,
                HttpStatusCode = getOneResponse.HttpStatusCode,
                Result = getOneResponse.Result == null ? null : new[] { getOneResponse.Result }
            };
            transformed.Warnings.AddRange(getOneWarnings);
            return transformed;
        }

        var paths = _objectsPath.BuildGet(param, out var getResponseWarnings);
        var getListResponse = await _transport.GetAsync<ObjectsListResponse>(paths, cancellationToken).ConfigureAwait(false);
        var getApiResponse = new ApiResponse<WeaviateObjectResponse[]>
        {
            Error = getListResponse.Error,
            Uri = getListResponse.Uri,
            HttpMethod = getListResponse.HttpMethod,
            RequestBody = getListResponse.RequestBody,
            ResponseBody = getListResponse.ResponseBody,
            HttpStatusCode = getListResponse.HttpStatusCode,
            Result = getListResponse.Result?
                .Objects // TODO! this is hiding information and shouldn't we just return this property?
        };
        getApiResponse.Warnings.AddRange(getResponseWarnings);
        return getApiResponse;
    }

    private static ObjectPathParams GetObjectPathParams(GetObjectRequest request)
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
	    return param;
    }

    private static WeaviateObject GetWeaviateObject(ValidateObjectRequest request)
    {
	    var weaviateObject = new WeaviateObject
	    {
		    Class = request.Class,
		    Id = request.Id,
		    Properties = request.Properties
	    };
	    return weaviateObject;
    }

    private static WeaviateObject GetWeaviateObject(UpdateObjectRequest request)
    {
	    var weaviateObject = new WeaviateObject
	    {
		    Id = request.Id,
		    Class = request.Class,
		    Properties = request.Properties
	    };
	    return weaviateObject;
    }

    private static WeaviateObject GetWeaviateObject(CreateObjectRequest request)
    {
	    var obj = new WeaviateObject
	    {
		    Id = string.IsNullOrEmpty(request.Id) // TODO! is this the best allocation algorithm?
			    ? Guid.NewGuid().ToString()
			    : request.Id,
		    Class = request.Class,
		    Properties = request.Properties,
		    Vector = request.Vector
	    };
	    return obj;
    }

    private string GetCheckPath(CheckObjectRequest request, out List<string> warnings)
    {
	    var path = _objectsPath.BuildCheck(new()
	    {
		    Id = request.Id,
		    Class = request.Class
	    }, out warnings);
	    return path;
    }

    private string GetDeletePath(DeleteObjectRequest request, out List<string> warnings)
    {
	    var path = _objectsPath.BuildDelete(new()
	    {
		    Id = request.Id,
		    Class = request.Class
	    }, out warnings);
	    return path;
    }

    private string GetUpdatePath(UpdateObjectRequest request, out List<string> warnings)
    {
	    var path = _objectsPath.BuildUpdate(new()
	    {
		    Id = request.Id,
		    Class = request.Class
	    }, out warnings);
	    return path;
    }
}

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
/// Reference operations
/// </summary>
public class ReferenceApi
{
    private readonly BeaconPath _beaconPath;
    private readonly ReferencesPath _referencesPath;
    private readonly Transport _transport;

    internal ReferenceApi(Transport transport, DbVersionSupport dbVersionSupport)
    {
        _transport = transport;
        _referencesPath = new(dbVersionSupport);
        _beaconPath = new(dbVersionSupport);
    }

    public ApiResponse Replace(ReplaceReferenceRequest request)
    {
        var pathParams = GetReferencePathParams(request);
        var path = _referencesPath.Build(pathParams, out var warnings);
        var response = _transport.PutAsync(path, request.ReferencePayload).GetAwaiter().GetResult();
        response.Warnings.AddRange(warnings);
        return response;
    }

    public async Task<ApiResponse> ReplaceAsync(ReplaceReferenceRequest request, CancellationToken cancellationToken = default)
    {
        var pathParams = GetReferencePathParams(request);
        var path = _referencesPath.Build(pathParams, out var warnings);
        var response = await _transport.PutAsync(path, request.ReferencePayload, cancellationToken).ConfigureAwait(false);
        response.Warnings.AddRange(warnings);
        return response;
    }

    public ApiResponse Create(CreateReferenceRequest request)
    {
        var pathParams = GetReferencePathParams(request);
        var path = _referencesPath.Build(pathParams, out var warnings);
        var response = _transport.PostAsync(path, request.ReferencePayload).GetAwaiter().GetResult();
        response.Warnings.AddRange(warnings);
        return response;
    }

    public async Task<ApiResponse> CreateAsync(CreateReferenceRequest request, CancellationToken cancellationToken = default)
    {
        var pathParams = GetReferencePathParams(request);
        var path = _referencesPath.Build(pathParams, out var warnings);
        var response = await _transport.PostAsync(path, request.ReferencePayload, cancellationToken).ConfigureAwait(false);
        response.Warnings.AddRange(warnings);
        return response;
    }

    public ApiResponse Delete(DeleteReferenceRequest request)
    {
        var pathParams = GetReferencePathParams(request);
        var path = _referencesPath.Build(pathParams, out var warnings);
        var response = _transport.DeleteAsync(path, request.ReferencePayload).GetAwaiter().GetResult();
        response.Warnings.AddRange(warnings);
        return response;
    }

    public async Task<ApiResponse> DeleteAsync(DeleteReferenceRequest request, CancellationToken cancellationToken = default)
    {
        var pathParams = GetReferencePathParams(request);
        var path = _referencesPath.Build(pathParams, out var warnings);
        var response = await _transport.DeleteAsync(path, request.ReferencePayload, cancellationToken).ConfigureAwait(false);
        response.Warnings.AddRange(warnings);
        return response;
    }

    private static ReferencePathParams GetReferencePathParams(CreateReferenceRequest request)
    {
	    var pathParams = new ReferencePathParams
	    {
		    Id = request.Id,
		    Class = request.Class,
		    Property = request.ReferenceProperty
	    };
	    return pathParams;
    }

    private static ReferencePathParams GetReferencePathParams(ReplaceReferenceRequest request)
    {
	    // TODO should reference payload be nullable?
	    var pathParams = new ReferencePathParams
	    {
		    Id = request.Id,
		    Class = request.Class,
		    Property = request.ReferenceProperty
	    };
	    return pathParams;
    }

    private static ReferencePathParams GetReferencePathParams(DeleteReferenceRequest request)
    {
	    var pathParams = new ReferencePathParams
	    {
		    Id = request.Id,
		    Class = request.Class,
		    Property = request.ReferenceProperty
	    };
	    return pathParams;
    }

    public SingleRef Reference(string? @class = null, string? id = null) => Reference(out _, _beaconPath, @class, id);

    public static SingleRef Reference(out List<string> warnings,
        BeaconPath? beaconPath = null,
        string? @class = null,
        string? id = null
    )
    {
        var warns = new List<string>();
        string beacon;
        if (beaconPath != null)
        {
            beacon = beaconPath.BuildSingle(
                new()
                {
                    Id = id,
                    Class = @class
                }, out var beaconWarns);
            warns.AddRange(beaconWarns);
        }
        else
        {
            // TODO! should this warn?
            beacon = $"weaviate://localhost/{id}";
        }

        warnings = warns;
        return new()
        {
            Beacon = beacon
        };
    }
}

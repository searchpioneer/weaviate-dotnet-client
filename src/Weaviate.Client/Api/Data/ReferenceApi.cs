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
using SearchPioneer.Weaviate.Client.Paths;

namespace SearchPioneer.Weaviate.Client.Api.Data;

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

    public ApiResponse<bool> Replace(ReplaceReferenceRequest request)
    {
        // TODO should reference payload be nullable?
        var pathParams = new ReferencePathParams
        {
            Id = request.Id,
            Class = request.Class,
            Property = request.ReferenceProperty
        };
        var path = _referencesPath.Build(pathParams, out var warnings);
        var response = _transport.Put<SingleRef[]?, object>(path, request.ReferencePayload);
        response.Warnings.AddRange(warnings);
        return response.MapToBool(response.HttpStatusCode == 200);
    }

    public ApiResponse<bool> Create(CreateReferenceRequest request)
    {
        var pathParams = new ReferencePathParams
        {
            Id = request.Id,
            Class = request.Class,
            Property = request.ReferenceProperty
        };
        var path = _referencesPath.Build(pathParams, out var warnings);
        var response = _transport.Post<SingleRef?, object>(path, request.ReferencePayload);
        response.Warnings.AddRange(warnings);
        return response.MapToBool(response.HttpStatusCode == 200);
    }

    public ApiResponse<bool> Delete(DeleteReferenceRequest request)
    {
        var pathParams = new ReferencePathParams
        {
            Id = request.Id,
            Class = request.Class,
            Property = request.ReferenceProperty
        };

        var path = _referencesPath.Build(pathParams, out var warnings);
        var response = _transport.Delete(path, request.ReferencePayload);
        response.Warnings.AddRange(warnings);
        return response.MapToBool(response.HttpStatusCode == 204);
    }

    public SingleRef Reference(string? @class = null, string? id = null)
    {
        return Reference(out _, _beaconPath, @class, id);
    }

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
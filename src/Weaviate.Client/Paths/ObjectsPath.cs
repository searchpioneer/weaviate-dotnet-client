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

using System.Text;
using SearchPioneer.Weaviate.Client.JsonConvertors;

namespace SearchPioneer.Weaviate.Client.Paths;

public class ObjectsPath
{
    private readonly DbVersionSupport _support;

    public ObjectsPath(DbVersionSupport support)
    {
        _support = support;
    }

    public string BuildCreate(ObjectPathParams pathParams, out List<string> warnings)
    {
        var result = Build(pathParams, out var warns);
        warnings = warns;
        return result;
    }

    public string BuildDelete(ObjectPathParams pathParams, out List<string> warnings)
    {
        var result = Build(pathParams, out var warns, AddClassNameDeprecatedNotSupportedCheck, AddId);
        warnings = warns;
        return result;
    }

    public string BuildCheck(ObjectPathParams pathParams, out List<string> warnings)
    {
        var result = Build(pathParams, out var warns, AddClassNameDeprecatedNotSupportedCheck, AddId);
        warnings = warns;
        return result;
    }

    public string BuildGetOne(ObjectPathParams pathParams, out List<string> warnings)
    {
        var result = Build(pathParams, out var warns, AddClassNameDeprecatedNotSupportedCheck, AddId,
            AddQueryParamsForGetOne);
        warnings = warns;
        return result;
    }

    public string BuildGet(ObjectPathParams pathParams, out List<string> warnings)
    {
        var result = Build(pathParams, out var warns, AddQueryParams);
        warnings = warns;
        return result;
    }

    public string BuildUpdate(ObjectPathParams pathParams, out List<string> warnings)
    {
        var result = Build(pathParams, out var warns, AddClassNameDeprecatedCheck, AddId);
        warnings = warns;
        return result;
    }

    private static string Build(ObjectPathParams pathParams, out List<string> warnings,
        params Func<StringBuilder, ObjectPathParams, List<string>?>[] modifiers)
    {
        var warns = new List<string>();
        var path = new StringBuilder("/objects");
        foreach (var modifier in modifiers)
        {
            var result = modifier(path, pathParams);
            if (result != null) warns.AddRange(result);
        }

        warnings = warns;
        return path.ToString();
    }

    private List<string>? AddClassNameDeprecatedNotSupportedCheck(StringBuilder path, ObjectPathParams pathParams)
    {
        var warnings = new List<string>();
        if (_support.SupportsClassNameNamespacedEndpoints())
        {
            if (!string.IsNullOrWhiteSpace(pathParams.Class))
                path.Append('/').Append(pathParams.Class.Trim());
            else
                warnings.Add(_support.GetWarnDeprecatedNonClassNameNamespacedEndpointsForObjects());
        }
        else if (!string.IsNullOrWhiteSpace(pathParams.Class))
        {
            warnings.Add(_support.GetWarnNotSupportedClassNamespacedEndpointsForObjects());
        }

        return warnings;
    }

    private List<string>? AddClassNameDeprecatedCheck(StringBuilder path, ObjectPathParams pathParams)
    {
        var warnings = new List<string>();
        if (_support.SupportsClassNameNamespacedEndpoints())
        {
            if (!string.IsNullOrWhiteSpace(pathParams.Class))
                path.Append('/').Append(pathParams.Class.Trim());
            else
                warnings.Add(_support.GetWarnDeprecatedNonClassNameNamespacedEndpointsForObjects());
        }

        return warnings;
    }

    private static List<string>? AddId(StringBuilder path, ObjectPathParams pathParams)
    {
        if (!string.IsNullOrWhiteSpace(pathParams.Id)) path.Append('/').Append(pathParams.Id.Trim());
        return null;
    }

    private List<string>? AddQueryParams(StringBuilder path, ObjectPathParams pathParams)
    {
        var warnings = new List<string>();
        var queryParams = new List<string>();
        if (pathParams.Additional is { Length: > 0 })
            queryParams.Add($"include={string.Join(",", pathParams.Additional)}");

        if (pathParams.Limit != null) queryParams.Add($"limit={pathParams.Limit.Value}");

        if (string.IsNullOrWhiteSpace(pathParams.Id) && !string.IsNullOrWhiteSpace(pathParams.Class))
        {
            if (_support.SupportsClassNameNamespacedEndpoints())
                queryParams.Add($"class={pathParams.Class}");
            else
                warnings.Add(_support.GetWarnNotSupportedClassParameterInEndpointsForObjects());
        }

        if (queryParams.Count > 0) path.Append('?').Append(string.Join("&", queryParams.ToArray()));
        return warnings;
    }

    private static List<string>? AddQueryParamsForGetOne(StringBuilder path, ObjectPathParams pathParams)
    {
        var queryParams = new List<string>();
        if (pathParams.Additional is { Length: > 0 })
            queryParams.Add($"include={string.Join(",", pathParams.Additional)}");
        if (pathParams.ConsistencyLevel != null)
            queryParams.Add(
                $"consistency_level={ConsistencyLevelJsonConverter.ToString(pathParams.ConsistencyLevel.Value)}");
        if (!string.IsNullOrEmpty(pathParams.NodeName)) queryParams.Add($"node_name={pathParams.NodeName}");

        if (queryParams.Count > 0) path.Append('?').Append(string.Join("&", queryParams.ToArray()));
        return null;
    }
}
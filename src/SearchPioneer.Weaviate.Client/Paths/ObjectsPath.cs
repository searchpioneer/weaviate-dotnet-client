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

public class ObjectsPath
{
    private readonly DbVersionSupport _support;

    public ObjectsPath(DbVersionSupport support) => _support = support;

    public string BuildCreate(ObjectPathParams pathParams, out List<string> warnings)
    {
        var result = Build(pathParams, out var warns,
	        AddQueryConsistencyLevel);
        warnings = warns;
        return result;
    }

    public string BuildDelete(ObjectPathParams pathParams, out List<string> warnings)
    {
	    var result = Build(pathParams, out var warns,
		    AddPathClassNameWithDeprecatedNotSupportedCheck,
		    AddPathId,
		    AddQueryConsistencyLevel);
        warnings = warns;
        return result;
    }

    public string BuildUpdate(ObjectPathParams pathParams, out List<string> warnings)
    {
	    var result = Build(pathParams, out var warns,
		    AddPathClassNameWithDeprecatedCheck,
		    AddPathId,
		    AddQueryConsistencyLevel);
	    warnings = warns;
	    return result;
    }

    public string BuildCheck(ObjectPathParams pathParams, out List<string> warnings)
    {
	    var result = Build(pathParams, out var warns,
		    AddPathClassNameWithDeprecatedNotSupportedCheck,
		    AddPathId);
        warnings = warns;
        return result;
    }

    public string BuildGet(ObjectPathParams pathParams, out List<string> warnings)
    {
	    var result = Build(pathParams, out var warns,
		    AddQueryClassNameWithDeprecatedCheck,
		    AddQueryAdditionals,
		    AddQueryLimit);
	    warnings = warns;
	    return result;
    }

    public string BuildGetOne(ObjectPathParams pathParams, out List<string> warnings)
    {
        var result = Build(pathParams, out var warns,
	        AddPathClassNameWithDeprecatedNotSupportedCheck,
	        AddPathId,
	        AddQueryAdditionals,
	        AddQueryConsistencyLevel,
	        AddQueryNodeName);
        warnings = warns;
        return result;
    }

    private static string Build(ObjectPathParams pathParams, out List<string> warnings,
        params Func<List<string>, List<string>, ObjectPathParams, List<string>?>[] modifiers)
    {
	    if (pathParams == null)
		    throw new ArgumentNullException(nameof(pathParams));

        var warns = new List<string>();
        var path = new List<string>();
        var query = new List<string>();
        foreach (var modifier in modifiers)
        {
            var result = modifier(path, query, pathParams);
            if (result != null) warns.AddRange(result);
        }

        warnings = warns;
        var url = path.Count > 0 ? string.Join("/", path) : string.Empty;
		var queryString = query.Count > 0 ? $"?{string.Join("&", query)}" : string.Empty;
		if (string.IsNullOrEmpty(url) && string.IsNullOrEmpty(queryString))
			return "/objects";
		if (string.IsNullOrEmpty(url))
			return $"/objects{queryString}";
		return $"/objects/{url}{queryString}";
    }

    private List<string>? AddPathClassNameWithDeprecatedNotSupportedCheck(List<string> path, List<string> query, ObjectPathParams pathParams)
    {
        var warnings = new List<string>();
        if (_support.SupportsClassNameNamespacedEndpoints())
        {
            if (!string.IsNullOrWhiteSpace(pathParams.Class))
                path.Add(pathParams.Class.Trim());
            else
                warnings.Add(_support.GetWarnDeprecatedNonClassNameNamespacedEndpointsForObjects());
        }
        else if (!string.IsNullOrWhiteSpace(pathParams.Class))
        {
            warnings.Add(_support.GetWarnNotSupportedClassNamespacedEndpointsForObjects());
        }

        return warnings;
    }

    private List<string>? AddPathClassNameWithDeprecatedCheck(List<string> path, List<string> query, ObjectPathParams pathParams)
    {
        var warnings = new List<string>();
        if (_support.SupportsClassNameNamespacedEndpoints())
        {
            if (!string.IsNullOrWhiteSpace(pathParams.Class))
                path.Add(pathParams.Class.Trim());
            else
                warnings.Add(_support.GetWarnDeprecatedNonClassNameNamespacedEndpointsForObjects());
        }
        return warnings;
    }

    private static List<string>? AddPathId(List<string> path, List<string> query, ObjectPathParams pathParams)
    {
	    if (!string.IsNullOrWhiteSpace(pathParams.Id))
		    path.Add(pathParams.Id.Trim());
	    return null;
    }

    private List<string>? AddQueryClassNameWithDeprecatedCheck(List<string> path, List<string> query, ObjectPathParams pathParams)
    {
	    var warnings = new List<string>();
	    if (string.IsNullOrWhiteSpace(pathParams.Id) && !string.IsNullOrWhiteSpace(pathParams.Class))
	    {
		    if (_support.SupportsClassNameNamespacedEndpoints())
			    query.Add($"class={pathParams.Class}");
		    else
			    warnings.Add(_support.GetWarnNotSupportedClassParameterInEndpointsForObjects());
	    }
	    return warnings;
    }

    private List<string>? AddQueryAdditionals(List<string> path, List<string> query, ObjectPathParams pathParams)
    {
	    var warnings = new List<string>();
	    if (pathParams.Additional is { Length: > 0 })
		    query.Add($"include={string.Join(",", pathParams.Additional)}");
	    return warnings;
    }

    private List<string>? AddQueryLimit(List<string> path, List<string> query, ObjectPathParams pathParams)
    {
	    if (pathParams.Limit != null) query.Add($"limit={pathParams.Limit.Value}");
	    return null;
    }

    private static List<string>? AddQueryConsistencyLevel(List<string> path, List<string> query, ObjectPathParams pathParams)
    {
	    if (pathParams.ConsistencyLevel != null)
		    query.Add($"consistency_level={ConsistencyLevelJsonConverter.ToString(pathParams.ConsistencyLevel.Value)}");
        return null;
    }

    private static List<string>? AddQueryNodeName(List<string> path, List<string> query, ObjectPathParams pathParams)
    {
	    if (!string.IsNullOrEmpty(pathParams.NodeName))
		    query.Add($"node_name={pathParams.NodeName}");

        return null;
    }
}

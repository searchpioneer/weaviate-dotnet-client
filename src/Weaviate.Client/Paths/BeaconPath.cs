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

namespace SearchPioneer.Weaviate.Client.Paths;

public class BeaconPath
{
    private readonly DbVersionSupport _support;

    internal BeaconPath()
    {
    }

    public BeaconPath(DbVersionSupport support)
    {
        _support = support;
    }

    public virtual string BuildBatchFrom(BeaconPathParams pathParams, out List<string> warnings)
    {
        return Build(pathParams, out warnings, AddClassName, AddId, AddProperty);
    }

    public virtual string BuildBatchTo(BeaconPathParams pathParams, out List<string> warnings)
    {
        return Build(pathParams, out warnings, AddClassNameDeprecatedNotSupportedCheck, AddId);
    }

    public virtual string BuildSingle(BeaconPathParams pathParams, out List<string> warnings)
    {
        return Build(pathParams, out warnings, AddClassNameDeprecatedNotSupportedCheck, AddId);
    }

    private static string Build(BeaconPathParams pathParams, out List<string> warnings,
        params Func<StringBuilder, BeaconPathParams, List<string>?>[] modifiers)
    {
        var warns = new List<string>();
        var path = new StringBuilder("weaviate://localhost");
        foreach (var modifier in modifiers)
        {
            var result = modifier(path, pathParams);
            if (result != null) warns.AddRange(result);
        }

        warnings = warns;
        return path.ToString();
    }

    private List<string>? AddClassNameDeprecatedNotSupportedCheck(StringBuilder path, BeaconPathParams pathParams)
    {
        var warns = new List<string>();
        if (_support.SupportsClassNameNamespacedEndpoints())
        {
            if (!string.IsNullOrWhiteSpace(pathParams.Class.Trim()))
                path.Append('/').Append(pathParams.Class.Trim());
            else
                warns.Add(_support.GetWarnDeprecatedNonClassNameNamespacedEndpointsForBeacons());
        }
        else if (!string.IsNullOrWhiteSpace(pathParams.Class))
        {
            warns.Add(_support.GetWarnNotSupportedClassNamespacedEndpointsForBeacons());
        }

        return warns;
    }

    private static List<string>? AddClassName(StringBuilder path, BeaconPathParams pathParams)
    {
        if (!string.IsNullOrWhiteSpace(pathParams.Class)) path.Append('/').Append(pathParams.Class.Trim());
        return null;
    }

    private static List<string>? AddId(StringBuilder path, BeaconPathParams pathParams)
    {
        if (!string.IsNullOrWhiteSpace(pathParams.Id)) path.Append('/').Append(pathParams.Id.Trim());
        return null;
    }

    private static List<string>? AddProperty(StringBuilder path, BeaconPathParams pathParams)
    {
        if (!string.IsNullOrWhiteSpace(pathParams.Property)) path.Append('/').Append(pathParams.Property.Trim());
        return null;
    }
}
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

public class ReferencesPath
{
    private readonly DbVersionSupport _support;

    public ReferencesPath(DbVersionSupport support)
    {
        _support = support;
    }

    public string Build(ReferencePathParams? pathParams, out List<string> warnings)
    {
        var warns = new List<string>();
        const string value = "/objects";
        if (pathParams == null)
        {
            warnings = warns;
            return value;
        }

        var path = new StringBuilder(value);
        if (_support.SupportsClassNameNamespacedEndpoints())
        {
            if (!string.IsNullOrWhiteSpace(pathParams.Class))
                path.Append('/').Append(pathParams.Class.Trim());
            else
                warns.Add(_support.GetWarnDeprecatedNonClassNameNamespacedEndpointsForReferences());
        }
        else if (!string.IsNullOrWhiteSpace(pathParams.Class))
        {
            warns.Add(_support.GetWarnNotSupportedClassNamespacedEndpointsForReferences());
        }

        if (!string.IsNullOrWhiteSpace(pathParams.Id)) path.Append('/').Append(pathParams.Id.Trim());

        path.Append("/references");
        if (!string.IsNullOrWhiteSpace(pathParams.Property)) path.Append('/').Append(pathParams.Property.Trim());

        warnings = warns;
        return path.ToString();
    }
}
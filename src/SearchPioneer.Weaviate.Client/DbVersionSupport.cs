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

public class DbVersionSupport
{
    private readonly string _version;

    internal DbVersionSupport(){}

    internal DbVersionSupport(string version) => _version = version;

    public virtual bool SupportsClassNameNamespacedEndpoints()
    {
        // supported since 1.14
        var versionNumbers = _version.Split('.');
        if (versionNumbers.Length < 2) return false;

        var major = int.Parse(versionNumbers[0]);
        var minor = int.Parse(versionNumbers[1]);
        return (major == 1 && minor >= 14) || major >= 2;
    }

    public string GetWarnDeprecatedNonClassNameNamespacedEndpointsForObjects() =>
	    $"WARNING: Usage of objects paths without class is deprecated in Weaviate {_version}." +
	    " Please provide class parameter\n";

    public string GetWarnDeprecatedNonClassNameNamespacedEndpointsForReferences() =>
	    $"WARNING: Usage of references paths without class is deprecated in Weaviate {_version}." +
	    " Please provide class parameter\n";

    public string GetWarnDeprecatedNonClassNameNamespacedEndpointsForBeacons() =>
	    $"WARNING: Usage of beacon paths without class is deprecated in Weaviate {_version}." +
	    " Please provide class parameter\n";

    public string GetWarnNotSupportedClassNamespacedEndpointsForObjects() =>
	    $"WARNING: Usage of objects paths with class is not supported in Weaviate {_version}." +
	    " class parameter is ignored\n";

    public string GetWarnNotSupportedClassParameterInEndpointsForObjects() =>
	    $"WARNING: Usage of objects paths with class query parameter is not supported in Weaviate {_version}." +
	    " class query parameter is ignored\n";

    public string GetWarnNotSupportedClassNamespacedEndpointsForReferences() =>
	    $"WARNING: Usage of references paths with class is not supported in Weaviate {_version}." +
	    " class parameter is ignored\n";

    public string GetWarnNotSupportedClassNamespacedEndpointsForBeacons() =>
	    $"WARNING: Usage of beacons paths with class is not supported in Weaviate {_version}." +
	    " class parameter is ignored\n";
}

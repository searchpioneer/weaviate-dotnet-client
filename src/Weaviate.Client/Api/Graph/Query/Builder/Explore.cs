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

using SearchPioneer.Weaviate.Client.Api.Graph.Model;
using SearchPioneer.Weaviate.Client.Api.Graph.Query.Arguments;

namespace SearchPioneer.Weaviate.Client.Api.Graph.Query.Builder;

public class Explore
{
    public ExploreFields[]? Fields { get; set; }
    public int? Limit { get; set; }
    public int? Offset { get; set; }
    public Ask? Ask { get; set; }
    public NearImage? NearImage { get; set; }
    public NearObject? NearObject { get; set; }
    public NearText? NearText { get; set; }
    public NearVector? NearVector { get; set; }

    public override string ToString()
    {
        var fieldsClause = "";
        if (Fields is { Length: > 0 })
            // TODO: lots of enums to lowering, write an extension method that does proper camelCasing and replace all
            fieldsClause = string.Join(",", Fields.Select(f =>
            {
                var name = f.ToString("f");
                return char.ToLowerInvariant(name[0]) + name[1..];
            }));

        return $"{{Explore({CreateFilterClause()}){{{fieldsClause}}}}}";
    }

    private string CreateFilterClause()
    {
        var filters = new HashSet<string>();
        if (NearText != null) filters.Add(NearText.ToString());
        if (NearObject != null) filters.Add(NearObject.ToString());
        if (NearVector != null) filters.Add(NearVector.ToString());
        if (Ask != null) filters.Add(Ask.ToString());
        if (NearImage != null) filters.Add(NearImage.ToString());
        if (Limit != null) filters.Add($"limit:{Limit}");
        if (Offset != null) filters.Add($"offset:{Offset}");
        var s = string.Join(",", filters.ToArray());
        return $"{s}";
    }
}
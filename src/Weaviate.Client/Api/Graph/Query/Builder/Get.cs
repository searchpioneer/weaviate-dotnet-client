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

using SearchPioneer.Weaviate.Client.Api.Filters;
using SearchPioneer.Weaviate.Client.Api.Graph.Query.Arguments;
using SearchPioneer.Weaviate.Client.Api.Graph.Query.Fields;

namespace SearchPioneer.Weaviate.Client.Api.Graph.Query.Builder;

public class Get
{
    public string? Class { get; set; }
    public Field[]? Fields { get; set; }
    public int? Offset { get; set; }
    public int? Limit { get; set; }
    public Where? Where { get; set; }
    public NearText? NearText { get; set; }
    public BM25? BM25 { get; set; }
    public Hybrid? Hybrid { get; set; }
    public NearObject? NearObject { get; set; }
    public Ask? Ask { get; set; }
    public NearImage? NearImage { get; set; }
    public NearVector? NearVector { get; set; }
    public Group? Group { get; set; }
    public Sort[]? Sorts { get; set; }

    public override string ToString()
    {
        return $"{{Get{{{Class}{CreateFilterClause()}{{{CreateFields()}}}}}}}";
    }

    private string CreateFields()
    {
        return Fields is { Length: > 0 }
            ? string.Join(" ", Fields.Select(f => f.ToString()).ToArray())
            : string.Empty;
    }

    private string CreateFilterClause()
    {
        if (Where == null && NearText == null && BM25 == null && Hybrid == null && NearObject == null
            && NearVector == null && NearImage == null && Group == null
            && Ask == null && Limit == null && Offset == null && Sorts == null)
            return string.Empty;

        var filters = new HashSet<string>();
        if (Where != null) filters.Add(Where.BuildWithWhere());
        if (NearText != null) filters.Add(NearText.ToString());
        if (BM25 != null) filters.Add(BM25.ToString());
        if (Hybrid != null) filters.Add(Hybrid.ToString());
        if (NearObject != null) filters.Add(NearObject.ToString());
        if (NearVector != null) filters.Add(NearVector.ToString());
        if (Group != null) filters.Add(Group.ToString());
        if (Ask != null) filters.Add(Ask.ToString());
        if (NearImage != null) filters.Add(NearImage.ToString());
        if (Limit != null) filters.Add($"limit:{Limit}");
        if (Offset != null) filters.Add($"offset:{Offset}");
        if (Sorts != null)
        {
            if (Sorts is { Length: > 0 })
            {
                var args = Sorts.Select(s => s.ToString()).ToArray();
                var sorts = string.Join(",", args);
                filters.Add($"sort:[{sorts}]");
            }
            else
            {
                filters.Add("sort:[]");
            }
        }

        var s = string.Join(",", filters.ToArray());
        return $"({s})";
    }
}
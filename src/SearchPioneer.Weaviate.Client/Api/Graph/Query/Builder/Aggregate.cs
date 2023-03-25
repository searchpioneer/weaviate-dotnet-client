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

public class Aggregate
{
    public string? Class { get; set; }
    public Field[]? Fields { get; set; }
    public string? GroupBy { get; set; }
    public Where? Where { get; set; }
    public NearText? NearText { get; set; }
    public NearObject? NearObject { get; set; }
    public NearVector? NearVector { get; set; }
    public Ask? Ask { get; set; }
    public NearImage? NearImage { get; set; }
    public int? ObjectLimit { get; set; }
    public int? Limit { get; set; }

    public override string ToString() => $"{{Aggregate{{{Class}{CreateFilterClause()}{{{CreateFields()}}}}}}}";

    private string CreateFields() =>
	    Fields is { Length: > 0 }
		    ? string.Join(" ", Fields.Select(f => f.ToString()).ToArray())
		    : string.Empty;

    private string CreateFilterClause()
    {
        if (Where == null && NearText == null && NearObject == null
            && NearVector == null && ObjectLimit == null && Ask == null
            && NearImage == null && Limit == null && string.IsNullOrEmpty(GroupBy))
            return string.Empty;

        var filters = new HashSet<string>();
        if (!string.IsNullOrEmpty(GroupBy)) filters.Add($"groupBy:\"{GroupBy}\"");
        if (Where != null) filters.Add(Where.BuildWithWhere());
        if (NearText != null) filters.Add(NearText.ToString());
        if (NearObject != null) filters.Add(NearObject.ToString());
        if (NearVector != null) filters.Add(NearVector.ToString());
        if (Ask != null) filters.Add(Ask.ToString());
        if (NearImage != null) filters.Add(NearImage.ToString());
        if (Limit != null) filters.Add($"limit:{Limit}");
        if (ObjectLimit != null) filters.Add($"objectLimit:{ObjectLimit}");
        var s = string.Join(",", filters.ToArray());
        return $"({s})";
    }
}

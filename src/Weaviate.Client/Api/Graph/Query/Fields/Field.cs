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

namespace SearchPioneer.Weaviate.Client.Api.Graph.Query.Fields;

public class Field
{
    public string? Name { get; set; }
    public Field[]? Fields { get; set; }

    public override string ToString()
    {
        var s = new StringBuilder();
        if (!string.IsNullOrEmpty(Name)) s.Append(Name);
        if (Fields is { Length: > 0 }) s.Append($"{{{string.Join(" ", Fields.Select(f => f.ToString()).ToArray())}}}");
        return s.ToString();
    }

    public static implicit operator Field(string name)
    {
        return new()
        {
            Name = name
        };
    }
}
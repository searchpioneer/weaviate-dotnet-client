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

namespace SearchPioneer.Weaviate.Client.Api.Graph.Query.Arguments;

public class NearTextMoveParameters
{
    public string[]? Concepts { get; set; }
    public float? Force { get; set; }
    public ObjectMove[]? Objects { get; set; }

    public override string ToString()
    {
        var arg = new HashSet<string>();
        if (Concepts is { Length: > 0 }) arg.Add($"concepts: [{Concepts.WrapDoubleQuoteAndJoinWithComma()}]");
        if (Force != null) arg.Add($"force:{Force}");
        if (Objects is { Length: > 0 })
        {
            var values = Objects.Select(o => o.ToString()).ToArray();
            var objects = string.Join(",", values);
            arg.Add($"objects: [{objects}]");
        }

        return string.Join(" ", arg.ToArray());
    }
}
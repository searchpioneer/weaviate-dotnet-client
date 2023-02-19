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

public class Ask
{
    public string? Question { get; set; }
    public string[]? Properties { get; set; }
    public float? Certainty { get; set; }
    public float? Distance { get; set; }
    public bool? Autocorrect { get; set; }
    public bool? Rerank { get; set; }

    public override string ToString()
    {
        var arg = new HashSet<string>();
        if (!string.IsNullOrEmpty(Question)) arg.Add($"question:\"{Question}\"");
        if (Properties is { Length: > 0 }) arg.Add($"properties: [{Properties.WrapDoubleQuoteAndJoinWithComma()}]");
        if (Certainty != null) arg.Add($"certainty:{Certainty}");
        if (Distance != null) arg.Add($"distance:{Distance}");
        if (Autocorrect != null) arg.Add($"autocorrect:{Autocorrect.ToLower()}");
        if (Rerank != null) arg.Add($"rerank:{Rerank.ToLower()}");
        var s = string.Join(" ", arg.ToArray());
        return $"ask:{{{s}}}";
    }
}
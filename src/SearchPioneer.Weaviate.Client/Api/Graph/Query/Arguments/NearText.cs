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

using System.Globalization;

public class NearText
{
    public string[]? Concepts { get; set; } // TODO! change to class with object equivalency for arrays and single items
    public float? Certainty { get; set; }
    public float? Distance { get; set; }
    public NearTextMoveParameters? MoveTo { get; set; }
    public NearTextMoveParameters? MoveAwayFrom { get; set; }
    public bool? Autocorrect { get; set; }

    public override string ToString()
    {
        var arg = new HashSet<string>();
        if (Concepts is { Length: > 0 }) arg.Add($"concepts: [{Concepts.WrapDoubleQuoteAndJoinWithComma()}]");
        if (Certainty != null) arg.Add($"certainty:{Certainty.Value.ToString(CultureInfo.InvariantCulture)}");
        if (Distance != null) arg.Add($"distance:{Distance.Value.ToString(CultureInfo.InvariantCulture)}");
        if (MoveTo != null) arg.Add($"moveTo:{{{MoveTo}}}");
        if (MoveAwayFrom != null) arg.Add($"moveAwayFrom:{{{MoveAwayFrom}}}");
        if (Autocorrect != null) arg.Add($"autocorrect:{Autocorrect.ToLower()}");
        var s = string.Join(" ", arg.ToArray());
        return $"nearText:{{{s}}}";
    }
}

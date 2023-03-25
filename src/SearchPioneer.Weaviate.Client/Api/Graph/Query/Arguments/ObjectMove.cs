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

public class ObjectMove
{
    public string? Id { get; set; }
    public string? Beacon { get; set; }

    public override string ToString()
    {
        var args = new HashSet<string>();
        if (!string.IsNullOrEmpty(Id)) args.Add($"id:\"{Id}\"");
        if (!string.IsNullOrEmpty(Beacon)) args.Add($"beacon:\"{Beacon}\"");
        var s = string.Join(" ", args.ToArray());
        return $"{{{s}}}";
    }
}

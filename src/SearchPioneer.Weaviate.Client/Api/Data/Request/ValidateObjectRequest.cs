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

public class ValidateObjectRequest
{
    public ValidateObjectRequest(string id, string @class)
    {
        if (id == null) throw new ArgumentNullException(nameof(id));
        if (@class == null) throw new ArgumentNullException(nameof(@class));

        Id = id;
        Class = @class;
    }

    public string Id { get; }
    public string? Class { get; }
    public Dictionary<string, object>? Properties { get; set; }
}

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

using SearchPioneer.Weaviate.Client.Api.Graph.Query.Arguments;

namespace SearchPioneer.Weaviate.Client.Paths;

public class ObjectPathParams
{
    public string? Id { get; set; }
    public string? Class { get; set; }
    public int? Limit { get; set; }
    public string[]? Additional { get; set; }
    public ConsistencyLevel? ConsistencyLevel { get; set; }
    public string? NodeName { get; set; }
}
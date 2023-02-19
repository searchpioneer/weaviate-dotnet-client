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

using System.Text.Json.Serialization;
using SearchPioneer.Weaviate.Client.Api.Data.Model;
using SearchPioneer.Weaviate.Client.JsonConvertors;

namespace SearchPioneer.Weaviate.Client.Api.Data.Response;

public class WeaviateObjectResponse : WeaviateObject
{
    public object? VectorWeights { get; init; }
    public Dictionary<string, object>? Additional { get; init; }

    [JsonConverter(typeof(UnixSecondsDateTimeJsonConverter))]
    [JsonPropertyName("creationTimeUnix")]
    public DateTime? CreationTime { get; init; }

    [JsonConverter(typeof(UnixSecondsDateTimeJsonConverter))]
    [JsonPropertyName("lastUpdateTimeUnix")]
    public DateTime? LastUpdateTime { get; init; }
}
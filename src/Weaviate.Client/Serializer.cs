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

using System.Text.Json;
using System.Text.Json.Serialization;
using SearchPioneer.Weaviate.Client.JsonConvertors;

namespace SearchPioneer.Weaviate.Client;

public class Serializer
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters =
        {
            new OperatorJsonConverter(),
            new ShardStatusJsonConverter(),
            new StatusJsonConverter(),
            new ConsistencyLevelJsonConverter(),
            new TokenizationJsonConverter(),
            new BackendJsonConverter(),
            new BackupStatusJsonConverter(),
            new BatchDeleteOutputJsonConverter(),
            new BatchDeleteResultStatusJsonConverter(),
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, false)
        },
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public T Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json, _jsonSerializerOptions);
    }

    public string Serialize<T>(T @object)
    {
        return JsonSerializer.Serialize(@object, _jsonSerializerOptions);
    }
}
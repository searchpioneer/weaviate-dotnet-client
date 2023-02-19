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
using SearchPioneer.Weaviate.Client.Api.Graph.Query.Arguments;

namespace SearchPioneer.Weaviate.Client.JsonConvertors;

internal class ConsistencyLevelJsonConverter : JsonConverter<ConsistencyLevel>
{
    private const string All = "ALL";
    private const string One = "ONE";
    private const string Quorum = "QUORUM";

    private static ConsistencyLevel FromString(string status)
    {
        return status switch
        {
            All => ConsistencyLevel.All,
            One => ConsistencyLevel.One,
            Quorum => ConsistencyLevel.Quorum,
            _ => throw new ArgumentOutOfRangeException(nameof(status))
        };
    }

    public static string ToString(ConsistencyLevel status)
    {
        return status switch
        {
            ConsistencyLevel.All => All,
            ConsistencyLevel.One => One,
            ConsistencyLevel.Quorum => Quorum,
            _ => throw new ArgumentOutOfRangeException(nameof(status))
        };
    }

    public override ConsistencyLevel Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return FromString(reader.GetString()!);
    }

    public override void Write(Utf8JsonWriter writer, ConsistencyLevel value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(ToString(value));
    }
}
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
using SearchPioneer.Weaviate.Client.Api.Schema.Model;

namespace SearchPioneer.Weaviate.Client.JsonConvertors;

internal class ShardStatusJsonConverter : JsonConverter<ShardStatus>
{
    private const string Readonly = "READONLY";
    private const string Ready = "READY";

    private static ShardStatus FromString(string status)
    {
        return status switch
        {
            Ready => ShardStatus.Ready,
            Readonly => ShardStatus.ReadOnly,
            _ => throw new ArgumentOutOfRangeException(nameof(status))
        };
    }

    private static string ToString(ShardStatus status)
    {
        return status switch
        {
            ShardStatus.Ready => Ready,
            ShardStatus.ReadOnly => Readonly,
            _ => throw new ArgumentOutOfRangeException(nameof(status))
        };
    }

    public override ShardStatus Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return FromString(reader.GetString()!);
    }

    public override void Write(Utf8JsonWriter writer, ShardStatus value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(ToString(value));
    }
}
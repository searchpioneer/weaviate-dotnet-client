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
using SearchPioneer.Weaviate.Client.Api.Batch.Response;

namespace SearchPioneer.Weaviate.Client.JsonConvertors;

internal class BatchDeleteResultStatusJsonConverter : JsonConverter<BatchResultStatus>
{
    private const string Success = "SUCCESS";
    private const string Failed = "FAILED";
    private const string DryRun = "DRYRUN";

    private static BatchResultStatus FromString(string status)
    {
        return status switch
        {
            Success => BatchResultStatus.Success,
            Failed => BatchResultStatus.Failed,
            DryRun => BatchResultStatus.DryRun,
            _ => throw new ArgumentOutOfRangeException(nameof(status))
        };
    }

    private static string ToString(BatchResultStatus status)
    {
        return status switch
        {
            BatchResultStatus.Success => Success,
            BatchResultStatus.Failed => Failed,
            BatchResultStatus.DryRun => DryRun,
            _ => throw new ArgumentOutOfRangeException(nameof(status))
        };
    }

    public override BatchResultStatus Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        return FromString(reader.GetString()!);
    }

    public override void Write(Utf8JsonWriter writer, BatchResultStatus value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(ToString(value));
    }
}
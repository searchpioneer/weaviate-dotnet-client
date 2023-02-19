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
using SearchPioneer.Weaviate.Client.Api.Backup.Model;

namespace SearchPioneer.Weaviate.Client.JsonConvertors;

internal class BackupStatusJsonConverter : JsonConverter<BackupStatus>
{
    private const string Started = "STARTED";
    private const string Transferring = "TRANSFERRING";
    private const string Transferred = "TRANSFERRED";
    private const string Success = "SUCCESS";
    private const string Failed = "FAILED";

    private static BackupStatus FromString(string status)
    {
        return status switch
        {
            Started => BackupStatus.Started,
            Transferring => BackupStatus.Transferring,
            Transferred => BackupStatus.Transferred,
            Success => BackupStatus.Success,
            Failed => BackupStatus.Failed,
            _ => throw new ArgumentOutOfRangeException(nameof(status))
        };
    }

    private static string ToString(BackupStatus status)
    {
        return status switch
        {
            BackupStatus.Started => Started,
            BackupStatus.Transferring => Transferring,
            BackupStatus.Transferred => Transferred,
            BackupStatus.Success => Success,
            BackupStatus.Failed => Failed,
            _ => throw new ArgumentOutOfRangeException(nameof(status))
        };
    }

    public override BackupStatus Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return FromString(reader.GetString()!);
    }

    public override void Write(Utf8JsonWriter writer, BackupStatus value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(ToString(value));
    }
}
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


// ReSharper disable once CheckNamespace
namespace SearchPioneer.Weaviate.Client;

internal class BackendJsonConverter : JsonConverter<Backend>
{
    private const string Filesystem = "filesystem";
    private const string S3 = "s3";
    private const string Gcs = "gcs";
    private const string Azure = "azure";

    private static Backend FromString(string status) =>
	    status switch
	    {
		    Filesystem => Backend.Filesystem,
		    S3 => Backend.AmazonSimpleStorageService,
		    Gcs => Backend.GoogleCloudStorage,
		    Azure => Backend.AzureStorage,
		    _ => throw new ArgumentOutOfRangeException(nameof(status))
	    };

    public static string ToString(Backend status) =>
	    status switch
	    {
		    Backend.Filesystem => Filesystem,
		    Backend.AmazonSimpleStorageService => S3,
		    Backend.GoogleCloudStorage => Gcs,
		    Backend.AzureStorage => Azure,
		    _ => throw new ArgumentOutOfRangeException(nameof(status))
	    };

    public override Backend Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
	    FromString(reader.GetString()!);

    public override void Write(Utf8JsonWriter writer, Backend value, JsonSerializerOptions options) =>
	    writer.WriteStringValue(ToString(value));
}

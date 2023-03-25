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

internal class StatusJsonConverter : JsonConverter<Status>
{
    private const string Healthy = "HEALTHY";
    private const string Unhealthy = "UNHEALTHY";
    private const string Unavailable = "UNAVAILABLE";

    private static Status FromString(string status) =>
	    status switch
	    {
		    Healthy => Status.Healthy,
		    Unhealthy => Status.Unhealthy,
		    Unavailable => Status.Unavailable,
		    _ => throw new ArgumentOutOfRangeException(nameof(status))
	    };

    private static string ToString(Status status) =>
	    status switch
	    {
		    Status.Healthy => Healthy,
		    Status.Unhealthy => Unhealthy,
		    Status.Unavailable => Unavailable,
		    _ => throw new ArgumentOutOfRangeException(nameof(status))
	    };

    public override Status Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
	    FromString(reader.GetString()!);

    public override void Write(Utf8JsonWriter writer, Status value, JsonSerializerOptions options) =>
	    writer.WriteStringValue(ToString(value));
}

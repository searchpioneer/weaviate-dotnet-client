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

namespace SearchPioneer.Weaviate.Client;

internal class DistributionTypeJsonConverter : JsonConverter<DistributionType>
{
	private const string LogNormal = "log-normal";
	private const string Normal = "normal";

	private static DistributionType FromString(string status) =>
		status switch
		{
			LogNormal => DistributionType.LogNormal,
			Normal => DistributionType.Normal,
			_ => throw new ArgumentOutOfRangeException(nameof(status))
		};

	private static string ToString(DistributionType status) =>
		status switch
		{
			DistributionType.LogNormal => LogNormal,
			DistributionType.Normal => Normal,
			_ => throw new ArgumentOutOfRangeException(nameof(status))
		};

	public override DistributionType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
		FromString(reader.GetString()!);

	public override void Write(Utf8JsonWriter writer, DistributionType value, JsonSerializerOptions options) =>
		writer.WriteStringValue(ToString(value));
}

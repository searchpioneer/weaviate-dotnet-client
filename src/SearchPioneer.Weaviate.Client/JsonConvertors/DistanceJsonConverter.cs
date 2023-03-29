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

internal class DistanceJsonConverter : JsonConverter<Distance>
{
	private const string Cosine = "cosine";
	private const string DotProduct = "dot";
	private const string L2Squared = "l2-squared";
	private const string Hamming = "hamming";
	private const string Manhattan = "manhattan";

	private static Distance FromString(string status) =>
		status switch
		{
			Cosine => Distance.Cosine,
			DotProduct => Distance.DotProduct,
			L2Squared => Distance.L2Squared,
			Hamming => Distance.Hamming,
			Manhattan => Distance.Manhattan,
			_ => throw new ArgumentOutOfRangeException(nameof(status))
		};

	private static string ToString(Distance status) =>
		status switch
		{
			Distance.Cosine => Cosine,
			Distance.DotProduct => DotProduct,
			Distance.L2Squared => L2Squared,
			Distance.Hamming => Hamming,
			Distance.Manhattan => Manhattan,
			_ => throw new ArgumentOutOfRangeException(nameof(status))
		};

	public override Distance Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
		FromString(reader.GetString()!);

	public override void Write(Utf8JsonWriter writer, Distance value, JsonSerializerOptions options) =>
		writer.WriteStringValue(ToString(value));
}

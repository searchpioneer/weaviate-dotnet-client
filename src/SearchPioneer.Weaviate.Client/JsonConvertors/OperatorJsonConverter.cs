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

internal class OperatorJsonConverter : JsonConverter<Operator>
{
    private const string And = "And";
    private const string Equal = "Equal";
    private const string GreaterThan = "GreaterThan";
    private const string GreaterThanEqual = "GreaterThanEqual";
    private const string LessThan = "LessThan";
    private const string LessThanEqual = "LessThanEqual";
    private const string Like = "Like";
    private const string Not = "Not";
    private const string NotEqual = "NotEqual";
    private const string Or = "Or";
    private const string WithinGeoRange = "WithinGeoRange";

    private static Operator FromString(string status) =>
	    status switch
	    {
		    And => Operator.And,
		    Equal => Operator.Equal,
		    GreaterThan => Operator.GreaterThan,
		    GreaterThanEqual => Operator.GreaterThanEqual,
		    LessThan => Operator.LessThan,
		    LessThanEqual => Operator.LessThanEqual,
		    Like => Operator.Like,
		    Not => Operator.Not,
		    NotEqual => Operator.NotEqual,
		    Or => Operator.Or,
		    WithinGeoRange => Operator.WithinGeoRange,
		    _ => throw new ArgumentOutOfRangeException(nameof(status))
	    };

    private static string ToString(Operator status) =>
	    status switch
	    {
		    Operator.And => And,
		    Operator.Equal => Equal,
		    Operator.GreaterThan => GreaterThan,
		    Operator.GreaterThanEqual => GreaterThanEqual,
		    Operator.LessThan => LessThan,
		    Operator.LessThanEqual => LessThanEqual,
		    Operator.Like => Like,
		    Operator.Not => Not,
		    Operator.NotEqual => NotEqual,
		    Operator.Or => Or,
		    Operator.WithinGeoRange => WithinGeoRange,
		    _ => throw new ArgumentOutOfRangeException(nameof(status))
	    };

    public override Operator Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
	    FromString(reader.GetString()!);

    public override void Write(Utf8JsonWriter writer, Operator value, JsonSerializerOptions options) =>
	    writer.WriteStringValue(ToString(value));
}

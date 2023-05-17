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

// ReSharper disable once CheckNamespace
namespace SearchPioneer.Weaviate.Client;

using System.Globalization;

public class Where
{
    public Where[]? Operands { get; set; }
    public Operator? Operator { get; set; }
    public string[]? Path { get; set; } // TODO! value object with built in conversions

    // TODO: use polymorphism for these Value*?
    public bool? ValueBoolean { get; set; }
    public DateTime? ValueDate { get; set; }
    public GeoRange? ValueGeoRange { get; set; }
    public int? ValueInt { get; set; }
    public double? ValueNumber { get; set; }
    public string? ValueString { get; set; }
    public string? ValueText { get; set; }

    public string BuildWithWhere() => $"where:{{{ToString()}}}";

    public override string ToString()
    {
        var args = new HashSet<string>();

        if (Operands is { Length: > 0 })
        {
            if (Operator != null) args.Add($"operator:{Operator}");
            var ops = Operands.Select(operand => $"{{{operand}}}").ToArray();
            var operands = string.Join(",", ops);
            args.Add($"operands:[{operands}]");
        }
        else
        {
            if (Path is { Length: > 0 }) args.Add($"path:[{Path.WrapDoubleQuoteAndJoinWithComma()}]");
            if (ValueInt.HasValue) args.Add($"valueInt:{ValueInt}");
            if (ValueNumber.HasValue) args.Add($"valueNumber:{ValueNumber.Value.ToString(CultureInfo.InvariantCulture)}");
            if (ValueBoolean.HasValue) args.Add($"valueBoolean:{ValueBoolean.Value.ToString().ToLower()}");
            if (ValueString != null)
            {
                object value = $"\"{ValueString}\"";
                args.Add($"valueString:{value}");
            }

            if (ValueText != null)
            {
                object value = $"\"{ValueText}\"";
                args.Add($"valueText:{value}");
            }

            if (ValueDate.HasValue) args.Add($"valueDate:\"{ValueDate.Value:o}\"");
            if (ValueGeoRange != null) args.Add($"valueGeoRange:{ValueGeoRange.ToString()}");
            if (Operator.HasValue) args.Add($"operator:{Operator}");
        }

        return string.Join(" ", args.ToArray());
    }
}

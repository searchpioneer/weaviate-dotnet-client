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

public static class FieldExtensionMethods
{
    public static Field AsField(this string value) =>
	    new()
	    {
		    Name = value
	    };

    public static Field[] AsFields(this string value) =>
	    new[]
	    {
		    new Field
		    {
			    Name = value
		    }
	    };

    // TODO! additional fields are known, so model them explicitly
    public static Field AsAdditional(this string value) =>
	    new()
	    {
		    Name = "_additional",
		    Fields = new[]
		    {
			    new Field
			    {
				    Name = value
			    }
		    }
	    };

    public static Field[] AsFields(this IEnumerable<string> value) => value.Select(v => new Field { Name = v }).ToArray();

    public static Field AsAdditionalFields(this Field[] additionalFields) =>
	    new()
	    {
		    Name = "_additional",
		    Fields = additionalFields
	    };

    public static Field AsAdditionalField(this Field additionalField) =>
	    new()
	    {
		    Name = "_additional",
		    Fields = new[] { additionalField }
	    };
}

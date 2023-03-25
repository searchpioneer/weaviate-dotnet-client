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
using Xunit;

namespace SearchPioneer.Weaviate.Client.IntegrationTests.Api.Data;

[Collection("Sequential")]
public class CreateTests : TestBase
{
	[Fact]
	public void Create()
	{
		CreateWeaviateTestSchemaFood(Client);

		var id1 = "abefd256-8574-442b-9293-9205193737ee";
		var id2 = "565da3b6-60b3-40e5-ba21-e6bfe5dbba91";

		var newPizza = Client.Data.Create(new(CLASS_NAME_PIZZA)
		{
			Id = id1,
			Properties = new()
			{
				{ "name", "Hawaii" },
				{ "description", "Universally accepted to be the best pizza ever created." }
			}
		});
		Assert.True(newPizza.HttpStatusCode == 200);

		var newSoup = Client.Data.Create(new(CLASS_NAME_SOUP)
		{
			Id = id2,
			Properties = new()
			{
				{ "name", "ChickenSoup" },
				{
					"description",
					"Used by humans when their inferior genetics are attacked by microscopic organisms."
				}
			}
		});
		Assert.True(newSoup.HttpStatusCode == 200);


		var pizza = Client.Data.Get(new() { Class = CLASS_NAME_PIZZA, Id = id1 });
		Assert.True(pizza.HttpStatusCode == 200);
		Assert.Equal("Hawaii", ((JsonElement)pizza.Result.Single().Properties!["name"]).GetString());

		var soup = Client.Data.Get(new() { Class = CLASS_NAME_SOUP, Id = id2 });
		Assert.True(soup.HttpStatusCode == 200);
		Assert.Equal("ChickenSoup", ((JsonElement)soup.Result.Single().Properties!["name"]).GetString());
	}

	[Fact]
	public void CreateWithSpecialCharacters()
	{
		CreateWeaviateTestSchemaFood(Client);

		var id1 = "abefd256-8574-442b-9293-9205193737ee";

		var name = "Zażółć gęślą jaźń";
		var description = "test äüëö";

		var newPizza = Client.Data.Create(new(CLASS_NAME_PIZZA)
		{
			Id = id1, Properties = new() { { "name", name }, { "description", description } }
		});
		Assert.True(newPizza.HttpStatusCode == 200);

		var pizza = Client.Data.Get(new() { Class = CLASS_NAME_PIZZA, Id = id1 });
		Assert.True(pizza.HttpStatusCode == 200);
		Assert.Equal(name, ((JsonElement)pizza.Result.Single().Properties!["name"]).GetString());
		Assert.Equal(description, ((JsonElement)pizza.Result.Single().Properties!["description"]).GetString());
	}

	[Fact]
	public void CreateWithArrayType()
	{
		Client.Schema.DeleteAllClasses();

		const string @class = "ClassArrays";
		var schema = Client.Schema.CreateSchemaClass(new(@class)
		{
			Description = "Class which properties are all array properties",
			VectorIndexType = VectorIndexType.HNSW,
			Vectorizer = Vectorizer.Text2VecContextionary,
			Properties = new Property[]
			{
				new() { Name = "stringArray", DataType = new[] { DataType.StringArray } },
				new() { Name = "textArray", DataType = new[] { DataType.TextArray } },
				new() { Name = "intArray", DataType = new[] { DataType.IntArray } },
				new() { Name = "numberArray", DataType = new[] { DataType.NumberArray } },
				new() { Name = "booleanArray", DataType = new[] { DataType.BooleanArray } }
			}
		});
		Assert.True(schema.HttpStatusCode == 200);

		var id = "abefd256-8574-442b-9293-9205193737ee";

		var create = Client.Data.Create(new(@class)
		{
			Id = id,
			Properties = new()
			{
				{ "stringArray", new[] { "a", "b" } },
				{ "textArray", new[] { "c", "d" } },
				{ "intArray", new[] { 1, 2 } },
				{ "numberArray", new[] { 3.3f, 4.4f } },
				{ "booleanArray", new[] { true, false } }
			}
		});
		Assert.True(create.HttpStatusCode == 200);

		var get = Client.Data.Get(new() { Id = id, Class = @class });
		Assert.True(get.HttpStatusCode == 200);
		// TODO: we should have extension methods for this.
		Assert.Equal(new[] { "a", "b" },
			((JsonElement)get.Result.Single().Properties!["stringArray"]).EnumerateArray().Select(a => a.GetString())
			.ToArray());
		Assert.Equal(new[] { "c", "d" },
			((JsonElement)get.Result.Single().Properties!["textArray"]).EnumerateArray().Select(a => a.GetString())
			.ToArray());
		Assert.Equal(new[] { 1, 2 },
			((JsonElement)get.Result.Single().Properties!["intArray"]).EnumerateArray().Select(a => a.GetInt32())
			.ToArray());
		Assert.Equal(new[] { 3.3f, 4.4f },
			((JsonElement)get.Result.Single().Properties!["numberArray"]).EnumerateArray().Select(a => a.GetSingle())
			.ToArray());
		Assert.Equal(new[] { true, false },
			((JsonElement)get.Result.Single().Properties!["booleanArray"]).EnumerateArray().Select(a => a.GetBoolean())
			.ToArray());
	}

	[Fact]
	public void CreateWithIdInNotUuidFormat()
	{
		CreateWeaviateTestSchemaFood(Client);

		var newPizza = Client.Data.Create(new(CLASS_NAME_PIZZA)
		{
			Id = "not-uuid", Properties = new() { { "name", "name" }, { "description", "description" } }
		});
		Assert.True(newPizza.HttpStatusCode == 422);
		Assert.Contains(
			"id in body must be of type uuid",
			newPizza.Error!.Message);
	}
}

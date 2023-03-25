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
public class UpdateTests : TestBase
{
	[Fact]
	public void Update()
	{
		CreateWeaviateTestSchemaFood(Client);

		var pizzaId = "abefd256-8574-442b-9293-9205193737ee";
		var soupId = "565da3b6-60b3-40e5-ba21-e6bfe5dbba91";

		var badPizza = Client.Data.Create(new(CLASS_NAME_PIZZA)
		{
			Id = pizzaId, Properties = new() { { "name", "Hawaii" }, { "description", "Missing description" } }
		});
		Assert.True(badPizza.HttpStatusCode == 200);

		var badSoup = Client.Data.Create(new(CLASS_NAME_SOUP)
		{
			Id = soupId, Properties = new() { { "name", "Water" }, { "description", "Missing description" } }
		});
		Assert.True(badSoup.HttpStatusCode == 200);

		var updatePizza = Client.Data.Update(new(pizzaId, CLASS_NAME_PIZZA)
		{
			Properties = new()
			{
				{ "name", "Hawaii" },
				{ "description", "Universally accepted to be the best pizza ever created." }
			}
		});
		Assert.True(updatePizza.HttpStatusCode == 200);

		var updateSoup = Client.Data.Update(new(soupId, CLASS_NAME_SOUP)
		{
			Properties = new()
			{
				{ "name", "ChickenSoup" },
				{
					"description",
					"Used by humans when their inferior genetics are attacked by microscopic organisms."
				}
			}
		});
		Assert.True(updateSoup.HttpStatusCode == 200);

		var pizza = Client.Data.Get(new() { Class = CLASS_NAME_PIZZA, Id = pizzaId });
		Assert.True(pizza.HttpStatusCode == 200);
		Assert.Equal("Hawaii", ((JsonElement)pizza.Result.Single().Properties!["name"]).GetString());

		var soup = Client.Data.Get(new() { Class = CLASS_NAME_SOUP, Id = soupId });
		Assert.True(soup.HttpStatusCode == 200);
		Assert.Equal("ChickenSoup", ((JsonElement)soup.Result.Single().Properties!["name"]).GetString());
	}

	[Fact]
	public void Merge()
	{
		CreateWeaviateTestSchemaFood(Client);

		var pizzaId = "abefd256-8574-442b-9293-9205193737ee";
		var soupId = "565da3b6-60b3-40e5-ba21-e6bfe5dbba91";

		var badPizza = Client.Data.Create(new(CLASS_NAME_PIZZA)
		{
			Id = pizzaId, Properties = new() { { "name", "Hawaii" }, { "description", "Missing description" } }
		});
		Assert.True(badPizza.HttpStatusCode == 200);

		var badSoup = Client.Data.Create(new(CLASS_NAME_SOUP)
		{
			Id = soupId, Properties = new() { { "name", "ChickenSoup" }, { "description", "Missing description" } }
		});
		Assert.True(badSoup.HttpStatusCode == 200);

		var updatePizza = Client.Data.Update(new(pizzaId, CLASS_NAME_PIZZA)
		{
			Properties = new() { { "description", "Universally accepted to be the best pizza ever created." } },
			WithMerge = true
		});
		Assert.True(updatePizza.HttpStatusCode == 204);

		var updateSoup = Client.Data.Update(new(soupId, CLASS_NAME_SOUP)
		{
			Properties = new()
			{
				{
					"description",
					"Used by humans when their inferior genetics are attacked by microscopic organisms."
				}
			},
			WithMerge = true
		});
		Assert.True(updateSoup.HttpStatusCode == 204);

		var pizza = Client.Data.Get(new() { Class = CLASS_NAME_PIZZA, Id = pizzaId });
		Assert.True(pizza.HttpStatusCode == 200);
		Assert.Equal("Hawaii", ((JsonElement)pizza.Result.Single().Properties!["name"]).GetString());
		Assert.Equal("Universally accepted to be the best pizza ever created.",
			((JsonElement)pizza.Result.Single().Properties!["description"]).GetString());

		var soup = Client.Data.Get(new() { Class = CLASS_NAME_SOUP, Id = soupId });
		Assert.True(soup.HttpStatusCode == 200);
		Assert.Equal("ChickenSoup", ((JsonElement)soup.Result.Single().Properties!["name"]).GetString());
		Assert.Equal("Used by humans when their inferior genetics are attacked by microscopic organisms.",
			((JsonElement)soup.Result.Single().Properties!["description"]).GetString());
	}
}

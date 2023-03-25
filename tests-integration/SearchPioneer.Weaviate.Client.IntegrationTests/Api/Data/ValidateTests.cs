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

using Xunit;

namespace SearchPioneer.Weaviate.Client.IntegrationTests.Api.Data;

[Collection("Sequential")]
public class ValidateTests : TestBase
{
	[Fact]
	public void Validate()
	{
		CreateWeaviateTestSchemaFood(Client);

		var pizzaId = "abefd256-8574-442b-9293-9205193737ee";
		var soupId = "565da3b6-60b3-40e5-ba21-e6bfe5dbba91";

		var pizza = Client.Data.Validate(new(pizzaId, CLASS_NAME_PIZZA)
		{
			Properties = new()
			{
				{ "name", "Hawaii" },
				{ "description", "Universally accepted to be the best pizza ever created." }
			}
		});
		Assert.True(pizza.HttpStatusCode == 200);

		var soup = Client.Data.Validate(new(soupId, CLASS_NAME_SOUP)
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
		Assert.True(soup.HttpStatusCode == 200);

		var pizza2 = Client.Data.Validate(new(pizzaId, CLASS_NAME_PIZZA)
		{
			Properties = new()
			{
				{ "name", "test" },
				{ "description", "Universally accepted to be the best pizza ever created." },
				{ "test", "not existing property" }
			}
		});
		Assert.True(pizza2.HttpStatusCode == 422);
		Assert.Contains(
			"invalid object: no such prop with name 'test' found in class 'Pizza' in the schema. Check your schema files for which properties in this class are available",
			pizza2.Error!.Error!.Select(e => e.Message));

		var soup2 = Client.Data.Validate(new(soupId, CLASS_NAME_SOUP)
		{
			Properties = new()
			{
				{ "name", "ChickenSoup" },
				{
					"description",
					"Used by humans when their inferior genetics are attacked by microscopic organisms."
				},
				{ "test", "not existing property" }
			}
		});
		Assert.True(soup2.HttpStatusCode == 422);
		Assert.Contains(
			"invalid object: no such prop with name 'test' found in class 'Soup' in the schema. Check your schema files for which properties in this class are available",
			soup2.Error!.Error!.Select(e => e.Message));
	}
}

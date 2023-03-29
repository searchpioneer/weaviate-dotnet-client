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
public class DeleteTests : TestBase
{
	[Fact]
	public void Delete()
	{
		CreateWeaviateTestSchemaFood(Client);

		var pizzaId = "abefd256-8574-442b-9293-9205193737ee";
		var soupId = "565da3b6-60b3-40e5-ba21-e6bfe5dbba91";

		var hawaiian = Client.Data.Create(new(CLASS_NAME_PIZZA)
		{
			Id = pizzaId,
			Properties = new()
			{
				{ "name", "Hawaii" },
				{ "description", "Universally accepted to be the best pizza ever created." }
			},
			ConsistencyLevel = ConsistencyLevel.Quorum
		});
		Assert.True(hawaiian.HttpStatusCode == 200);

		var chickensoup = Client.Data.Create(new(CLASS_NAME_SOUP)
		{
			Id = soupId,
			Properties = new()
			{
				{ "name", "ChickenSoup" },
				{
					"description",
					"Used by humans when their inferior genetics are attacked by microscopic organisms."
				}
			},
			ConsistencyLevel = ConsistencyLevel.Quorum
		});
		Assert.True(chickensoup.HttpStatusCode == 200);

		var pizzaDelete =
			Client.Data.Delete(new(pizzaId) { Class = CLASS_NAME_PIZZA, ConsistencyLevel = ConsistencyLevel.Quorum });
		var soupDelete =
			Client.Data.Delete(new(soupId) { Class = CLASS_NAME_SOUP, ConsistencyLevel = ConsistencyLevel.Quorum });

		Assert.True(pizzaDelete.HttpStatusCode == 204);
		Assert.True(soupDelete.HttpStatusCode == 204);
	}
}

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

namespace SearchPioneer.Weaviate.Client.IntegrationTests.Api.Batch;

[Collection("Sequential")]
public class CreateTests : TestBase
{
	[Fact]
	public void Create()
	{
		const string id1 = "abefd256-8574-442b-9293-9205193737ee";
		const string id2 = "97fa5147-bdad-4d74-9a81-f8babc811b09";
		const string id3 = "565da3b6-60b3-40e5-ba21-e6bfe5dbba91";
		const string id4 = "07473b34-0ab2-4120-882d-303d9e13f7af";

		CreateWeaviateTestSchemaFood(Client);

		var batch = Client.Batch.CreateObjects(new(
			new WeaviateObject
			{
				Id = id1,
				Class = CLASS_NAME_PIZZA,
				Properties = new()
				{
					{ "name", "Hawaii" },
					{ "description", "Universally accepted to be the best pizza ever created." }
				}
			},
			new WeaviateObject
			{
				Id = id2,
				Class = CLASS_NAME_PIZZA,
				Properties = new()
				{
					{ "name", "Hawaii" },
					{ "description", "Universally accepted to be the best pizza ever created." }
				}
			},
			new WeaviateObject
			{
				Id = id3,
				Class = CLASS_NAME_SOUP,
				Properties = new()
				{
					{ "name", "ChickenSoup" },
					{
						"description",
						"Used by humans when their inferior genetics are attacked by microscopic organisms."
					}
				}
			},
			new WeaviateObject
			{
				Id = id4,
				Class = "Beautiful",
				Properties = new()
				{
					{ "name", "Hawaii" },
					{ "description", "Putting the game of letter soups to a whole new level." }
				}
			})
		{
			ConsistencyLevel = ConsistencyLevel.Quorum
		});
		Assert.True(batch.HttpStatusCode == 200);

		void AssertCount(ApiResponse<WeaviateObjectResponse[]> result)
		{
			Assert.Single(result.Result);
		}

		AssertCount(Client.Data.Get(new() { Class = CLASS_NAME_PIZZA, Id = id1 }));
		AssertCount(Client.Data.Get(new() { Class = CLASS_NAME_PIZZA, Id = id2 }));
		AssertCount(Client.Data.Get(new() { Class = CLASS_NAME_SOUP, Id = id3 }));
		AssertCount(Client.Data.Get(new() { Class = "Beautiful", Id = id4 }));
	}
}

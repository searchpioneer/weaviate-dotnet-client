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
public class CheckTests : TestBase
{
	[Fact]
	public void Check()
	{
		CreateTestSchemaAndData(Client);

		var nonExistent = Client.Data.Check(new("11111111-1111-1111-aaaa-aaaaaaaaaaaa"));
		Assert.True(nonExistent.HttpStatusCode == 404);

		var pizza = Client.Data.Check(new(PIZZA_HAWAII_ID));
		Assert.True(pizza.HttpStatusCode == 204);

		var soup = Client.Data.Check(new(SOUP_CHICKENSOUP_ID));
		Assert.True(soup.HttpStatusCode == 204);

		Client.Schema.DeleteAllClasses();

		var pizzaNone = Client.Data.Check(new(PIZZA_HAWAII_ID));
		Assert.True(pizzaNone.HttpStatusCode == 404);

		var soupNone = Client.Data.Check(new(SOUP_CHICKENSOUP_ID));
		Assert.True(soupNone.HttpStatusCode == 404);
	}
}

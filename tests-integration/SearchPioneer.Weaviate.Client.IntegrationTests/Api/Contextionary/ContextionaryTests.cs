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

namespace SearchPioneer.Weaviate.Client.IntegrationTests.Api.Contextionary;

[Collection("Sequential")]
public class ContextionaryTests : TestBase
{
	[Fact]
	public void Getter()
	{
		CreateTestSchemaAndData(Client);

		var result = Client.Contextionary.GetConcepts(new() { Concept = "pizzaHawaii" });

		Assert.Equal(12, result.Result!.ConcatenatedWord.ConcatenatedNearestNeighbors.Length);
	}

	[Fact]
	public void ExtensionCreator()
	{
		CreateTestSchemaAndData(Client);

		var result = Client.Contextionary.CreateExtension(new()
		{
			Concept = "xoxo", Definition = "Hugs and kisses", Weight = 1.0f
		});

		Assert.True(result.HttpStatusCode == 200);
	}
}

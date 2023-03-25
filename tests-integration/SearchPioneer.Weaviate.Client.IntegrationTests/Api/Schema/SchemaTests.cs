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

namespace SearchPioneer.Weaviate.Client.IntegrationTests.Api.Schema;

[Collection("Sequential")]
public class SchemaTests : TestBase
{
	[Fact]
	public void CreateBandClass()
	{
		Client.Schema.DeleteAllClasses();

		var createStatus = Client.Schema.CreateSchemaClass(new("Band")
		{
			Description = "Band that plays and produces music",
			VectorIndexType = VectorIndexType.HNSW,
			Vectorizer = Vectorizer.Text2VecContextionary
		});
		Assert.True(createStatus.HttpStatusCode == 200);

		var schema = Client.Schema.GetSchema();
		Assert.True(schema.HttpStatusCode == 200);
		Assert.Single(schema.Result.Classes);

		var deleteStatus = Client.Schema.DeleteClass(new("Band"));
		Assert.True(deleteStatus.HttpStatusCode == 200);
	}

	[Fact]
	public void CreateRunClass()
	{
		Client.Schema.DeleteAllClasses();

		var createStatus = Client.Schema.CreateSchemaClass(new("Run")
		{
			Description = "Running from the fuzz",
			VectorIndexType = VectorIndexType.HNSW,
			Vectorizer = Vectorizer.Text2VecContextionary
		});
		Assert.True(createStatus.HttpStatusCode == 200);

		var schema = Client.Schema.GetSchema();
		Assert.True(schema.HttpStatusCode == 200);
		Assert.Single(schema.Result.Classes);

		Client.Schema.DeleteAllClasses();

		var schemaAfterDelete = Client.Schema.GetSchema();
		Assert.True(schemaAfterDelete.HttpStatusCode == 200);
		Assert.Empty(schemaAfterDelete.Result.Classes);
	}

	[Fact]
	public void DeleteClasses()
	{
		CreateTestSchemaAndData(Client);

		var schemaAfterCreate = Client.Schema.GetSchema();
		Assert.Equal(2, schemaAfterCreate.Result.Classes.Count);

		var deletePizzas = Client.Schema.DeleteClass(new(CLASS_NAME_PIZZA));
		Assert.True(deletePizzas.HttpStatusCode == 200);

		var deleteSoups = Client.Schema.DeleteClass(new(CLASS_NAME_SOUP));
		Assert.True(deleteSoups.HttpStatusCode == 200);

		var schemaAfterDelete = Client.Schema.GetSchema();
		Assert.Empty(schemaAfterDelete.Result.Classes);
	}

	[Fact]
	public void DeleteAllClasses()
	{
		CreateTestSchemaAndData(Client);

		var schemaAfterCreate = Client.Schema.GetSchema();
		Assert.Equal(2, schemaAfterCreate.Result.Classes.Count);

		Client.Schema.DeleteAllClasses();

		var schemaAfterDelete = Client.Schema.GetSchema();
		Assert.Empty(schemaAfterDelete.Result.Classes);
	}

	[Fact]
	public void CreateClassesAddProperties()
	{
		CreateWeaviateTestSchemaFood(Client);

		var property = new Property
		{
			Name = "Additional", Description = "Additional property", DataType = new[] { DataType.String }
		};

		var pizzaProperty = Client.Schema.CreateProperty(new(CLASS_NAME_PIZZA) { Property = property });
		Assert.True(pizzaProperty.HttpStatusCode == 200);

		var soupProperty = Client.Schema.CreateProperty(new(CLASS_NAME_SOUP) { Property = property });
		Assert.True(soupProperty.HttpStatusCode == 200);

		var schemaAfterCreate = Client.Schema.GetSchema();
		Assert.True(schemaAfterCreate.HttpStatusCode == 200);
		foreach (var weaviateClass in schemaAfterCreate.Result.Classes)
		{
			Assert.Equal(5, weaviateClass.Properties.Length);
			Assert.Equal(Tokenization.Word, weaviateClass.Properties.Last().Tokenization);
		}
	}

	[Fact]
	public void CreateClassExplicitVectorizerWithProperties()
	{
		Client.Schema.DeleteAllClasses();

		var createStatus = Client.Schema.CreateSchemaClass(new("Article")
		{
			Description = "A written text, for example a news article or blog post",
			VectorIndexType = VectorIndexType.HNSW,
			Vectorizer = Vectorizer.Text2VecContextionary,
			Properties = new Property[]
			{
				new()
				{
					Name = "title",
					Description = "Title of the article",
					Tokenization = Tokenization.Field,
					DataType = new[] { DataType.String }
				},
				new()
				{
					Name = "content",
					Description = "The content of the article",
					Tokenization = Tokenization.Word,
					DataType = new[] { DataType.Text }
				}
			}
		});
		Assert.True(createStatus.HttpStatusCode == 200);

		var schema = Client.Schema.GetSchema();
		Assert.True(schema.HttpStatusCode == 200);
		Assert.Single(schema.Result.Classes);
		Assert.Equal(Tokenization.Field,
			schema.Result.Classes.First().Properties.Single(p => p.Name == "title").Tokenization);
		Assert.Equal(Tokenization.Word,
			schema.Result.Classes.First().Properties.Single(p => p.Name == "content").Tokenization);

		var deleteStatus = Client.Schema.DeleteClass(new("Article"));
		Assert.True(deleteStatus.HttpStatusCode == 200);
	}

	[Fact]
	public void CreateClassExplicitVectorizerWithArrayProperties()
	{
		Client.Schema.DeleteAllClasses();

		var createStatus = Client.Schema.CreateSchemaClass(new("ClassArrays")
		{
			Description = "Class which properties are all array properties",
			VectorIndexType = VectorIndexType.HNSW,
			Vectorizer = Vectorizer.Text2VecContextionary,
			Properties = new Property[]
			{
				new()
				{
					Name = "stringArray",
					Tokenization = Tokenization.Field,
					DataType = new[] { DataType.StringArray }
				},
				new()
				{
					Name = "textArray",
					Tokenization = Tokenization.Word,
					DataType = new[] { DataType.TextArray }
				},
				new() { Name = "intArray", DataType = new[] { DataType.IntArray } },
				new() { Name = "numberArray", DataType = new[] { DataType.NumberArray } },
				new() { Name = "booleanArray", DataType = new[] { DataType.BooleanArray } },
				new() { Name = "dateArray", DataType = new[] { DataType.DateArray } }
			}
		});
		Assert.True(createStatus.HttpStatusCode == 200);
		Assert.Equal(6, createStatus.Result.Properties.Length);
	}
}

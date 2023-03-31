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

	[Fact]
	public void CreateClassWithProperties()
	{
		Client.Schema.DeleteAllClasses();

		var createStatus = Client.Schema.CreateSchemaClass(new("Article")
		{
			Description = "A written text, for example a news article or blog post",
			Properties = new Property[]
			{
				new()
				{
					Name = "title",
					Description = "Title of the article",
					DataType = new[] { DataType.String }
				},
				new()
				{
					Name = "content",
					Description = "The content of the article",
					DataType = new[] { DataType.Text }
				}
			}
		});
		Assert.True(createStatus.HttpStatusCode == 200);

		Assert.Equal(Tokenization.Word, createStatus.Result!.Properties![0].Tokenization);
		Assert.Equal(Tokenization.Word, createStatus.Result!.Properties![1].Tokenization);
	}

	[Fact]
	public void CreateClassWithInvalidTokenizationProperty()
	{
		Client.Schema.DeleteAllClasses();

		var createStatus1 = Client.Schema.CreateSchemaClass(new("Pizza")
		{
			Description = "A delicious religion like food and arguably the best export of Italy.",
			Properties = new Property[]
			{
				new()
				{
					Name = "someText",
					Description = "someText",
					DataType = new[] { DataType.Text },
					Tokenization = Tokenization.Field
				}
			}
		});
		Assert.True(createStatus1.HttpStatusCode == 422);
		Assert.Equal("Tokenization 'field' is not allowed for data type 'text'", createStatus1.Error!.Error![0].Message!);

		var createStatus2 = Client.Schema.CreateSchemaClass(new("Pizza")
		{
			Description = "A delicious religion like food and arguably the best export of Italy.",
			Properties = new Property[]
			{
				new()
				{
					Name = "someInt",
					Description = "someInt",
					DataType = new[] { DataType.Int },
					Tokenization = Tokenization.Word
				}
			}
		});
		Assert.True(createStatus2.HttpStatusCode == 422);
		Assert.Equal("Tokenization 'word' is not allowed for data type 'int'", createStatus2.Error!.Error![0].Message!);
	}

	[Fact]
	public void CreateClassWithBM25Config()
	{
		Client.Schema.DeleteAllClasses();

		var createStatus = Client.Schema.CreateSchemaClass(new("Band")
		{
			Description = "Band that plays and produces music",
			VectorIndexType = VectorIndexType.HNSW,
			Vectorizer = Vectorizer.Text2VecContextionary,
			InvertedIndexConfig = new()
			{
				Bm25 = new() { B = 0.777f, K1 = 1.777f },
				Stopwords = new()
				{
					Preset = "en",
					Additions = new[] { "star", "nebula" },
					Removals = new[] { "a", "the" }
				},
				CleanupIntervalSeconds = 300
			}
		});
		Assert.True(createStatus.HttpStatusCode == 200);

		void Verify(WeaviateClass weaviateClass)
		{
			Assert.Equal(0.777f, weaviateClass.InvertedIndexConfig!.Bm25!.B);
			Assert.Equal(1.777f, weaviateClass.InvertedIndexConfig.Bm25.K1);
			Assert.Equal("en", weaviateClass.InvertedIndexConfig.Stopwords!.Preset);
			Assert.Equal(new[] { "star", "nebula" }, weaviateClass.InvertedIndexConfig.Stopwords!.Additions);
			Assert.Equal(new[] { "a", "the" }, weaviateClass.InvertedIndexConfig.Stopwords!.Removals);
			Assert.Equal(300, weaviateClass.InvertedIndexConfig.CleanupIntervalSeconds);
		}

		Verify(createStatus.Result!);

		var schemaAfterCreate = Client.Schema.GetClass(new("Band"));
		Assert.True(schemaAfterCreate.HttpStatusCode == 200);

		Verify(schemaAfterCreate.Result!);
	}

	[Fact]
	public void CreateClassWithStopwordsConfig()
	{
		Client.Schema.DeleteAllClasses();

		var createStatus = Client.Schema.CreateSchemaClass(new("Band")
		{
			Description = "Band that plays and produces music",
			VectorIndexType = VectorIndexType.HNSW,
			Vectorizer = Vectorizer.Text2VecContextionary,
			InvertedIndexConfig =  new()
				{
					Stopwords = new()
					{
						Preset = "en",
						Additions = new[] { "star", "nebula" },
						Removals = new[] { "a", "the" }
					}
				}
		});
		Assert.True(createStatus.HttpStatusCode == 200);

		void Verify(WeaviateClass weaviateClass)
		{
			Assert.Equal("en", weaviateClass.InvertedIndexConfig!.Stopwords!.Preset);
			Assert.Equal(new[] { "star", "nebula" }, weaviateClass.InvertedIndexConfig.Stopwords!.Additions);
			Assert.Equal(new[] { "a", "the" }, weaviateClass.InvertedIndexConfig.Stopwords!.Removals);
		}

		Verify(createStatus.Result!);

		var schemaAfterCreate = Client.Schema.GetClass(new("Band"));
		Assert.True(schemaAfterCreate.HttpStatusCode == 200);

		Verify(schemaAfterCreate.Result!);
	}

	[Fact]
	public void CreateClassWithBM25ConfigAndWithStopwordsConfig()
	{
		Client.Schema.DeleteAllClasses();

		var createStatus = Client.Schema.CreateSchemaClass(new("Band")
		{
			Description = "Band that plays and produces music",
			VectorIndexType = VectorIndexType.HNSW,
			Vectorizer = Vectorizer.Text2VecContextionary,
			InvertedIndexConfig = new()
			{
				Bm25 = new() { B = 0.777f, K1 = 1.777f },
				Stopwords = new()
				{
					Preset = "en",
					Additions = new[] { "star", "nebula" },
					Removals = new[] { "a", "the" }
				},
				CleanupIntervalSeconds = 300
			}
		});
		Assert.True(createStatus.HttpStatusCode == 200);

		void Verify(WeaviateClass weaviateClass)
		{
			Assert.Equal(0.777f, weaviateClass.InvertedIndexConfig!.Bm25!.B);
			Assert.Equal(1.777f, weaviateClass.InvertedIndexConfig.Bm25.K1);
			Assert.Equal("en", weaviateClass.InvertedIndexConfig.Stopwords!.Preset);
			Assert.Equal(new[] { "star", "nebula" }, weaviateClass.InvertedIndexConfig.Stopwords!.Additions);
			Assert.Equal(new[] { "a", "the" }, weaviateClass.InvertedIndexConfig.Stopwords!.Removals);
			Assert.Equal(300, weaviateClass.InvertedIndexConfig.CleanupIntervalSeconds);
		}

		Verify(createStatus.Result!);

		var schemaAfterCreate = Client.Schema.GetClass(new("Band"));
		Assert.True(schemaAfterCreate.HttpStatusCode == 200);

		Verify(schemaAfterCreate.Result!);
	}

	[Fact]
	public void GetShards()
	{
		Client.Schema.DeleteAllClasses();

		var createStatus = Client.Schema.CreateSchemaClass(new("Band")
		{
			Description = "Band that plays and produces music",
			VectorIndexType = VectorIndexType.HNSW,
			Vectorizer = Vectorizer.Text2VecContextionary
		});
		Assert.True(createStatus.HttpStatusCode == 200);

		var shards = Client.Schema.GetShards(new("Band"));
		Assert.True(shards.HttpStatusCode == 200);

		Assert.NotNull(shards.Result![0].Name);
		Assert.Equal(ShardStatus.Ready, shards.Result![0].Status);
	}

	[Fact]
	public void UpdateShard()
	{
		Client.Schema.DeleteAllClasses();

		var createStatus = Client.Schema.CreateSchemaClass(new("Band")
		{
			Description = "Band that plays and produces music",
			VectorIndexType = VectorIndexType.HNSW,
			Vectorizer = Vectorizer.Text2VecContextionary
		});
		Assert.True(createStatus.HttpStatusCode == 200);

		var shards = Client.Schema.GetShards(new("Band"));
		Assert.True(shards.HttpStatusCode == 200);

		Assert.NotNull(shards.Result![0].Name);
		Assert.Equal(ShardStatus.Ready, shards.Result![0].Status);

		var updateShard = Client.Schema.UpdateShard(new("Band", shards.Result![0].Name, ShardStatus.ReadOnly));
		Assert.True(updateShard.HttpStatusCode == 200);
		Assert.Equal(ShardStatus.ReadOnly, updateShard.Result);

		var readyShard = Client.Schema.UpdateShard(new("Band", shards.Result![0].Name, ShardStatus.Ready));
		Assert.True(readyShard.HttpStatusCode == 200);
		Assert.Equal(ShardStatus.Ready, readyShard.Result);
	}

	[Fact]
	public void UpdateShards()
	{
		const int shardCount = 3;

		Client.Schema.DeleteAllClasses();

		var createStatus = Client.Schema.CreateSchemaClass(new("Band")
		{
			Description = "Band that plays and produces music",
			VectorIndexType = VectorIndexType.HNSW,
			Vectorizer = Vectorizer.Text2VecContextionary,
			ShardingConfig = new()
			{
				ActualCount = shardCount,
				ActualVirtualCount = 128,
				DesiredCount = shardCount,
				DesiredVirtualCount = 128,
				Function = "murmur3",
				Key = "_id",
				Strategy = "hash",
				VirtualPerPhysical = 128
			}
		});
		Assert.True(createStatus.HttpStatusCode == 200);

		var shards = Client.Schema.GetShards(new("Band"));
		Assert.True(shards.HttpStatusCode == 200);
		Assert.Equal(3, shards.Result!.Length);

		foreach (var shard in shards.Result)
		{
			var updateShard = Client.Schema.UpdateShard(new("Band", shard.Name, ShardStatus.ReadOnly));
			Assert.True(updateShard.HttpStatusCode == 200);
			Assert.Equal(ShardStatus.ReadOnly, updateShard.Result);
		}

		foreach (var shard in shards.Result)
		{
			var updateShard = Client.Schema.UpdateShard(new("Band", shard.Name, ShardStatus.Ready));
			Assert.True(updateShard.HttpStatusCode == 200);
			Assert.Equal(ShardStatus.Ready, updateShard.Result);
		}
	}

	[Fact]
	public void CreateClassWithExplicitReplicationFactor()
	{
		Client.Schema.DeleteAllClasses();

		var createStatus = Client.Schema.CreateSchemaClass(new("Band")
		{
			Description = "Band that plays and produces music",
			VectorIndexType = VectorIndexType.HNSW,
			Vectorizer = Vectorizer.Text2VecContextionary,
			ReplicationConfig = new()
			{
				Factor = 2
			}
		});
		Assert.True(createStatus.HttpStatusCode == 200);

		void Verify(WeaviateClass weaviateClass)
		{
			Assert.Equal(2, weaviateClass.ReplicationConfig!.Factor);
		}

		Verify(createStatus.Result!);

		var schemaAfterCreate = Client.Schema.GetClass(new("Band"));
		Assert.True(schemaAfterCreate.HttpStatusCode == 200);

		Verify(schemaAfterCreate.Result!);
	}

	[Fact]
	public void CreateClassWithImplicitReplicationFactor()
	{
		Client.Schema.DeleteAllClasses();

		var createStatus = Client.Schema.CreateSchemaClass(new("Band")
		{
			Description = "Band that plays and produces music",
			VectorIndexType = VectorIndexType.HNSW,
			Vectorizer = Vectorizer.Text2VecContextionary,
			ReplicationConfig = new()
		});
		Assert.True(createStatus.HttpStatusCode == 200);

		void Verify(WeaviateClass weaviateClass)
		{
			Assert.Equal(1, weaviateClass.ReplicationConfig!.Factor);
		}

		Verify(createStatus.Result!);

		var schemaAfterCreate = Client.Schema.GetClass(new("Band"));
		Assert.True(schemaAfterCreate.HttpStatusCode == 200);

		Verify(schemaAfterCreate.Result!);
	}

	[Fact]
	public void CreateClassWithInvertedIndexConfigAndVectorIndexConfigAndShardConfig()
	{
		Client.Schema.DeleteAllClasses();

		var createStatus = Client.Schema.CreateSchemaClass(new("Band")
		{
			Description = "Band that plays and produces music",
			VectorIndexType = VectorIndexType.HNSW,
			Vectorizer = Vectorizer.Text2VecContextionary,
			InvertedIndexConfig =
				new()
				{
					Bm25 = new() { B = 0.777f, K1 = 1.777f },
					Stopwords = new()
					{
						Preset = "en",
						Additions = new[] { "star", "nebula" },
						Removals = new[] { "a", "the" }
					},
					CleanupIntervalSeconds = 300
				},
			VectorIndexConfig = new()
			{
				CleanupIntervalSeconds = 300,
				EfConstruction = 128,
				MaxConnections = 64,
				VectorCacheMaxObjects = 500000,
				Ef = -1,
				Skip = false,
				DynamicEfFactor = 8,
				DynamicEfMin = 100,
				DynamicEfMax = 500,
				FlatSearchCutoff = 40000,
				Distance = Distance.DotProduct,
				ProductQuantization = new()
				{
					Enabled = true,
					BitCompression = true,
					Segments = 4,
					Centroids = 8,
					Encoder = new()
					{
						Type = EncoderType.Tile,
						Distribution = DistributionType.Normal
					}
				}
			},
			ShardingConfig = new()
			{
				ActualCount = 1,
				ActualVirtualCount = 128,
				DesiredCount = 1,
				DesiredVirtualCount = 128,
				Function = "murmur3",
				Key = "_id",
				Strategy = "hash",
				VirtualPerPhysical = 128
			}
		});
		Assert.True(createStatus.HttpStatusCode == 200);

		void Verify(WeaviateClass weaviateClass)
		{
			Assert.Equal(0.777f, weaviateClass.InvertedIndexConfig!.Bm25!.B);
			Assert.Equal(1.777f, weaviateClass.InvertedIndexConfig.Bm25.K1);
			Assert.Equal("en", weaviateClass.InvertedIndexConfig.Stopwords!.Preset);
			Assert.Equal(new[] { "star", "nebula" }, weaviateClass.InvertedIndexConfig.Stopwords!.Additions);
			Assert.Equal(new[] { "a", "the" }, weaviateClass.InvertedIndexConfig.Stopwords!.Removals);
			Assert.Equal(300, weaviateClass.InvertedIndexConfig.CleanupIntervalSeconds);

			Assert.Equal(300, weaviateClass.VectorIndexConfig!.CleanupIntervalSeconds);
			Assert.Equal(128, weaviateClass.VectorIndexConfig.EfConstruction);
			Assert.Equal(64, weaviateClass.VectorIndexConfig.MaxConnections);
			Assert.Equal(500000, weaviateClass.VectorIndexConfig.VectorCacheMaxObjects);
			Assert.Equal(-1, weaviateClass.VectorIndexConfig.Ef);
			Assert.False(weaviateClass.VectorIndexConfig.Skip);
			Assert.Equal(8, weaviateClass.VectorIndexConfig.DynamicEfFactor);
			Assert.Equal(100, weaviateClass.VectorIndexConfig.DynamicEfMin);
			Assert.Equal(500, weaviateClass.VectorIndexConfig.DynamicEfMax);
			Assert.Equal(40000, weaviateClass.VectorIndexConfig.FlatSearchCutoff);
			Assert.Equal(Distance.DotProduct, weaviateClass.VectorIndexConfig.Distance);

			Assert.True(weaviateClass.VectorIndexConfig.ProductQuantization!.Enabled);
			Assert.True(weaviateClass.VectorIndexConfig.ProductQuantization!.BitCompression);
			Assert.Equal(4, weaviateClass.VectorIndexConfig.ProductQuantization!.Segments);
			Assert.Equal(8, weaviateClass.VectorIndexConfig.ProductQuantization!.Centroids);
			Assert.Equal(EncoderType.Tile, weaviateClass.VectorIndexConfig.ProductQuantization!.Encoder!.Type);
			Assert.Equal(DistributionType.Normal, weaviateClass.VectorIndexConfig.ProductQuantization!.Encoder!.Distribution);

			Assert.Equal(1, weaviateClass.ShardingConfig!.ActualCount);
			Assert.Equal(128, weaviateClass.ShardingConfig.ActualVirtualCount);
			Assert.Equal(1, weaviateClass.ShardingConfig.DesiredCount);
			Assert.Equal(128, weaviateClass.ShardingConfig.DesiredVirtualCount);
			Assert.Equal("murmur3", weaviateClass.ShardingConfig.Function);
			Assert.Equal("_id", weaviateClass.ShardingConfig.Key);
			Assert.Equal("hash", weaviateClass.ShardingConfig.Strategy);
			Assert.Equal(128, weaviateClass.ShardingConfig.VirtualPerPhysical);
		}

		Verify(createStatus.Result!);

		var schemaAfterCreate = Client.Schema.GetClass(new("Band"));
		Assert.True(schemaAfterCreate.HttpStatusCode == 200);

		Verify(schemaAfterCreate.Result!);
	}
}

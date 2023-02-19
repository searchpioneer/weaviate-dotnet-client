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

using SearchPioneer.Weaviate.Client.Api.Data.Model;
using SearchPioneer.Weaviate.Client.Api.Filters;
using SearchPioneer.Weaviate.Client.Api.Graph.Query.Fields;
using Xunit;

namespace SearchPioneer.Weaviate.Client.IntegrationTest.Api.Graph;

[Collection("Sequential")]
public class AggregateTests : TestBase
{
    [Fact]
    public void Aggregate()
    {
        CreateTestSchemaAndData(Client);

        var result = Client.Graph.Aggregate(new()
        {
            Class = CLASS_NAME_PIZZA,
            Fields = new[]
            {
                new Field
                {
                    Name = "meta",
                    Fields = new[]
                    {
                        new Field
                        {
                            Name = "count"
                        }
                    }
                }
            }
        });

        Assert.Single(result.Result.Data["Aggregate"]![CLASS_NAME_PIZZA]!.AsArray());
        Assert.Equal(4,
            result.Result.Data["Aggregate"]![CLASS_NAME_PIZZA]!.AsArray().First()!["meta"]!["count"]!.GetValue<int>());
    }

    [Fact]
    public void AggregateWithWhereFilter()
    {
        CreateTestSchemaAndData(Client);

        const string id = "6baed48e-2afe-4be4-a09d-b00a955d96ee";

        var batch = Client.Batch.CreateObjects(new(new WeaviateObject
            {
                Id = id,
                Class = CLASS_NAME_PIZZA,
                Properties = new()
                {
                    { "name", "JustPizza" },
                    { "description", "pizza with id" }
                }
            }
        ));
        Assert.True(batch.HttpStatusCode == 200);

        var result = Client.Graph.Aggregate(new()
        {
            Class = CLASS_NAME_PIZZA,
            Where = new()
            {
                Path = new[] { "id" },
                Operator = Operator.Equal,
                ValueString = id
            },
            Fields = new[]
            {
                new Field
                {
                    Name = "meta",
                    Fields = new[]
                    {
                        new Field
                        {
                            Name = "count"
                        }
                    }
                }
            }
        });

        Assert.Single(result.Result.Data["Aggregate"]![CLASS_NAME_PIZZA]!.AsArray());
        Assert.Equal(1,
            result.Result.Data["Aggregate"]![CLASS_NAME_PIZZA]!.AsArray().First()!["meta"]!["count"]!.GetValue<int>());
    }

    [Fact]
    public void AggregateWithGroupedByAndWhere()
    {
        CreateTestSchemaAndData(Client);

        const string id = "6baed48e-2afe-4be4-a09d-b00a955d96ee";

        var batch = Client.Batch.CreateObjects(new(new WeaviateObject
            {
                Id = id,
                Class = CLASS_NAME_PIZZA,
                Properties = new()
                {
                    { "name", "JustPizza" },
                    { "description", "pizza with id" }
                }
            }
        ));
        Assert.True(batch.HttpStatusCode == 200);

        var result = Client.Graph.Aggregate(new()
        {
            Class = CLASS_NAME_PIZZA,
            Where = new()
            {
                Path = new[] { "id" },
                Operator = Operator.Equal,
                ValueString = id
            },
            GroupBy = "name",
            Fields = new[]
            {
                new Field
                {
                    Name = "meta",
                    Fields = new[]
                    {
                        new Field
                        {
                            Name = "count"
                        }
                    }
                }
            }
        });

        Assert.Single(result.Result.Data["Aggregate"]![CLASS_NAME_PIZZA]!.AsArray());
        Assert.Equal(1,
            result.Result.Data["Aggregate"]![CLASS_NAME_PIZZA]!.AsArray().First()!["meta"]!["count"]!.GetValue<int>());
    }

    [Fact]
    public void AggregateWithGroupedBy()
    {
        CreateTestSchemaAndData(Client);

        const string id = "6baed48e-2afe-4be4-a09d-b00a955d96ee";

        var batch = Client.Batch.CreateObjects(new(new WeaviateObject
            {
                Id = id,
                Class = CLASS_NAME_PIZZA,
                Properties = new()
                {
                    { "name", "JustPizza" },
                    { "description", "pizza with id" }
                }
            }
        ));
        Assert.True(batch.HttpStatusCode == 200);

        var result = Client.Graph.Aggregate(new()
        {
            Class = CLASS_NAME_PIZZA,
            GroupBy = "name",
            Fields = new[]
            {
                new Field
                {
                    Name = "meta",
                    Fields = new[]
                    {
                        new Field
                        {
                            Name = "count"
                        }
                    }
                }
            }
        });

        Assert.Equal(5, result.Result.Data["Aggregate"]![CLASS_NAME_PIZZA]!.AsArray().Count);
        Assert.Equal(1,
            result.Result.Data["Aggregate"]![CLASS_NAME_PIZZA]!.AsArray().First()!["meta"]!["count"]!.GetValue<int>());
    }

    [Fact]
    public void AggregateWithNearVector()
    {
        CreateTestSchemaAndData(Client);

        var vectorResult = Client.Graph.Get(new()
        {
            Class = CLASS_NAME_PIZZA,
            Fields = new[]
            {
                AdditionalField.Vector.AsAdditionalField()
            }
        });

        var vector = vectorResult.Result.Data["Get"]![CLASS_NAME_PIZZA]!.AsArray().First()!["_additional"]!["vector"]!
            .AsArray()
            .Select(v => v!.GetValue<float>()).ToArray();

        var result = Client.Graph.Aggregate(new()
        {
            Class = CLASS_NAME_PIZZA,
            NearVector = new()
            {
                Vector = vector,
                Certainty = 0.7f
            },
            Fields = new[]
            {
                new Field
                {
                    Name = "meta",
                    Fields = new Field[]
                    {
                        "count"
                    }
                }
            }
        });

        Assert.Single(result.Result.Data["Aggregate"]![CLASS_NAME_PIZZA]!.AsArray());
        Assert.Equal(4,
            result.Result.Data["Aggregate"]![CLASS_NAME_PIZZA]!.AsArray().First()!["meta"]!["count"]!.GetValue<int>());
    }

    [Fact]
    public void AggregateWithNearObjectAndCertainty()
    {
        CreateTestSchemaAndData(Client);

        var vectorResult = Client.Graph.Get(new()
        {
            Class = CLASS_NAME_PIZZA,
            Fields = new[]
            {
                AdditionalField.Id.AsAdditionalField()
            }
        });

        var id = vectorResult.Result.Data["Get"]![CLASS_NAME_PIZZA]!.AsArray().First()!["_additional"]!["id"]!
            .GetValue<string>();

        var result = Client.Graph.Aggregate(new()
        {
            Class = CLASS_NAME_PIZZA,
            NearObject = new()
            {
                Id = id,
                Certainty = 0.7f
            },
            Fields = new[]
            {
                new Field
                {
                    Name = "meta",
                    Fields = new[]
                    {
                        new Field
                        {
                            Name = "count"
                        }
                    }
                }
            }
        });

        Assert.Single(result.Result.Data["Aggregate"]![CLASS_NAME_PIZZA]!.AsArray());
        Assert.Equal(4,
            result.Result.Data["Aggregate"]![CLASS_NAME_PIZZA]!.AsArray().First()!["meta"]!["count"]!.GetValue<int>());
    }

    [Fact]
    public void AggregateWithNearObjectAndDistance()
    {
        CreateTestSchemaAndData(Client);

        var vectorResult = Client.Graph.Get(new()
        {
            Class = CLASS_NAME_PIZZA,
            Fields = new[]
            {
                "id".AsAdditional()
            }
        });

        var id = vectorResult.Result.Data["Get"]![CLASS_NAME_PIZZA]!.AsArray().First()!["_additional"]!["id"]!
            .GetValue<string>();

        var result = Client.Graph.Aggregate(new()
        {
            Class = CLASS_NAME_PIZZA,
            NearObject = new()
            {
                Id = id,
                Distance = 0.3f
            },
            Fields = new[]
            {
                new Field
                {
                    Name = "meta",
                    Fields = new[]
                    {
                        new Field
                        {
                            Name = "count"
                        }
                    }
                }
            }
        });

        Assert.Single(result.Result.Data["Aggregate"]![CLASS_NAME_PIZZA]!.AsArray());
        Assert.Equal(4,
            result.Result.Data["Aggregate"]![CLASS_NAME_PIZZA]!.AsArray().First()!["meta"]!["count"]!.GetValue<int>());
    }

    [Fact]
    public void AggregateWithNearTextAndCertainty()
    {
        CreateTestSchemaAndData(Client);

        var result = Client.Graph.Aggregate(new()
        {
            Class = CLASS_NAME_PIZZA,
            NearText = new()
            {
                Concepts = new[] { CLASS_NAME_PIZZA },
                Certainty = 0.7f
            },
            Fields = new[]
            {
                new Field
                {
                    Name = "meta",
                    Fields = new[]
                    {
                        new Field
                        {
                            Name = "count"
                        }
                    }
                }
            }
        });

        Assert.Single(result.Result.Data["Aggregate"]![CLASS_NAME_PIZZA]!.AsArray());
        Assert.Equal(4,
            result.Result.Data["Aggregate"]![CLASS_NAME_PIZZA]!.AsArray().First()!["meta"]!["count"]!.GetValue<int>());
    }

    [Fact]
    public void AggregateWithNearTextAndDistance()
    {
        CreateTestSchemaAndData(Client);

        var result = Client.Graph.Aggregate(new()
        {
            Class = CLASS_NAME_PIZZA,
            NearText = new()
            {
                Concepts = new[] { CLASS_NAME_PIZZA },
                Distance = 0.6f
            },
            Fields = new[]
            {
                new Field
                {
                    Name = "meta",
                    Fields = new[]
                    {
                        new Field
                        {
                            Name = "count"
                        }
                    }
                }
            }
        });

        Assert.Single(result.Result.Data["Aggregate"]![CLASS_NAME_PIZZA]!.AsArray());
        Assert.Equal(4,
            result.Result.Data["Aggregate"]![CLASS_NAME_PIZZA]!.AsArray().First()!["meta"]!["count"]!.GetValue<int>());
    }

    [Fact]
    public void AggregateWithObjectLimitAndCertainty()
    {
        CreateTestSchemaAndData(Client);

        var result = Client.Graph.Aggregate(new()
        {
            Class = CLASS_NAME_PIZZA,
            NearText = new()
            {
                Concepts = new[] { CLASS_NAME_PIZZA },
                Certainty = 0.7f
            },
            ObjectLimit = 1,
            Fields = new[]
            {
                new Field
                {
                    Name = "meta",
                    Fields = new[]
                    {
                        new Field
                        {
                            Name = "count"
                        }
                    }
                }
            }
        });

        Assert.Single(result.Result.Data["Aggregate"]![CLASS_NAME_PIZZA]!.AsArray());
        Assert.Equal(1,
            result.Result.Data["Aggregate"]![CLASS_NAME_PIZZA]!.AsArray().First()!["meta"]!["count"]!.GetValue<int>());
    }

    [Fact]
    public void AggregateWithObjectLimitAndDistance()
    {
        CreateTestSchemaAndData(Client);

        var result = Client.Graph.Aggregate(new()
        {
            Class = CLASS_NAME_PIZZA,
            NearText = new()
            {
                Concepts = new[] { CLASS_NAME_PIZZA },
                Distance = 0.3f
            },
            ObjectLimit = 1,
            Fields = new[]
            {
                new Field
                {
                    Name = "meta",
                    Fields = new[]
                    {
                        new Field
                        {
                            Name = "count"
                        }
                    }
                }
            }
        });

        Assert.Single(result.Result.Data["Aggregate"]![CLASS_NAME_PIZZA]!.AsArray());
        Assert.Equal(1,
            result.Result.Data["Aggregate"]![CLASS_NAME_PIZZA]!.AsArray().First()!["meta"]!["count"]!.GetValue<int>());
    }
}
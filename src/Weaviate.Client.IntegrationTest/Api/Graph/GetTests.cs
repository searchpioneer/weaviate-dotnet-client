﻿// Copyright (C) 2023 Search Pioneer - https://www.searchpioneer.com
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
using SearchPioneer.Weaviate.Client.Api.Graph.Query.Arguments;
using SearchPioneer.Weaviate.Client.Api.Graph.Query.Fields;
using SearchPioneer.Weaviate.Client.Api.Graph.Response;
using Xunit;

namespace SearchPioneer.Weaviate.Client.IntegrationTest.Api.Graph;

[Collection("Sequential")]
public class GetTests : TestBase
{
    [Fact]
    public void Get()
    {
        CreateTestSchemaAndData(Client);

        var result = Client.Graph.Get(new()
        {
            Class = CLASS_NAME_PIZZA,
            Fields = "name".AsFields()
        });

        Assert.Equal(4, result.Result.Data["Get"]![CLASS_NAME_PIZZA]!.AsArray().Count);
    }

    [Fact]
    public void NearObjectAndCertainty()
    {
        CreateTestSchemaAndData(Client);

        const string id = "6baed48e-2afe-4be4-a09d-b00a955d962b";

        var newSoup = Client.Data.Create(new(CLASS_NAME_SOUP)
        {
            Id = id,
            Properties = new()
            {
                { "name", "JustSoup" },
                { "description", "soup with id" }
            }
        });
        Assert.True(newSoup.HttpStatusCode == 200);

        var result = Client.Graph.Get(new()
        {
            Class = CLASS_NAME_SOUP,
            NearObject = new()
            {
                Id = id,
                Certainty = 0.99f
            },
            Fields = new[]
            {
                "name".AsField(),
                AdditionalField.Certainty.AsAdditionalField()
            }
        });

        Assert.Single(result.Result.Data["Get"]![CLASS_NAME_SOUP]!.AsArray());
    }

    [Fact]
    public void NearObjectAndDistance()
    {
        CreateTestSchemaAndData(Client);

        const string id = "6baed48e-2afe-4be4-a09d-b00a955d962b";

        var newSoup = Client.Data.Create(new(CLASS_NAME_SOUP)
        {
            Id = id,
            Properties = new()
            {
                { "name", "JustSoup" },
                { "description", "soup with id" }
            }
        });
        Assert.True(newSoup.HttpStatusCode == 200);

        var result = Client.Graph.Get(new()
        {
            Class = CLASS_NAME_SOUP,
            NearObject = new()
            {
                Id = id,
                Distance = 0.01f
            },
            Fields = new[]
            {
                "name".AsField(),
                AdditionalField.Certainty.AsAdditionalField()
            }
        });

        Assert.Single(result.Result.Data["Get"]![CLASS_NAME_SOUP]!.AsArray());
    }

    [Fact]
    public void BM25()
    {
        CreateTestSchemaAndData(Client);

        var result = Client.Graph.Get(new()
        {
            Class = CLASS_NAME_PIZZA,
            BM25 = new()
            {
                Query = "innovation",
                Properties = new[] { "description" }
            },
            Fields = new[]
            {
                "description".AsField(),
                AdditionalField.Id.AsAdditionalField()
            }
        });

        Assert.Single(result.Result.Data["Get"]![CLASS_NAME_PIZZA]!.AsArray());
        Assert.Contains("innovation",
            result.Result.Data["Get"]![CLASS_NAME_PIZZA]!.AsArray().First()!["description"]!.GetValue<string>());
    }

    [Fact]
    public void Hybrid()
    {
        CreateTestSchemaAndData(Client);

        var result = Client.Graph.Get(new()
        {
            Class = CLASS_NAME_PIZZA,
            Hybrid = new()
            {
                Query = "some say revolution",
                Alpha = 0.8f
            },
            Fields = new[]
            {
                "description".AsField(),
                AdditionalField.Id.AsAdditionalField()
            }
        });

        Assert.NotNull(result.Result.Data["Get"]![CLASS_NAME_PIZZA]!.AsArray());
        Assert.Equal(4, result.Result.Data["Get"]![CLASS_NAME_PIZZA]!.AsArray().Count);
    }

    [Fact]
    public void NearTextAndCertainty()
    {
        CreateTestSchemaAndData(Client);

        var result = Client.Graph.Get(new()
        {
            Class = CLASS_NAME_PIZZA,
            NearText = new()
            {
                Concepts = new[] { "some say revolution" },
                MoveAwayFrom = new()
                {
                    Concepts = new[] { "Universally" },
                    Force = 0.8f
                },
                Certainty = 0.8f
            },
            Fields = new[]
            {
                "name".AsField(),
                AdditionalField.Certainty.AsAdditionalField()
            }
        });
        Assert.True(result.HttpStatusCode == 200);

        Assert.Single(result.Result.Data["Get"]![CLASS_NAME_PIZZA]!.AsArray());
    }

    [Fact]
    public void NearTextAndDistance()
    {
        CreateTestSchemaAndData(Client);

        var result = Client.Graph.Get(new()
        {
            Class = CLASS_NAME_PIZZA,
            NearText = new()
            {
                Concepts = new[] { "some say revolution" },
                MoveAwayFrom = new()
                {
                    Concepts = new[] { "Universally" },
                    Force = 0.8f
                },
                Distance = 0.4f
            },
            Fields = new[]
            {
                "name".AsField(),
                AdditionalField.Certainty.AsAdditionalField()
            }
        });
        Assert.True(result.HttpStatusCode == 200);

        Assert.Single(result.Result.Data["Get"]![CLASS_NAME_PIZZA]!.AsArray());
    }

    [Fact]
    public void NearTextAndMoveParamsAndCertainty()
    {
        CreateTestSchemaAndData(Client);

        const string id1 = "6baed48e-2afe-4be4-a09d-b00a955d962b";
        const string id2 = "6baed48e-2afe-4be4-a09d-b00a955d962a";

        var batch = Client.Batch.CreateObjects(new(new WeaviateObject
            {
                Id = id1,
                Class = CLASS_NAME_PIZZA,
                Properties = new()
                {
                    { "name", "JustPizza1" },
                    { "description", "Universally pizza with id" }
                }
            },
            new WeaviateObject
            {
                Id = id2,
                Class = CLASS_NAME_PIZZA,
                Properties = new()
                {
                    { "name", "JustPizza2" },
                    { "description", "Universally pizza with some other id" }
                }
            }
        ));
        Assert.True(batch.HttpStatusCode == 200);

        var result = Client.Graph.Get(new()
        {
            Class = CLASS_NAME_PIZZA,
            NearText = new()
            {
                Concepts = new[] { "Universally pizza with id" },
                MoveAwayFrom = new()
                {
                    Objects = new ObjectMove[]
                    {
                        new()
                        {
                            Id = id1
                        }
                    },
                    Force = 0.9f
                },
                MoveTo = new()
                {
                    Objects = new ObjectMove[]
                    {
                        new()
                        {
                            Id = id2
                        }
                    },
                    Force = 0.9f
                },
                Certainty = 0.4f
            },
            Fields = new[]
            {
                "name".AsField(),
                AdditionalField.Certainty.AsAdditionalField()
            }
        });

        Assert.Equal(6, result.Result.Data["Get"]![CLASS_NAME_PIZZA]!.AsArray().Count);
    }

    [Fact]
    public void NearTextAndLimitAndCertainty()
    {
        CreateTestSchemaAndData(Client);

        var result = Client.Graph.Get(new()
        {
            Class = CLASS_NAME_PIZZA,
            NearText = new()
            {
                Concepts = new[] { "some say revolution" },
                Certainty = 0.8f
            },
            Limit = 1,
            Fields = new[]
            {
                "name".AsField(),
                AdditionalField.Certainty.AsAdditionalField()
            }
        });
        Assert.True(result.HttpStatusCode == 200);

        Assert.Single(result.Result.Data["Get"]![CLASS_NAME_PIZZA]!.AsArray());
    }

    [Fact]
    public void NearTextAndLimitAndDistance()
    {
        CreateTestSchemaAndData(Client);

        var result = Client.Graph.Get(new()
        {
            Class = CLASS_NAME_PIZZA,
            NearText = new()
            {
                Concepts = new[] { "some say revolution" },
                Distance = 0.4f
            },
            Limit = 1,
            Fields = new[]
            {
                "name".AsField(),
                AdditionalField.Certainty.AsAdditionalField()
            }
        });
        Assert.True(result.HttpStatusCode == 200);

        Assert.Single(result.Result.Data["Get"]![CLASS_NAME_PIZZA]!.AsArray());
    }

    [Fact]
    public void WhereByFieldTokenizedProperty()
    {
        CreateTestSchemaAndData(Client);

        void AssertCount(int count, ApiResponse<GraphResponse> result)
        {
            Assert.Equal(count, result.Result.Data["Get"]![CLASS_NAME_PIZZA]!.AsArray().Count);
        }

        AssertCount(1, Client.Graph.Get(new()
        {
            Class = CLASS_NAME_PIZZA,
            Where = new()
            {
                Path = new[] { "name" },
                Operator = Operator.Equal,
                ValueString = "Frutti di Mare"
            },
            Fields = new[]
            {
                "name".AsField()
            }
        }));

        AssertCount(0, Client.Graph.Get(new()
        {
            Class = CLASS_NAME_PIZZA,
            Where = new()
            {
                Path = new[] { "name" },
                Operator = Operator.Equal,
                ValueString = "Frutti"
            },
            Fields = new[]
            {
                "name".AsField()
            }
        }));

        AssertCount(1, Client.Graph.Get(new()
        {
            Class = CLASS_NAME_PIZZA,
            Where = new()
            {
                Path = new[] { "description" },
                Operator = Operator.Equal,
                ValueText = "Universally accepted to be the best pizza ever created."
            },
            Fields = new[]
            {
                "name".AsField()
            }
        }));

        AssertCount(1, Client.Graph.Get(new()
        {
            Class = CLASS_NAME_PIZZA,
            Where = new()
            {
                Path = new[] { "description" },
                Operator = Operator.Equal,
                ValueText = "Universally"
            },
            Fields = new[]
            {
                "name".AsField()
            }
        }));
    }

    [Fact]
    public void WhereByDate()
    {
        CreateTestSchemaAndData(Client);

        var result = Client.Graph.Get(new()
        {
            Class = CLASS_NAME_PIZZA,
            Where = new()
            {
                Path = new[] { "bestBefore" },
                Operator = Operator.GreaterThan,
                ValueDate = DateTime.Parse("2022-02-01T00:00:00+0100")
            },
            Fields = new[]
            {
                "name".AsField()
            }
        });

        Assert.Equal(3, result.Result.Data["Get"]![CLASS_NAME_PIZZA]!.AsArray().Count);
    }

    [Fact]
    public void Group()
    {
        CreateTestSchemaAndData(Client);

        var result = Client.Graph.Get(new()
        {
            Class = CLASS_NAME_SOUP,
            Group = new()
            {
                Type = GroupType.Merge,
                Force = 1.0f
            },
            Limit = 7,
            Fields = "name".AsFields()
        });

        Assert.Single(result.Result.Data["Get"]![CLASS_NAME_SOUP]!.AsArray());
    }

    [Fact]
    public void Sort()
    {
        CreateTestSchemaAndData(Client);

        void AssertOrder(IReadOnlyList<string> order, ApiResponse<GraphResponse> result)
        {
            var pizzas = result.Result.Data["Get"]![CLASS_NAME_PIZZA]!.AsArray();
            Assert.Equal(order.Count, pizzas.Count);
            for (var index = 0; index < pizzas.Count; index++)
            {
                var pizza = pizzas[(Index)index]!;
                Assert.Equal(order[index], pizza["name"]!.GetValue<string>());
            }
        }

        AssertOrder(new[] { "Quattro Formaggi", "Hawaii", "Frutti di Mare", "Doener" },
            Client.Graph.Get(new()
            {
                Class = CLASS_NAME_PIZZA,
                Sorts = new[]
                {
                    new Sort
                    {
                        Path = new[] { "name" },
                        Order = SortOrder.Desc
                    }
                },
                Fields = "name".AsFields()
            }));

        AssertOrder(new[] { "Hawaii", "Doener", "Quattro Formaggi", "Frutti di Mare" },
            Client.Graph.Get(new()
            {
                Class = CLASS_NAME_PIZZA,
                Sorts = new[]
                {
                    new Sort
                    {
                        Path = new[] { "price" },
                        Order = SortOrder.Asc
                    }
                },
                Fields = "name".AsFields()
            }));

        AssertOrder(new[] { "Quattro Formaggi", "Hawaii", "Frutti di Mare", "Doener" },
            Client.Graph.Get(new()
            {
                Class = CLASS_NAME_PIZZA,
                Sorts = new[]
                {
                    new Sort
                    {
                        Path = new[] { "name" },
                        Order = SortOrder.Desc
                    },
                    new Sort
                    {
                        Path = new[] { "price" },
                        Order = SortOrder.Asc
                    }
                },
                Fields = "name".AsFields()
            }));

        // SortArgument byNameDesc = client.graphQL().arguments().sortArgBuilder()
        //     .path(new String[]{ "name" })
        //     .order(SortOrder.desc)
        //     .build();
        // String[] expectedByNameDesc = new String[]{"Quattro Formaggi", "Hawaii", "Frutti di Mare", "Doener"};
        // SortArgument byPriceAsc = client.graphQL().arguments().sortArgBuilder()
        //     .path(new String[]{ "price" })
        //     .order(SortOrder.asc)
        //     .build();
        // String[] expectedByPriceAsc = new String[]{ "Hawaii", "Doener", "Quattro Formaggi", "Frutti di Mare" };
        // // when
        // testGenerics.createTestSchemaAndData(client);
        // Result<GraphQLResponse> resultByNameDesc = client.graphQL().get()
        //     .withClassName(CLASS_NAME_PIZZA)
        //     .withSort(byNameDesc)
        //     .withFields(name).run();
        // Result<GraphQLResponse> resultByDescriptionAsc = client.graphQL().get()
        //     .withClassName(CLASS_NAME_PIZZA)
        //     .withSort(byPriceAsc)
        //     .withFields(name).run();
        // Result<GraphQLResponse> resultByNameDescByPriceAsc = client.graphQL().get()
        //     .withClassName(CLASS_NAME_PIZZA)
        //     .withSort(byNameDesc, byPriceAsc)
        //     .withFields(name).run();
        // testGenerics.cleanupWeaviate(client);
        // // then
        // expectPizzaNamesOrder(resultByNameDesc, expectedByNameDesc);
        // expectPizzaNamesOrder(resultByDescriptionAsc, expectedByPriceAsc);
        // expectPizzaNamesOrder(resultByNameDescByPriceAsc, expectedByNameDesc);
    }
}
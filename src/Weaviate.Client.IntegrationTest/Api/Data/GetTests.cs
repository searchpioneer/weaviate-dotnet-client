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
using SearchPioneer.Weaviate.Client.Api.Schema.Model;
using Xunit;

namespace SearchPioneer.Weaviate.Client.IntegrationTest.Api.Data;

[Collection("Sequential")]
public class GetTests : TestBase
{
    [Fact]
    public void GetActionsThings()
    {
        CreateWeaviateTestSchemaFood(Client);

        var margherita = Client.Data.Create(new(CLASS_NAME_PIZZA)
        {
            Properties = new()
            {
                { "name", "Margherita" },
                { "description", "plain" }
            }
        });
        Assert.True(margherita.HttpStatusCode == 200);

        var pepperoni = Client.Data.Create(new(CLASS_NAME_PIZZA)
        {
            Properties = new()
            {
                { "name", "Pepperoni" },
                { "description", "meat" }
            }
        });
        Assert.True(pepperoni.HttpStatusCode == 200);

        var chicken = Client.Data.Create(new(CLASS_NAME_SOUP)
        {
            Properties = new()
            {
                { "name", "Chicken" },
                { "description", "plain" }
            }
        });
        Assert.True(chicken.HttpStatusCode == 200);

        var tofu = Client.Data.Create(new(CLASS_NAME_SOUP)
        {
            Properties = new()
            {
                { "name", "Tofu" },
                { "description", "vegetarian" }
            }
        });
        Assert.True(tofu.HttpStatusCode == 200);

        var allResults = Client.Data.Get(new());
        var allResultsConvenience = Client.Data.GetAll();

        var singleResult = Client.Data.Get(new()
        {
            Limit = 1
        });

        Assert.Equal(4, allResults.Result.Length);
        Assert.Equal(4, allResultsConvenience.Result.Length);
        Assert.Single(singleResult.Result);
    }

    [Fact]
    public void GetWithAdditional()
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
            }
        });
        Assert.True(hawaiian.HttpStatusCode == 200);

        var chickensoup = Client.Data.Create(new(CLASS_NAME_SOUP)
        {
            Id = soupId,
            Properties = new()
            {
                { "name", "ChickenSoup" },
                { "description", "Used by humans when their inferior genetics are attacked by microscopic organisms." }
            }
        });
        Assert.True(chickensoup.HttpStatusCode == 200);

        var pizza = Client.Data.Get(new()
        {
            Id = pizzaId,
            Class = CLASS_NAME_PIZZA,
            Additional = new()
            {
                "classification", // TODO! can these be strings somewhere?
                "nearestNeighbors",
                "vector"
            }
        });
        Assert.True(pizza.HttpStatusCode == 200);
        Assert.NotNull(pizza.Result.Single().Additional);
        Assert.Equal(pizzaId, pizza.Result.Single().Id);
        Assert.Null(pizza.Result.Single().Additional!["classification"]);
        Assert.NotNull(pizza.Result.Single().Additional!["nearestNeighbors"]);
        Assert.NotNull(pizza.Result.Single().Vector);

        var soup = Client.Data.Get(new()
        {
            Id = soupId,
            Class = CLASS_NAME_SOUP,
            Additional = new()
            {
                "classification",
                "nearestNeighbors",
                "interpretation",
                "vector"
            }
        });
        Assert.True(soup.HttpStatusCode == 200);
        Assert.Equal(soupId, soup.Result.Single().Id);
        Assert.Null(soup.Result.Single().Additional!["classification"]);
        Assert.NotNull(soup.Result.Single().Additional!["nearestNeighbors"]);
        Assert.NotNull(soup.Result.Single().Additional!["interpretation"]);
        Assert.NotNull(soup.Result.Single().Vector);
    }

    [Fact]
    public void GetWithAdditionalError()
    {
        CreateWeaviateTestSchemaFood(Client);

        var pizzaId = "abefd256-8574-442b-9293-9205193737ee";
        var hawaiian = Client.Data.Create(new(CLASS_NAME_PIZZA)
        {
            Id = pizzaId,
            Properties = new()
            {
                { "name", "Hawaii" },
                { "description", "Universally accepted to be the best pizza ever created." }
            }
        });
        Assert.True(hawaiian.HttpStatusCode == 200);

        var get = Client.Data.Get(new()
        {
            Id = pizzaId,
            Class = CLASS_NAME_PIZZA,
            Additional = new()
            {
                "featureProjection"
            }
        });

        Assert.True(get.HttpStatusCode == 500);
        Assert.Contains(
            "get extend: unknown capability: featureProjection",
            get.Error!.Error!.Select(e => e.Message));
    }

    [Fact]
    public void GetWithVector()
    {
        Client.Schema.DeleteAllClasses();

        const string @class = "ClassCustomVector";
        var schema = Client.Schema.CreateClass(new(@class)
        {
            Description = "Class with custom vector",
            Vectorizer = Vectorizer.None,
            Properties = new Property[]
            {
                new()
                {
                    Name = "foo",
                    DataType = new[] { DataType.String }
                }
            }
        });
        Assert.True(schema.HttpStatusCode == 200);

        var id = "abefd256-8574-442b-9293-9205193737ee";
        var floats = new[]
        {
            -0.26736435f, -0.112380296f, 0.29648793f, 0.39212644f, 0.0033650293f, -0.07112332f, 0.07513781f, 0.22459874f
        };

        var create = Client.Data.Create(new(@class)
        {
            Id = id,
            Properties = new()
            {
                { "foo", "bar" }
            },
            Vector = floats
        });

        var get = Client.Data.Get(new()
        {
            Id = id,
            Class = @class,
            Additional = new()
            {
                "vector"
            }
        });

        Assert.True(get.HttpStatusCode == 200);
        Assert.Equal(floats.Length, get.Result.Single().Vector!.Length);
    }

    [Fact]
    public void GetUsingClassParameter()
    {
        CreateTestSchemaAndData(Client);

        var all = Client.Data.GetAll();
        Assert.True(all.HttpStatusCode == 200);
        Assert.Equal(6, all.Result.Length);

        var pizzas = Client.Data.Get(new()
        {
            Class = CLASS_NAME_PIZZA
        });
        Assert.True(pizzas.HttpStatusCode == 200);
        Assert.Equal(4, pizzas.Result.Length);

        var soups = Client.Data.Get(new()
        {
            Class = CLASS_NAME_SOUP
        });
        Assert.True(soups.HttpStatusCode == 200);
        Assert.Equal(2, soups.Result.Length);
    }
}
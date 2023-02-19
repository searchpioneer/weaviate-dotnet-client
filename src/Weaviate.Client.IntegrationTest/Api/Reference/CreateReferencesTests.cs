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

using System.Text.Json;
using SearchPioneer.Weaviate.Client.Api.Data.Model;
using Xunit;

namespace SearchPioneer.Weaviate.Client.IntegrationTest.Api.Reference;

[Collection("Sequential")]
public class CreateReferencesTests : TestBase
{
    [Fact]
    public void CreateWithReferenceCreate()
    {
        CreateWeaviateTestSchemaFoodWithReferenceProperty(Client);

        var pizzaId = "abefd256-8574-442b-9293-9205193737ee";
        var soupId = "565da3b6-60b3-40e5-ba21-e6bfe5dbba91";

        CreateAndVerifyReferences(pizzaId, soupId);
    }

    [Fact]
    public void CreateWithReferenceReplace()
    {
        CreateWeaviateTestSchemaFoodWithReferenceProperty(Client);

        var pizzaId = "abefd256-8574-442b-9293-9205193737ee";
        var soupId = "565da3b6-60b3-40e5-ba21-e6bfe5dbba91";

        CreateAndVerifyReferences(pizzaId, soupId);

        var soupReference = Client.Reference.Replace(new()
        {
            Id = pizzaId,
            Class = CLASS_NAME_PIZZA,
            ReferenceProperty = "otherFoods",
            ReferencePayload = new[] { Client.Reference.Reference(CLASS_NAME_PIZZA, pizzaId) }
        });
        Assert.True(soupReference.HttpStatusCode == 200);

        var pizzaReference = Client.Reference.Replace(new()
        {
            Id = soupId,
            Class = CLASS_NAME_SOUP,
            ReferenceProperty = "otherFoods",
            ReferencePayload = new[] { Client.Reference.Reference(CLASS_NAME_SOUP, soupId) }
        });
        Assert.True(pizzaReference.HttpStatusCode == 200);

        var pizzaRef = Client.Data.Get(new()
        {
            Id = pizzaId,
            Class = CLASS_NAME_PIZZA
        });
        Assert.True(pizzaRef.HttpStatusCode == 200);
        CheckReference(pizzaRef.Result, CLASS_NAME_PIZZA, pizzaId);

        var soupRef = Client.Data.Get(new()
        {
            Id = soupId,
            Class = CLASS_NAME_SOUP
        });
        Assert.True(soupRef.HttpStatusCode == 200);
        CheckReference(soupRef.Result, CLASS_NAME_SOUP, soupId);
    }

    [Fact]
    public void CreateWithReferenceDelete()
    {
        CreateWeaviateTestSchemaFoodWithReferenceProperty(Client);

        var pizzaId = "abefd256-8574-442b-9293-9205193737ee";
        var soupId = "565da3b6-60b3-40e5-ba21-e6bfe5dbba91";

        CreateAndVerifyReferences(pizzaId, soupId);

        var soupReference = Client.Reference.Delete(new()
        {
            Id = pizzaId,
            Class = CLASS_NAME_PIZZA,
            ReferenceProperty = "otherFoods",
            ReferencePayload = Client.Reference.Reference(CLASS_NAME_SOUP, soupId)
        });
        Assert.True(soupReference.HttpStatusCode == 204);

        var pizzaReference = Client.Reference.Delete(new()
        {
            Id = soupId,
            Class = CLASS_NAME_SOUP,
            ReferenceProperty = "otherFoods",
            ReferencePayload = Client.Reference.Reference(CLASS_NAME_PIZZA, pizzaId)
        });
        Assert.True(pizzaReference.HttpStatusCode == 204);

        var pizzaRef = Client.Data.Get(new()
        {
            Id = pizzaId,
            Class = CLASS_NAME_PIZZA
        });
        Assert.True(pizzaRef.HttpStatusCode == 200);
        Assert.Empty(((JsonElement)pizzaRef.Result.Single().Properties!["otherFoods"])
            .EnumerateArray().Select(a => a).ToArray());

        var soupRef = Client.Data.Get(new()
        {
            Id = soupId,
            Class = CLASS_NAME_SOUP
        });
        Assert.True(soupRef.HttpStatusCode == 200);
        Assert.Empty(((JsonElement)soupRef.Result.Single().Properties!["otherFoods"])
            .EnumerateArray().Select(a => a).ToArray());
    }

    [Fact]
    public void CreateWithAddReferenceUsingProperties()
    {
        CreateWeaviateTestSchemaFoodWithReferenceProperty(Client);

        var pizzaId = "abefd256-8574-442b-9293-9205193737ee";
        var beaconId = "565da3b6-60b3-40e5-ba21-e6bfe5dbba92";

        var beacon = Client.Data.Create(new(CLASS_NAME_PIZZA)
        {
            Id = beaconId,
            Properties = new()
            {
                { "name", "Hawaii" },
                { "description", "Universally accepted to be the best pizza ever created." }
            }
        });
        Assert.True(beacon.HttpStatusCode == 200);

        var pizza = Client.Data.Create(new(CLASS_NAME_PIZZA)
        {
            Id = pizzaId,
            Properties = new()
            {
                { "name", "Hawaii" },
                { "description", "Universally accepted to be the best pizza ever created." },
                { "otherFoods", new[] { Client.Reference.Reference(CLASS_NAME_PIZZA, beaconId) } }
            }
        });
        Assert.True(pizza.HttpStatusCode == 200);

        var pizzaRef = Client.Data.Get(new()
        {
            Id = pizzaId,
            Class = CLASS_NAME_PIZZA
        });
        Assert.True(pizzaRef.HttpStatusCode == 200);
        CheckReference(pizzaRef.Result, CLASS_NAME_PIZZA, beaconId);
    }

    private void CreateAndVerifyReferences(string pizzaId, string soupId)
    {
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

        var soupReference = Client.Reference.Create(new()
        {
            Id = pizzaId,
            Class = CLASS_NAME_PIZZA,
            ReferenceProperty = "otherFoods",
            ReferencePayload = Client.Reference.Reference(CLASS_NAME_SOUP, soupId)
        });
        Assert.True(soupReference.HttpStatusCode == 200);

        var pizzaReference = Client.Reference.Create(new()
        {
            Id = soupId,
            Class = CLASS_NAME_SOUP,
            ReferenceProperty = "otherFoods",
            ReferencePayload = Client.Reference.Reference(CLASS_NAME_PIZZA, pizzaId)
        });
        Assert.True(pizzaReference.HttpStatusCode == 200);

        var pizzaRef = Client.Data.Get(new()
        {
            Id = pizzaId,
            Class = CLASS_NAME_PIZZA
        });
        Assert.True(pizzaRef.HttpStatusCode == 200);
        CheckReference(pizzaRef.Result, CLASS_NAME_SOUP, soupId);

        var soupRef = Client.Data.Get(new()
        {
            Id = soupId,
            Class = CLASS_NAME_SOUP
        });
        Assert.True(soupRef.HttpStatusCode == 200);
        CheckReference(soupRef.Result, CLASS_NAME_PIZZA, pizzaId);
    }

    private static void CheckReference(IEnumerable<WeaviateObject> result, string className, string refId)
    {
        var jsonElement = (JsonElement)result.Single().Properties!["otherFoods"];
        var otherFoods = jsonElement.EnumerateArray().Select(a => a).ToArray();
        var otherFood = otherFoods.Single();
        Assert.Equal(otherFood.GetProperty("beacon").GetString(), "weaviate://localhost/" + className + "/" + refId);
        Assert.Equal(otherFood.GetProperty("href").GetString(), "/v1/objects/" + className + "/" + refId);
    }
}
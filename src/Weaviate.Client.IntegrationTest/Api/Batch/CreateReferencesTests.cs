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
using SearchPioneer.Weaviate.Client.Api.Batch.Request;
using SearchPioneer.Weaviate.Client.Api.Data.Model;
using Xunit;

namespace SearchPioneer.Weaviate.Client.IntegrationTest.Api.Batch;

[Collection("Sequential")]
public class CreateReferencesTests : TestBase
{
    [Fact]
    public void References()
    {
        CreateWeaviateTestSchemaFoodWithReferenceProperty(Client);

        var id1 = "97fa5147-bdad-4d74-9a81-f8babc811b09";
        var id2 = "07473b34-0ab2-4120-882d-303d9e13f7af";

        var batch = Client.Batch.CreateObjects(new(
            new WeaviateObject
            {
                Id = id1,
                Class = CLASS_NAME_PIZZA,
                Properties = new()
                {
                    { "name", "Doener" },
                    { "description", "A innovation, some say revolution, in the pizza industry." }
                }
            },
            new WeaviateObject
            {
                Id = id2,
                Class = CLASS_NAME_SOUP,
                Properties = new()
                {
                    { "name", "Beautiful" },
                    { "description", "Putting the game of letter soups to a whole new level." }
                }
            }));
        Assert.True(batch.HttpStatusCode == 200);

        var refTo1 = new BatchReference(
            "weaviate://localhost/Pizza/97fa5147-bdad-4d74-9a81-f8babc811b09/otherFoods",
            "weaviate://localhost/Soup/07473b34-0ab2-4120-882d-303d9e13f7af");

        var refTo2 = new BatchReference(
            "weaviate://localhost/Soup/07473b34-0ab2-4120-882d-303d9e13f7af/otherFoods",
            "weaviate://localhost/Pizza/97fa5147-bdad-4d74-9a81-f8babc811b09");

        var ref1To1 = Client.Batch.Reference(CLASS_NAME_PIZZA, CLASS_NAME_PIZZA, "otherFoods", id1, id1)!;
        var ref2To2 = Client.Batch.Reference(CLASS_NAME_SOUP, CLASS_NAME_SOUP, "otherFoods", id2, id2)!;

        var references = Client.Batch.CreateReferences(new(refTo1, refTo2, ref1To1, ref2To2));
        Assert.True(references.HttpStatusCode == 200);

        var object1 = Client.Data.Get(new() { Class = CLASS_NAME_PIZZA, Id = id1 }).Result.Single();
        var object2 = Client.Data.Get(new() { Class = CLASS_NAME_SOUP, Id = id2 }).Result.Single();

        var object1Beacons = ((JsonElement)object1.Properties!["otherFoods"]).EnumerateArray()
            .Select(a => a.GetProperty("beacon").GetString()).ToArray();
        var object2Beacons = ((JsonElement)object2.Properties!["otherFoods"]).EnumerateArray()
            .Select(a => a.GetProperty("beacon").GetString()).ToArray();

        Assert.Contains("weaviate://localhost/Soup/07473b34-0ab2-4120-882d-303d9e13f7af", object1Beacons);
        Assert.Contains("weaviate://localhost/Pizza/97fa5147-bdad-4d74-9a81-f8babc811b09", object1Beacons);

        Assert.Contains("weaviate://localhost/Soup/07473b34-0ab2-4120-882d-303d9e13f7af", object2Beacons);
        Assert.Contains("weaviate://localhost/Pizza/97fa5147-bdad-4d74-9a81-f8babc811b09", object2Beacons);
    }
}
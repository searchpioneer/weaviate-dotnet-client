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

using SearchPioneer.Weaviate.Client.Api.Schema.Model;
using Xunit;

namespace SearchPioneer.Weaviate.Client.UnitTest.V1.Schema.Model;

public class PropertyTests
{
    [Fact]
    public void Serialise()
    {
        var property = new Property
        {
            Name = "price",
            Description = "price",
            DataType = new[]
            {
                DataType.Number
            },
            ModuleConfig = new Dictionary<object, Dictionary<object, object>>
            {
                { "text2vec-contextionary", new() { { "vectorizePropertyName", false } } }
            }
        };

        var expected =
            "{\"name\":\"price\",\"dataType\":[\"number\"],\"description\":\"price\",\"indexInverted\":true," +
            "\"moduleConfig\":{\"text2vec-contextionary\":{\"vectorizePropertyName\":false}}}";

        Assert.Equal(expected, new Serializer().Serialize(property));
    }
}
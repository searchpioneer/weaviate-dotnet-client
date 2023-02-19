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

using SearchPioneer.Weaviate.Client.Api.Graph.Query.Arguments;
using Xunit;

namespace SearchPioneer.Weaviate.Client.UnitTest.V1.Graph.Query.Arguments;

public class SortTests : TestBase
{
    [Fact]
    public void WithPathAndProperty()
    {
        var arg = new Sort
        {
            Path = new[]
            {
                "property"
            },
            Order = SortOrder.Asc
        };
        Assert.Equal("{path:[\"property\"] order:asc}", arg.ToString());
    }

    [Fact]
    public void WithoutOrder()
    {
        var arg = new Sort
        {
            Path = new[]
            {
                "property"
            }
        };
        Assert.Equal("{path:[\"property\"]}", arg.ToString());
    }

    [Fact]
    public void WithoutAll()
    {
        var arg = new Sort();

        // builder will return a faulty nearObject arg in order for Weaviate to error
        // so that user will know that something was wrong
        Assert.Equal("{}", arg.ToString());
    }
}
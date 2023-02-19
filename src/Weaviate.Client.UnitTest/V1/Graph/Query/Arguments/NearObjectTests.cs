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

public class NearObjectTests : TestBase
{
    [Fact]
    public void WithCertainty()
    {
        var nearObject = new NearObject
        {
            Id = "id",
            Beacon = "beacon",
            Certainty = 0.8f
        };
        Assert.Equal("nearObject:{id:\"id\" beacon:\"beacon\" certainty:0.8}", nearObject.ToString());
    }

    [Fact]
    public void WithoutCertainity()
    {
        var nearObject = new NearObject
        {
            Id = "id",
            Beacon = "beacon"
        };
        Assert.Equal("nearObject:{id:\"id\" beacon:\"beacon\"}", nearObject.ToString());
    }

    [Fact]
    public void WithDistance()
    {
        var nearObject = new NearObject
        {
            Id = "id",
            Beacon = "beacon",
            Distance = 0.8f
        };
        Assert.Equal("nearObject:{id:\"id\" beacon:\"beacon\" distance:0.8}", nearObject.ToString());
    }

    [Fact]
    public void WithCertaintyAndWithoutId()
    {
        var nearObject = new NearObject
        {
            Beacon = "beacon",
            Certainty = 0.4f
        };
        Assert.Equal("nearObject:{beacon:\"beacon\" certainty:0.4}", nearObject.ToString());
    }

    [Fact]
    public void WithDistanceAndWithoutId()
    {
        var nearObject = new NearObject
        {
            Beacon = "beacon",
            Distance = 0.4f
        };
        Assert.Equal("nearObject:{beacon:\"beacon\" distance:0.4}", nearObject.ToString());
    }

    [Fact]
    public void WithCertaintyWithoutBeacon()
    {
        var nearObject = new NearObject
        {
            Id = "id",
            Certainty = 0.1f
        };
        Assert.Equal("nearObject:{id:\"id\" certainty:0.1}", nearObject.ToString());
    }

    [Fact]
    public void WithDistanceWithoutBeacon()
    {
        var nearObject = new NearObject
        {
            Id = "id",
            Distance = 0.1f
        };
        Assert.Equal("nearObject:{id:\"id\" distance:0.1}", nearObject.ToString());
    }

    [Fact]
    public void WithoutAll()
    {
        var nearObject = new NearObject();

        // builder will return a faulty nearObject arg in order for Weaviate to error
        // so that user will know that something was wrong
        Assert.Equal("nearObject:{}", nearObject.ToString());
    }
}
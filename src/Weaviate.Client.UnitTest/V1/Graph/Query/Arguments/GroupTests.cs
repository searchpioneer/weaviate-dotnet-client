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

public class GroupTests : TestBase
{
    [Fact]
    public void WithAllParameters()
    {
        var groupArgument = new Group
        {
            Type = GroupType.Merge,
            Force = 0.05f
        };
        Assert.Equal("group:{type:merge force:0.05}", groupArgument.ToString());
    }

    [Fact]
    public void WithType()
    {
        var groupArgument = new Group
        {
            Type = GroupType.Closest
        };
        Assert.Equal("group:{type:closest}", groupArgument.ToString());
    }

    [Fact]
    public void WithForce()
    {
        var groupArgument = new Group
        {
            Force = 0.90f
        };
        Assert.Equal("group:{force:0.9}", groupArgument.ToString());
    }

    [Fact]
    public void WithoutAll()
    {
        var groupArgument = new Group();
        // builder will return a faulty group arg in order for Weaviate to error
        // so that user will know that something was wrong
        Assert.Equal("group:{}", groupArgument.ToString());
    }
}
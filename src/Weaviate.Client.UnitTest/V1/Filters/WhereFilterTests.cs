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

using SearchPioneer.Weaviate.Client.Api.Filters;
using Xunit;

namespace SearchPioneer.Weaviate.Client.UnitTest.V1.Filters;

public class WhereFilterTests
{
    [Fact]
    public void ValueText()
    {
        var expected = "where:{path:[\"add\"] valueText:\"txt\" operator:And}";
        var filter = new Where
        {
            ValueText = "txt",
            Operator = Operator.And,
            Path = new[]
            {
                "add"
            }
        };
        Assert.Equal(expected, filter.BuildWithWhere());
    }

    [Fact]
    public void ValueString()
    {
        var expected = "where:{path:[\"add\"] valueString:\"txt\" operator:Equal}";
        var filter = new Where
        {
            ValueString = "txt",
            Operator = Operator.Equal,
            Path = new[]
            {
                "add"
            }
        };
        Assert.Equal(expected, filter.BuildWithWhere());
    }

    [Fact]
    public void ValueInt()
    {
        var expected = "where:{path:[\"add\"] valueInt:11 operator:Or}";
        var filter = new Where
        {
            ValueInt = 11,
            Operator = Operator.Or,
            Path = new[]
            {
                "add"
            }
        };
        Assert.Equal(expected, filter.BuildWithWhere());
    }

    [Fact]
    public void ValueBoolean()
    {
        var expected = "where:{path:[\"add\"] valueBoolean:true operator:GreaterThan}";
        var filter = new Where
        {
            ValueBoolean = true,
            Operator = Operator.GreaterThan,
            Path = new[]
            {
                "add"
            }
        };
        Assert.Equal(expected, filter.BuildWithWhere());
    }

    [Fact]
    public void ValueNumber()
    {
        var expected = "where:{path:[\"add\"] valueNumber:22.1 operator:GreaterThanEqual}";
        var filter = new Where
        {
            ValueNumber = 22.1,
            Operator = Operator.GreaterThanEqual,
            Path = new[]
            {
                "add"
            }
        };
        Assert.Equal(expected, filter.BuildWithWhere());
    }

    [Fact]
    public void ValueGeoCoordinates()
    {
        var expected =
            "where:{path:[\"add\"] valueGeoRange:{geoCoordinates:{latitude:50.51,longitude:0.11},distance:{max:3000}} operator:WithinGeoRange}";
        var filter = new Where
        {
            ValueGeoRange = new()
            {
                Coordinates = new(50.51f, 0.11f),
                Distance = new()
                {
                    Max = 3000f
                }
            },
            Operator = Operator.WithinGeoRange,
            Path = new[]
            {
                "add"
            }
        };
        Assert.Equal(expected, filter.BuildWithWhere());
    }

    [Fact]
    public void ValueDate()
    {
        var date = DateTime.Now;
        var expected = $"where:{{path:[\"add\"] valueDate:\"{date:o}\" operator:Like}}";
        var filter = new Where
        {
            ValueDate = date,
            Operator = Operator.Like,
            Path = new[]
            {
                "add"
            }
        };
        Assert.Equal(expected, filter.BuildWithWhere());
    }

    [Fact]
    public void Operands()
    {
        var expected =
            "where:{operator:And operands:[{path:[\"wordCount\"] valueInt:10 operator:LessThanEqual},{path:[\"word\"] valueString:\"word\" operator:LessThan}]}";
        var filter = new Where
        {
            Operator = Operator.And,
            Operands = new Where[]
            {
                new()
                {
                    ValueInt = 10,
                    Operator = Operator.LessThanEqual,
                    Path = new[] { "wordCount" }
                },
                new()
                {
                    ValueString = "word",
                    Operator = Operator.LessThan,
                    Path = new[] { "word" }
                }
            }
        };
        Assert.Equal(expected, filter.BuildWithWhere());
    }

    [Fact]
    public void MultiplePathParams()
    {
        var date = DateTime.Now;
        var expected = $"where:{{path:[\"p1\",\"p2\",\"p3\"] valueDate:\"{date:o}\" operator:Not}}";
        var filter = new Where
        {
            ValueDate = date,
            Operator = Operator.Not,
            Path = new[]
            {
                "p1", "p2", "p3"
            }
        };
        Assert.Equal(expected, filter.BuildWithWhere());
    }

    [Fact]
    public void OperandsWithMultiplePathParams()
    {
        var expected =
            "where:{operator:NotEqual operands:[{path:[\"wordCount\"] valueInt:10 operator:LessThanEqual},{path:[\"w1\",\"w2\",\"w3\"] valueString:\"word\" operator:LessThan}]}";
        var filter = new Where
        {
            Operator = Operator.NotEqual,
            Operands = new Where[]
            {
                new()
                {
                    ValueInt = 10,
                    Operator = Operator.LessThanEqual,
                    Path = new[] { "wordCount" }
                },
                new()
                {
                    ValueString = "word",
                    Operator = Operator.LessThan,
                    Path = new[] { "w1", "w2", "w3" }
                }
            }
        };
        Assert.Equal(expected, filter.BuildWithWhere());
    }

    [Fact]
    public void WithoutAll()
    {
        Assert.Equal("where:{}", new Where().BuildWithWhere());
    }
}
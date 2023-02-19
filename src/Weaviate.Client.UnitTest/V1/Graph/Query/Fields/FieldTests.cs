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

using SearchPioneer.Weaviate.Client.Api.Graph.Query.Fields;
using Xunit;

namespace SearchPioneer.Weaviate.Client.UnitTest.V1.Graph.Query.Fields;

public class FieldTests : TestBase
{
    [Fact]
    public void FieldCertainty()
    {
        var field = new Field
        {
            Name = "_additional",
            Fields = new[]
            {
                new Field
                {
                    Name = "certainty"
                }
            }
        };
        Assert.Equal("_additional{certainty}", field.ToString());
    }

    [Fact]
    public void FieldAdditional()
    {
        var field = new Field
        {
            Name = "_additional",
            Fields = new[]
            {
                new Field
                {
                    Name = "classification",
                    Fields = new[]
                    {
                        new Field
                        {
                            Name = "basedOn"
                        },
                        new Field
                        {
                            Name = "classifiedFields"
                        },
                        new Field
                        {
                            Name = "completed"
                        },
                        new Field
                        {
                            Name = "id"
                        },
                        new Field
                        {
                            Name = "scope"
                        }
                    }
                }
            }
        };
        Assert.Equal("_additional{classification{basedOn classifiedFields completed id scope}}", field.ToString());
    }

    [Fact]
    public void FieldOn()
    {
        var field = new Field
        {
            Name = "inPublication",
            Fields = new[]
            {
                new Field
                {
                    Name = "... on Publication",
                    Fields = new[]
                    {
                        new Field
                        {
                            Name = "name"
                        }
                    }
                }
            }
        };
        Assert.Equal("inPublication{... on Publication{name}}", field.ToString());
    }

    [Fact]
    public void FieldDistance()
    {
        var field = new Field
        {
            Name = "_additional",
            Fields = new[]
            {
                new Field
                {
                    Name = "distance"
                }
            }
        };
        Assert.Equal("_additional{distance}", field.ToString());
    }

    [Fact]
    public void FieldNested()
    {
        var field = new Field
        {
            Name = "a",
            Fields = new[]
            {
                new Field
                {
                    Name = "b",
                    Fields = new[]
                    {
                        new Field
                        {
                            Name = "c"
                        }
                    }
                },
                new Field
                {
                    Name = "d",
                    Fields = new[]
                    {
                        new Field
                        {
                            Name = "e"
                        }
                    }
                },
                new Field
                {
                    Name = "f"
                }
            }
        };
        Assert.Equal("a{b{c} d{e} f}", field.ToString());
    }
}
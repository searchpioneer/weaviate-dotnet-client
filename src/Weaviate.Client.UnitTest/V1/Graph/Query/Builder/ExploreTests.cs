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

using SearchPioneer.Weaviate.Client.Api.Graph.Model;
using SearchPioneer.Weaviate.Client.Api.Graph.Query.Builder;
using Xunit;

namespace SearchPioneer.Weaviate.Client.UnitTest.V1.Graph.Query.Builder;

public class ExploreTests : TestBase
{
    [Fact]
    public void QueryWithCertainty()
    {
        var query = new Explore
        {
            NearText = new()
            {
                Concepts = new[] { "a", "b" },
                Certainty = 0.8f,
                MoveTo = new()
                {
                    Concepts = new[] { "a1", "b2" },
                    Force = 0.1f
                }
            },
            Fields = new[]
            {
                ExploreFields.Certainty,
                ExploreFields.Distance,
                ExploreFields.Beacon,
                ExploreFields.ClassName
            }
        };
        Assert.Equal(
            "{Explore(nearText:{concepts: [\"a\",\"b\"] certainty:0.8 moveTo:{concepts: [\"a1\",\"b2\"] force:0.1}}){certainty,distance," +
            "beacon,className}}", query.ToString());
    }

    [Fact]
    public void QueryWithDistance()
    {
        var query = new Explore
        {
            NearText = new()
            {
                Concepts = new[] { "a", "b" },
                Distance = 0.8f,
                MoveTo = new()
                {
                    Concepts = new[] { "a1", "b2" },
                    Force = 0.1f
                }
            },
            Fields = new[]
            {
                ExploreFields.Certainty,
                ExploreFields.Distance,
                ExploreFields.Beacon,
                ExploreFields.ClassName
            }
        };
        Assert.Equal(
            "{Explore(nearText:{concepts: [\"a\",\"b\"] distance:0.8 moveTo:{concepts: [\"a1\",\"b2\"] force:0.1}}){certainty,distance," +
            "beacon,className}}", query.ToString());
    }

    [Fact]
    public void SimpleExplore()
    {
        var query = new Explore
        {
            NearText = new()
            {
                Concepts = new[] { "Cheese", "pineapple" }
            },
            Fields = new[]
            {
                ExploreFields.Certainty,
                ExploreFields.Beacon,
                ExploreFields.Distance
            }
        };
        Assert.Equal("{Explore(nearText:{concepts: [\"Cheese\",\"pineapple\"]}){certainty,beacon,distance}}",
            query.ToString());
    }

    [Fact]
    public void ExploreWithLimitAndCertainty()
    {
        var query = new Explore
        {
            NearText = new()
            {
                Concepts = new[] { "Cheese" },
                Certainty = 0.71f
            },
            Fields = new[]
            {
                ExploreFields.Beacon
            }
        };
        Assert.Equal("{Explore(nearText:{concepts: [\"Cheese\"] certainty:0.71}){beacon}}", query.ToString());
    }

    [Fact]
    public void ExploreWithLimitAndDistance()
    {
        var query = new Explore
        {
            NearText = new()
            {
                Concepts = new[] { "Cheese" },
                Distance = 0.71f
            },
            Fields = new[]
            {
                ExploreFields.Beacon
            }
        };
        Assert.Equal("{Explore(nearText:{concepts: [\"Cheese\"] distance:0.71}){beacon}}", query.ToString());
    }

    [Fact]
    public void ExploreWithMove()
    {
        var query = new Explore
        {
            NearText = new()
            {
                Concepts = new[] { "Cheese" },
                MoveTo = new()
                {
                    Concepts = new[] { "pizza", "pineapple" },
                    Force = 0.2f
                },
                MoveAwayFrom = new()
                {
                    Concepts = new[] { "fish" },
                    Force = 0.1f
                }
            },
            Fields = new[]
            {
                ExploreFields.Beacon
            }
        };
        Assert.Equal("{Explore(nearText:{concepts: [\"Cheese\"] " +
                     "moveTo:{concepts: [\"pizza\",\"pineapple\"] force:0.2} " +
                     "moveAwayFrom:{concepts: [\"fish\"] force:0.1}}){beacon}}", query.ToString());
    }

    [Fact]
    public void ExploreWithAllParams()
    {
        var query = new Explore
        {
            NearText = new()
            {
                Concepts = new[] { "New Yorker" },
                MoveTo = new()
                {
                    Concepts = new[] { "publisher", "articles" },
                    Force = 0.5f
                },
                MoveAwayFrom = new()
                {
                    Concepts = new[] { "fashion", "shop" },
                    Force = 0.2f
                }
            },
            Fields = new[]
            {
                ExploreFields.Certainty,
                ExploreFields.Distance,
                ExploreFields.Beacon,
                ExploreFields.ClassName
            }
        };
        Assert.Equal(
            "{Explore(nearText:{concepts: [\"New Yorker\"] moveTo:{concepts: [\"publisher\",\"articles\"] force:0.5} moveAwayFrom:{concepts: " +
            "[\"fashion\",\"shop\"] force:0.2}}){certainty,distance,beacon,className}}", query.ToString());
    }

    [Fact]
    public void ExploreWithNearVectorCertainty()
    {
        var query = new Explore
        {
            NearVector = new()
            {
                Vector = new[] { 0f, 1f, 0.8f },
                Certainty = 0.8f
            },
            Fields = new[]
            {
                ExploreFields.Certainty,
                ExploreFields.Distance,
                ExploreFields.Beacon,
                ExploreFields.ClassName
            }
        };
        Assert.Equal("{Explore(nearVector:{vector: [0,1,0.8] certainty:0.8})" +
                     "{certainty,distance,beacon,className}}", query.ToString());
    }

    [Fact]
    public void ExploreWithNearVectorDistance()
    {
        var query = new Explore
        {
            NearVector = new()
            {
                Vector = new[] { 0f, 1f, 0.8f },
                Distance = 0.8f
            },
            Fields = new[]
            {
                ExploreFields.Certainty,
                ExploreFields.Distance,
                ExploreFields.Beacon,
                ExploreFields.ClassName
            }
        };
        Assert.Equal("{Explore(nearVector:{vector: [0,1,0.8] distance:0.8})" +
                     "{certainty,distance,beacon,className}}", query.ToString());
    }

    [Fact]
    public void ExploreWithNearObjectAndCertainty()
    {
        var query = new Explore
        {
            NearObject = new()
            {
                Id = "some-uuid",
                Certainty = 0.8f
            },
            Fields = new[]
            {
                ExploreFields.Certainty,
                ExploreFields.Distance,
                ExploreFields.Beacon,
                ExploreFields.ClassName
            }
        };
        Assert.Equal("{Explore(nearObject:{id:\"some-uuid\" certainty:0.8}){certainty,distance," +
                     "beacon,className}}", query.ToString());
    }

    [Fact]
    public void ExploreWithNearObjectAndDistance()
    {
        var query = new Explore
        {
            NearObject = new()
            {
                Id = "some-uuid",
                Distance = 0.8f
            },
            Fields = new[]
            {
                ExploreFields.Certainty,
                ExploreFields.Distance,
                ExploreFields.Beacon,
                ExploreFields.ClassName
            }
        };
        Assert.Equal("{Explore(nearObject:{id:\"some-uuid\" distance:0.8}){certainty,distance," +
                     "beacon,className}}", query.ToString());
    }

    [Fact]
    public void ExploreWithAskAndCertainty()
    {
        var query = new Explore
        {
            Ask = new()
            {
                Question = "question?",
                Certainty = 0.8f,
                Rerank = true
            },
            Fields = new[]
            {
                ExploreFields.Certainty,
                ExploreFields.Distance,
                ExploreFields.Beacon,
                ExploreFields.ClassName
            }
        };
        Assert.Equal("{Explore(ask:{question:\"question?\" certainty:0.8 rerank:true}){certainty,distance," +
                     "beacon,className}}", query.ToString());
    }

    [Fact]
    public void ExploreWithAskAndDistance()
    {
        var query = new Explore
        {
            Ask = new()
            {
                Question = "question?",
                Distance = 0.8f,
                Rerank = true
            },
            Fields = new[]
            {
                ExploreFields.Certainty,
                ExploreFields.Distance,
                ExploreFields.Beacon,
                ExploreFields.ClassName
            }
        };
        Assert.Equal("{Explore(ask:{question:\"question?\" distance:0.8 rerank:true}){certainty,distance," +
                     "beacon,className}}", query.ToString());
    }

    [Fact]
    public void ExploreWithNearImageAndCertainty()
    {
        var query = new Explore
        {
            NearImage = new()
            {
                Image = "iVBORw0KGgoAAAANS",
                Certainty = 0.8f
            },
            Fields = new[]
            {
                ExploreFields.Certainty,
                ExploreFields.Distance,
                ExploreFields.Beacon,
                ExploreFields.ClassName
            }
        };
        Assert.Equal("{Explore(nearImage:{image:\"iVBORw0KGgoAAAANS\" certainty:0.8}){certainty,distance," +
                     "beacon,className}}", query.ToString());
    }

    [Fact]
    public void ExploreWithNearImageAndDistance()
    {
        var query = new Explore
        {
            NearImage = new()
            {
                Image = "iVBORw0KGgoAAAANS",
                Distance = 0.8f
            },
            Fields = new[]
            {
                ExploreFields.Certainty,
                ExploreFields.Distance,
                ExploreFields.Beacon,
                ExploreFields.ClassName
            }
        };
        Assert.Equal("{Explore(nearImage:{image:\"iVBORw0KGgoAAAANS\" distance:0.8}){certainty,distance," +
                     "beacon,className}}", query.ToString());
    }
}
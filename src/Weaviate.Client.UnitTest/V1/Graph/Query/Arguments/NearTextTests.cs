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

public class NearTextTests : TestBase
{
    [Fact]
    public void WithCertainty()
    {
        var nearText = new NearText
        {
            Concepts = new[] { "a", "b", "c" },
            Certainty = 0.8f
        };
        Assert.Equal("nearText:{concepts: [\"a\",\"b\",\"c\"] certainty:0.8}", nearText.ToString());
    }

    [Fact]
    public void WithDistance()
    {
        var nearText = new NearText
        {
            Concepts = new[] { "a", "b", "c" },
            Distance = 0.8f
        };
        Assert.Equal("nearText:{concepts: [\"a\",\"b\",\"c\"] distance:0.8}", nearText.ToString());
    }

    [Fact]
    public void WithCertaintyAndNoConcepts()
    {
        var nearText = new NearText
        {
            Certainty = 0.8f
        };

        // builder will return a faulty nearText arg in order for Weaviate to error
        // so that user will know that something was wrong
        Assert.Equal("nearText:{certainty:0.8}", nearText.ToString());
    }

    [Fact]
    public void WithDistanceAndNoConcepts()
    {
        var nearText = new NearText
        {
            Distance = 0.8f
        };

        // builder will return a faulty nearText arg in order for Weaviate to error
        // so that user will know that something was wrong
        Assert.Equal("nearText:{distance:0.8}", nearText.ToString());
    }

    [Fact]
    public void WithoutAll()
    {
        var nearText = new NearText();

        // builder will return a faulty nearText arg in order for Weaviate to error
        // so that user will know that something was wrong
        Assert.Equal("nearText:{}", nearText.ToString());
    }

    [Fact]
    public void MoveToWithCertainty()
    {
        var nearText = new NearText
        {
            Concepts = new[] { "a", "b", "c" },
            Certainty = 0.8f,
            MoveTo = new()
            {
                Concepts = new[] { "a1", "b2" },
                Force = 0.1f
            }
        };
        Assert.Equal(
            "nearText:{concepts: [\"a\",\"b\",\"c\"] certainty:0.8 moveTo:{concepts: [\"a1\",\"b2\"] force:0.1}}",
            nearText.ToString());
    }

    [Fact]
    public void MoveToWithDistance()
    {
        var nearText = new NearText
        {
            Concepts = new[] { "a", "b", "c" },
            Distance = 0.8f,
            MoveTo = new()
            {
                Concepts = new[] { "a1", "b2" },
                Force = 0.1f
            }
        };
        Assert.Equal(
            "nearText:{concepts: [\"a\",\"b\",\"c\"] distance:0.8 moveTo:{concepts: [\"a1\",\"b2\"] force:0.1}}",
            nearText.ToString());
    }

    [Fact]
    public void MoveToWithCertaintyWithoutForce()
    {
        var nearText = new NearText
        {
            Concepts = new[] { "a", "b", "c" },
            Certainty = 0.8f,
            MoveTo = new()
            {
                Concepts = new[] { "a1", "b2" }
            }
        };
        Assert.Equal("nearText:{concepts: [\"a\",\"b\",\"c\"] certainty:0.8 moveTo:{concepts: [\"a1\",\"b2\"]}}",
            nearText.ToString());
    }

    [Fact]
    public void MoveToWithDistanceWithoutForce()
    {
        var nearText = new NearText
        {
            Concepts = new[] { "a", "b", "c" },
            Distance = 0.8f,
            MoveTo = new()
            {
                Concepts = new[] { "a1", "b2" }
            }
        };
        Assert.Equal("nearText:{concepts: [\"a\",\"b\",\"c\"] distance:0.8 moveTo:{concepts: [\"a1\",\"b2\"]}}",
            nearText.ToString());
    }

    [Fact]
    public void MoveAwayFromWithCertainty()
    {
        var nearText = new NearText
        {
            Concepts = new[] { "a", "b", "c" },
            Certainty = 0.8f,
            MoveAwayFrom = new()
            {
                Concepts = new[] { "a1", "b2" },
                Force = 0.1f
            }
        };
        Assert.Equal(
            "nearText:{concepts: [\"a\",\"b\",\"c\"] certainty:0.8 moveAwayFrom:{concepts: [\"a1\",\"b2\"] force:0.1}}",
            nearText.ToString());
    }

    [Fact]
    public void MoveAwayFromWithDistance()
    {
        var nearText = new NearText
        {
            Concepts = new[] { "a", "b", "c" },
            Distance = 0.8f,
            MoveAwayFrom = new()
            {
                Concepts = new[] { "a1", "b2" },
                Force = 0.1f
            }
        };
        Assert.Equal(
            "nearText:{concepts: [\"a\",\"b\",\"c\"] distance:0.8 moveAwayFrom:{concepts: [\"a1\",\"b2\"] force:0.1}}",
            nearText.ToString());
    }

    [Fact]
    public void MoveAwayFromWithCertaintyWithoutForce()
    {
        var nearText = new NearText
        {
            Concepts = new[] { "a", "b", "c" },
            Certainty = 0.8f,
            MoveAwayFrom = new()
            {
                Concepts = new[] { "a1", "b2" }
            }
        };
        Assert.Equal(
            "nearText:{concepts: [\"a\",\"b\",\"c\"] certainty:0.8 moveAwayFrom:{concepts: [\"a1\",\"b2\"]}}",
            nearText.ToString());
    }

    [Fact]
    public void MoveAwayFromWithDistanceWithoutForce()
    {
        var nearText = new NearText
        {
            Concepts = new[] { "a", "b", "c" },
            Distance = 0.8f,
            MoveAwayFrom = new()
            {
                Concepts = new[] { "a1", "b2" }
            }
        };
        Assert.Equal(
            "nearText:{concepts: [\"a\",\"b\",\"c\"] distance:0.8 moveAwayFrom:{concepts: [\"a1\",\"b2\"]}}",
            nearText.ToString());
    }

    [Fact]
    public void MoveToAndMoveAwayFromWithCertainty()
    {
        var nearText = new NearText
        {
            Concepts = new[] { "a", "b", "c" },
            Certainty = 0.8f,
            MoveTo = new()
            {
                Concepts = new[] { "z1", "y2" },
                Force = 0.8f
            },
            MoveAwayFrom = new()
            {
                Concepts = new[] { "a1", "b2" },
                Force = 0.1f
            }
        };
        Assert.Equal("nearText:{concepts: [\"a\",\"b\",\"c\"] certainty:0.8 " +
                     "moveTo:{concepts: [\"z1\",\"y2\"] force:0.8} " +
                     "moveAwayFrom:{concepts: [\"a1\",\"b2\"] force:0.1}}", nearText.ToString());
    }

    [Fact]
    public void MoveToAndMoveAwayFromWithDistance()
    {
        var nearText = new NearText
        {
            Concepts = new[] { "a", "b", "c" },
            Distance = 0.8f,
            MoveTo = new()
            {
                Concepts = new[] { "z1", "y2" },
                Force = 0.8f
            },
            MoveAwayFrom = new()
            {
                Concepts = new[] { "a1", "b2" },
                Force = 0.1f
            }
        };
        Assert.Equal("nearText:{concepts: [\"a\",\"b\",\"c\"] distance:0.8 " +
                     "moveTo:{concepts: [\"z1\",\"y2\"] force:0.8} " +
                     "moveAwayFrom:{concepts: [\"a1\",\"b2\"] force:0.1}}", nearText.ToString());
    }

    [Fact]
    public void MoveToAndMoveAwayFromWithCertaintyWithoutForce()
    {
        var nearText = new NearText
        {
            Concepts = new[] { "a", "b", "c" },
            Certainty = 0.8f,
            MoveTo = new()
            {
                Concepts = new[] { "z1", "y2" },
                Force = 0.8f
            },
            MoveAwayFrom = new()
            {
                Concepts = new[] { "a1", "b2" }
            }
        };
        Assert.Equal("nearText:{concepts: [\"a\",\"b\",\"c\"] certainty:0.8 " +
                     "moveTo:{concepts: [\"z1\",\"y2\"] force:0.8} " +
                     "moveAwayFrom:{concepts: [\"a1\",\"b2\"]}}", nearText.ToString());
    }

    [Fact]
    public void MoveToAndMoveAwayFromWithDistanceWithoutForce()
    {
        var nearText = new NearText
        {
            Concepts = new[] { "a", "b", "c" },
            Distance = 0.8f,
            MoveTo = new()
            {
                Concepts = new[] { "z1", "y2" },
                Force = 0.8f
            },
            MoveAwayFrom = new()
            {
                Concepts = new[] { "a1", "b2" }
            }
        };
        Assert.Equal("nearText:{concepts: [\"a\",\"b\",\"c\"] distance:0.8 " +
                     "moveTo:{concepts: [\"z1\",\"y2\"] force:0.8} " +
                     "moveAwayFrom:{concepts: [\"a1\",\"b2\"]}}", nearText.ToString());
    }

    [Fact]
    public void WithAutocorrectAndCertainty()
    {
        var nearText = new NearText
        {
            Concepts = new[] { "a", "b", "c" },
            Certainty = 0.8f,
            Autocorrect = false
        };
        Assert.Equal("nearText:{concepts: [\"a\",\"b\",\"c\"] certainty:0.8 autocorrect:false}",
            nearText.ToString());
    }

    [Fact]
    public void WithAutocorrectAndDistance()
    {
        var nearText = new NearText
        {
            Concepts = new[] { "a", "b", "c" },
            Distance = 0.8f,
            Autocorrect = false
        };
        Assert.Equal("nearText:{concepts: [\"a\",\"b\",\"c\"] distance:0.8 autocorrect:false}", nearText.ToString());
    }

    [Fact]
    public void MoveToAndMoveAwayFromWithCertaintyWithoutForceAndWithAutocorrect()
    {
        var nearText = new NearText
        {
            Concepts = new[] { "a", "b", "c" },
            Certainty = 0.8f,
            MoveTo = new()
            {
                Concepts = new[] { "z1", "y2" },
                Force = 0.8f
            },
            MoveAwayFrom = new()
            {
                Concepts = new[] { "a1", "b2" }
            },
            Autocorrect = true
        };
        Assert.Equal("nearText:{concepts: [\"a\",\"b\",\"c\"] certainty:0.8 " +
                     "moveTo:{concepts: [\"z1\",\"y2\"] force:0.8} " +
                     "moveAwayFrom:{concepts: [\"a1\",\"b2\"]} autocorrect:true}", nearText.ToString());
    }

    [Fact]
    public void MoveToAndMoveAwayFromWithDistanceWithoutForceAndWithAutocorrect()
    {
        var nearText = new NearText
        {
            Concepts = new[] { "a", "b", "c" },
            Distance = 0.8f,
            MoveTo = new()
            {
                Concepts = new[] { "z1", "y2" },
                Force = 0.8f
            },
            MoveAwayFrom = new()
            {
                Concepts = new[] { "a1", "b2" }
            },
            Autocorrect = true
        };
        Assert.Equal("nearText:{concepts: [\"a\",\"b\",\"c\"] distance:0.8 " +
                     "moveTo:{concepts: [\"z1\",\"y2\"] force:0.8} " +
                     "moveAwayFrom:{concepts: [\"a1\",\"b2\"]} autocorrect:true}", nearText.ToString());
    }

    [Fact]
    public void WithEmptyMoveTo()
    {
        var nearText = new NearText
        {
            MoveTo = new()
        };
        Assert.Equal("nearText:{moveTo:{}}", nearText.ToString());
    }

    [Fact]
    public void WithEmptyMoveAwayFrom()
    {
        var nearText = new NearText
        {
            MoveAwayFrom = new()
        };
        Assert.Equal("nearText:{moveAwayFrom:{}}", nearText.ToString());
    }

    [Fact]
    public void WithEmptyMoveToAndMoveAwayFrom()
    {
        var nearText = new NearText
        {
            MoveTo = new(),
            MoveAwayFrom = new()
        };
        Assert.Equal("nearText:{moveTo:{} moveAwayFrom:{}}", nearText.ToString());
    }

    [Fact]
    public void MoveToWithObjectsAndCertaintyIdBeaconForce()
    {
        var nearText = new NearText
        {
            Concepts = new[] { "a", "b", "c" },
            Certainty = 0.8f,
            MoveTo = new()
            {
                Objects = new[]
                {
                    new ObjectMove
                    {
                        Id = "uuid"
                    },
                    new ObjectMove
                    {
                        Beacon = "beacon"
                    }
                },
                Force = 0.1f
            }
        };
        Assert.Equal(
            "nearText:{concepts: [\"a\",\"b\",\"c\"] certainty:0.8 moveTo:{force:0.1 objects: [{id:\"uuid\"},{beacon:\"beacon\"}]}}",
            nearText.ToString());
    }

    [Fact]
    public void MoveToWithObjectsAndCertaintyIdBeacon()
    {
        var nearText = new NearText
        {
            Concepts = new[] { "a", "b", "c" },
            Certainty = 0.8f,
            MoveTo = new()
            {
                Objects = new[]
                {
                    new ObjectMove
                    {
                        Id = "uuid",
                        Beacon = "beacon"
                    }
                },
                Force = 0.1f
            }
        };
        Assert.Equal(
            "nearText:{concepts: [\"a\",\"b\",\"c\"] certainty:0.8 moveTo:{force:0.1 objects: [{id:\"uuid\" beacon:\"beacon\"}]}}",
            nearText.ToString());
    }

    [Fact]
    public void MoveToWithObjectsAndCertaintyId()
    {
        var nearText = new NearText
        {
            Concepts = new[] { "a", "b", "c" },
            Certainty = 0.8f,
            MoveTo = new()
            {
                Objects = new[]
                {
                    new ObjectMove
                    {
                        Id = "uuid"
                    }
                }
            }
        };
        Assert.Equal("nearText:{concepts: [\"a\",\"b\",\"c\"] certainty:0.8 moveTo:{objects: [{id:\"uuid\"}]}}",
            nearText.ToString());
    }

    [Fact]
    public void MoveToWithObjectsAndCertaintyBeacon()
    {
        var nearText = new NearText
        {
            Concepts = new[] { "a", "b", "c" },
            Certainty = 0.8f,
            MoveTo = new()
            {
                Objects = new[]
                {
                    new ObjectMove
                    {
                        Beacon = "beacon"
                    }
                }
            }
        };
        Assert.Equal(
            "nearText:{concepts: [\"a\",\"b\",\"c\"] certainty:0.8 moveTo:{objects: [{beacon:\"beacon\"}]}}",
            nearText.ToString());
    }

    [Fact]
    public void MoveToWithObjectsAndDistanceIdAndBeacon()
    {
        var nearText = new NearText
        {
            Concepts = new[] { "a", "b", "c" },
            Distance = 0.8f,
            MoveTo = new()
            {
                Objects = new[]
                {
                    new ObjectMove
                    {
                        Id = "uuid"
                    },
                    new ObjectMove
                    {
                        Beacon = "beacon"
                    }
                },
                Force = 0.1f
            }
        };
        Assert.Equal(
            "nearText:{concepts: [\"a\",\"b\",\"c\"] distance:0.8 moveTo:{force:0.1 objects: [{id:\"uuid\"},{beacon:\"beacon\"}]}}",
            nearText.ToString());
    }

    [Fact]
    public void MoveToWithObjectsAndDistanceIdBeaconForce()
    {
        var nearText = new NearText
        {
            Concepts = new[] { "a", "b", "c" },
            Distance = 0.8f,
            MoveTo = new()
            {
                Objects = new[]
                {
                    new ObjectMove
                    {
                        Id = "uuid",
                        Beacon = "beacon"
                    }
                },
                Force = 0.1f
            }
        };
        Assert.Equal(
            "nearText:{concepts: [\"a\",\"b\",\"c\"] distance:0.8 moveTo:{force:0.1 objects: [{id:\"uuid\" beacon:\"beacon\"}]}}",
            nearText.ToString());
    }

    [Fact]
    public void MoveToWithObjectsAndDistanceId()
    {
        var nearText = new NearText
        {
            Concepts = new[] { "a", "b", "c" },
            Distance = 0.8f,
            MoveTo = new()
            {
                Objects = new[]
                {
                    new ObjectMove
                    {
                        Id = "uuid"
                    }
                }
            }
        };
        Assert.Equal("nearText:{concepts: [\"a\",\"b\",\"c\"] distance:0.8 moveTo:{objects: [{id:\"uuid\"}]}}",
            nearText.ToString());
    }

    [Fact]
    public void MoveToWithObjectsAndDistanceBeacon()
    {
        var nearText = new NearText
        {
            Concepts = new[] { "a", "b", "c" },
            Distance = 0.8f,
            MoveTo = new()
            {
                Objects = new[]
                {
                    new ObjectMove
                    {
                        Beacon = "beacon"
                    }
                }
            }
        };
        Assert.Equal(
            "nearText:{concepts: [\"a\",\"b\",\"c\"] distance:0.8 moveTo:{objects: [{beacon:\"beacon\"}]}}",
            nearText.ToString());
    }

    [Fact]
    public void MoveAwayFromWithObjectsAndCertaintyIdBeaconForce()
    {
        var nearText = new NearText
        {
            Concepts = new[] { "a", "b", "c" },
            Certainty = 0.8f,
            MoveAwayFrom = new()
            {
                Objects = new[]
                {
                    new ObjectMove
                    {
                        Id = "uuid"
                    },
                    new ObjectMove
                    {
                        Beacon = "beacon"
                    }
                },
                Force = 0.1f
            }
        };
        Assert.Equal(
            "nearText:{concepts: [\"a\",\"b\",\"c\"] certainty:0.8 moveAwayFrom:{force:0.1 objects: [{id:\"uuid\"},{beacon:\"beacon\"}]}}",
            nearText.ToString());
    }

    [Fact]
    public void MoveAwayFromWithObjectsAndCertaintyIdBeacon()
    {
        var nearText = new NearText
        {
            Concepts = new[] { "a", "b", "c" },
            Certainty = 0.8f,
            MoveAwayFrom = new()
            {
                Objects = new[]
                {
                    new ObjectMove
                    {
                        Id = "uuid",
                        Beacon = "beacon"
                    }
                },
                Force = 0.1f
            }
        };
        Assert.Equal(
            "nearText:{concepts: [\"a\",\"b\",\"c\"] certainty:0.8 moveAwayFrom:{force:0.1 objects: [{id:\"uuid\" beacon:\"beacon\"}]}}",
            nearText.ToString());
    }

    [Fact]
    public void MoveAwayFromWithObjectsAndCertaintyId()
    {
        var nearText = new NearText
        {
            Concepts = new[] { "a", "b", "c" },
            Certainty = 0.8f,
            MoveAwayFrom = new()
            {
                Objects = new[]
                {
                    new ObjectMove
                    {
                        Id = "uuid"
                    }
                }
            }
        };
        Assert.Equal(
            "nearText:{concepts: [\"a\",\"b\",\"c\"] certainty:0.8 moveAwayFrom:{objects: [{id:\"uuid\"}]}}",
            nearText.ToString());
    }

    [Fact]
    public void MoveAwayFromWithObjectsAndCertaintyBeacon()
    {
        var nearText = new NearText
        {
            Concepts = new[] { "a", "b", "c" },
            Certainty = 0.8f,
            MoveAwayFrom = new()
            {
                Objects = new[]
                {
                    new ObjectMove
                    {
                        Beacon = "beacon"
                    }
                }
            }
        };
        Assert.Equal(
            "nearText:{concepts: [\"a\",\"b\",\"c\"] certainty:0.8 moveAwayFrom:{objects: [{beacon:\"beacon\"}]}}",
            nearText.ToString());
    }

    [Fact]
    public void MoveAwayFromWithObjectsAndDistanceIdAndBeacon()
    {
        var nearText = new NearText
        {
            Concepts = new[] { "a", "b", "c" },
            Distance = 0.8f,
            MoveAwayFrom = new()
            {
                Objects = new[]
                {
                    new ObjectMove
                    {
                        Id = "uuid"
                    },
                    new ObjectMove
                    {
                        Beacon = "beacon"
                    }
                },
                Force = 0.1f
            }
        };
        Assert.Equal(
            "nearText:{concepts: [\"a\",\"b\",\"c\"] distance:0.8 moveAwayFrom:{force:0.1 objects: [{id:\"uuid\"},{beacon:\"beacon\"}]}}",
            nearText.ToString());
    }

    [Fact]
    public void MoveAwayFromWithObjectsAndDistanceIdBeaconForce()
    {
        var nearText = new NearText
        {
            Concepts = new[] { "a", "b", "c" },
            Distance = 0.8f,
            MoveAwayFrom = new()
            {
                Objects = new[]
                {
                    new ObjectMove
                    {
                        Id = "uuid",
                        Beacon = "beacon"
                    }
                },
                Force = 0.1f
            }
        };
        Assert.Equal(
            "nearText:{concepts: [\"a\",\"b\",\"c\"] distance:0.8 moveAwayFrom:{force:0.1 objects: [{id:\"uuid\" beacon:\"beacon\"}]}}",
            nearText.ToString());
    }

    [Fact]
    public void MoveAwayFromWithObjectsAndDistanceId()
    {
        var nearText = new NearText
        {
            Concepts = new[] { "a", "b", "c" },
            Distance = 0.8f,
            MoveAwayFrom = new()
            {
                Objects = new[]
                {
                    new ObjectMove
                    {
                        Id = "uuid"
                    }
                }
            }
        };
        Assert.Equal(
            "nearText:{concepts: [\"a\",\"b\",\"c\"] distance:0.8 moveAwayFrom:{objects: [{id:\"uuid\"}]}}",
            nearText.ToString());
    }

    [Fact]
    public void MoveAwayFromWithObjectsAndDistanceBeacon()
    {
        var nearText = new NearText
        {
            Concepts = new[] { "a", "b", "c" },
            Distance = 0.8f,
            MoveAwayFrom = new()
            {
                Objects = new[]
                {
                    new ObjectMove
                    {
                        Beacon = "beacon"
                    }
                }
            }
        };
        Assert.Equal(
            "nearText:{concepts: [\"a\",\"b\",\"c\"] distance:0.8 moveAwayFrom:{objects: [{beacon:\"beacon\"}]}}",
            nearText.ToString());
    }

    [Fact]
    public void MoveToAndMoveAwayFromWithObjectsAndCertainty()
    {
        var nearText = new NearText
        {
            Concepts = new[] { "a", "b", "c" },
            Certainty = 0.8f,
            MoveTo = new()
            {
                Objects = new[]
                {
                    new ObjectMove
                    {
                        Id = "uuid"
                    },
                    new ObjectMove
                    {
                        Beacon = "beacon"
                    }
                },
                Force = 0.1f
            },
            MoveAwayFrom = new()
            {
                Objects = new[]
                {
                    new ObjectMove
                    {
                        Id = "uuid",
                        Beacon = "beacon"
                    }
                },
                Force = 0.2f
            }
        };
        Assert.Equal(
            "nearText:{concepts: [\"a\",\"b\",\"c\"] certainty:0.8 moveTo:{force:0.1 objects: [{id:\"uuid\"},{beacon:\"beacon\"}]} " +
            "moveAwayFrom:{force:0.2 objects: [{id:\"uuid\" beacon:\"beacon\"}]}}", nearText.ToString());
    }

    [Fact]
    public void MoveToAndMoveAwayFromWithObjectsAndDistance()
    {
        var nearText = new NearText
        {
            Concepts = new[] { "a", "b", "c" },
            Distance = 0.8f,
            MoveTo = new()
            {
                Objects = new[]
                {
                    new ObjectMove
                    {
                        Id = "uuid"
                    },
                    new ObjectMove
                    {
                        Beacon = "beacon"
                    }
                },
                Force = 0.1f
            },
            MoveAwayFrom = new()
            {
                Objects = new[]
                {
                    new ObjectMove
                    {
                        Id = "uuid",
                        Beacon = "beacon"
                    }
                },
                Force = 0.2f
            }
        };
        Assert.Equal(
            "nearText:{concepts: [\"a\",\"b\",\"c\"] distance:0.8 moveTo:{force:0.1 objects: [{id:\"uuid\"},{beacon:\"beacon\"}]} " +
            "moveAwayFrom:{force:0.2 objects: [{id:\"uuid\" beacon:\"beacon\"}]}}", nearText.ToString());
    }
}
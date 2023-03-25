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


using System.Reflection;
using Xunit;

namespace SearchPioneer.Weaviate.Client.Tests.V1.Graph.Query.Builder;

public class GetTests : TestBase
{
	private const string base64File =
		"iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAIAAACQd1PeAAABhGlDQ1BJQ0MgcHJvZmlsZQAAKJF9kT1Iw0AcxV/TSou0ONhBxCFD62RBVMRRq1CECqFWaNXB5NIvaNKSpLg4Cq4FBz8Wqw4uzro6uAqC4AeIm5uToouU+L+k0CLGg+N+vLv3uHsHCK0q08zAOKDplpFJJcVcflUMviKACEKIwy8zsz4nSWl4jq97+Ph6l+BZ3uf+HBG1YDLAJxLPsrphEW8QT29adc77xFFWllXic+Ixgy5I/Mh1xeU3ziWHBZ4ZNbKZeeIosVjqYaWHWdnQiKeIY6qmU76Qc1nlvMVZqzZY5578heGCvrLMdZojSGERS5AgQkEDFVRhIUGrToqJDO0nPfzDjl8il0KuChg5FlCDBtnxg//B727N4uSEmxROAn0vtv0RB4K7QLtp29/Htt0+AfzPwJXe9ddawMwn6c2uFjsCBraBi+uupuwBlzvA0FNdNmRH8tMUikXg/Yy+KQ8M3gL9a25vnX2cPgBZ6ip9AxwcAqMlyl73eHeot7d/z3T6+wEPO3J/B8olWgAAAAlwSFlzAAAuIwAALiMBeKU/dgAAAAd0SU1FB+UEDQgmFS2naPsAAAAZdEVYdENvbW1lbnQAQ3JlYXRlZCB3aXRoIEdJTVBXgQ4XAAAADElEQVQI12NgYGAAAAAEAAEnNCcKAAAAAElFTkSuQmCC";

	private readonly FileInfo _imageFile = new(Assembly.GetExecutingAssembly().Location + "../../../../../pixel.png");

	[Fact]
	public void SimpleGet()
	{
		var query = new Get { Class = CLASS_NAME_PIZZA, Fields = new[] { new Field { Name = "name" } } };
		Assert.Equal("{Get{Pizza{name}}}", query.ToString());
	}

	[Fact]
	public void GetMultipleFields()
	{
		var query = new Get
		{
			Class = CLASS_NAME_PIZZA,
			Fields = new[] { new Field { Name = "name" }, new Field { Name = "description" } }
		};
		Assert.Equal("{Get{Pizza{name description}}}", query.ToString());
	}

	[Fact]
	public void GetWhereFilter()
	{
		var query = new Get
		{
			Class = CLASS_NAME_PIZZA,
			Where = new() { Path = new[] { "name" }, ValueString = "Hawaii", Operator = Operator.Equal },
			Fields = new[] { new Field { Name = "name" } }
		};
		Assert.Equal("{Get{Pizza(where:{path:[\"name\"] valueString:\"Hawaii\" operator:Equal}){name}}}",
			query.ToString());
	}

	[Fact]
	public void GetWhereFilterComplex()
	{
		var query = new Get
		{
			Class = CLASS_NAME_PIZZA,
			Where = new()
			{
				Operator = Operator.Or,
				Operands = new[]
				{
					new Where
					{
						Path = new[] { "name" }, ValueString = "Hawaii", Operator = Operator.Equal
					},
					new Where { Path = new[] { "name" }, ValueString = "Doener", Operator = Operator.Equal }
				}
			},
			Fields = new[] { new Field { Name = "name" } }
		};
		Assert.Equal(
			"{Get{Pizza(where:{operator:Or operands:[{path:[\"name\"] valueString:\"Hawaii\" operator:Equal},{path:[\"name\"] valueString:\"Doener\" operator:Equal}]}){name}}}",
			query.ToString());
	}

	[Fact]
	public void GetWithLimit()
	{
		var query = new Get { Class = CLASS_NAME_PIZZA, Limit = 2, Fields = new[] { new Field { Name = "name" } } };
		Assert.Equal("{Get{Pizza(limit:2){name}}}", query.ToString());
	}

	[Fact]
	public void GetWithLimitAndOffset()
	{
		var query = new Get
		{
			Class = CLASS_NAME_PIZZA, Limit = 2, Offset = 0, Fields = new[] { new Field { Name = "name" } }
		};
		Assert.Equal("{Get{Pizza(limit:2,offset:0){name}}}", query.ToString());
	}

	[Fact]
	public void GetWithNearText()
	{
		var query = new Get
		{
			Class = CLASS_NAME_PIZZA,
			NearText = new() { Concepts = new[] { "good" } },
			Fields = new[] { new Field { Name = "name" } }
		};
		Assert.Equal("{Get{Pizza(nearText:{concepts: [\"good\"]}){name}}}", query.ToString());
	}

	[Fact]
	public void GetWithNearVectorAndCertainty()
	{
		var query = new Get
		{
			Class = CLASS_NAME_PIZZA,
			NearVector = new() { Vector = new[] { 0f, 1f, 0.8f }, Certainty = 0.8f },
			Fields = new[] { new Field { Name = "name" } }
		};
		Assert.Equal("{Get{Pizza(nearVector:{vector: [0,1,0.8] certainty:0.8}){name}}}", query.ToString());
	}

	[Fact]
	public void GetWithNearVectorAndDistance()
	{
		var query = new Get
		{
			Class = CLASS_NAME_PIZZA,
			NearVector = new() { Vector = new[] { 0f, 1f, 0.8f }, Distance = 0.8f },
			Fields = new[] { new Field { Name = "name" } }
		};
		Assert.Equal("{Get{Pizza(nearVector:{vector: [0,1,0.8] distance:0.8}){name}}}", query.ToString());
	}

	[Fact]
	public void GetWithGroupFilter()
	{
		var query = new Get
		{
			Class = CLASS_NAME_PIZZA,
			Group = new() { Type = GroupType.Closest, Force = 0.4f },
			Fields = new[] { new Field { Name = "name" } }
		};
		Assert.Equal("{Get{Pizza(group:{type:closest force:0.4}){name}}}", query.ToString());
	}

	[Fact]
	public void GetWithMultipleFilter()
	{
		var query = new Get
		{
			Class = CLASS_NAME_PIZZA,
			Where = new() { Path = new[] { "name" }, ValueString = "Hawaii", Operator = Operator.Equal },
			NearText = new() { Concepts = new[] { "good" } },
			Limit = 2,
			Fields = new[] { new Field { Name = "name" } }
		};
		Assert.Equal(
			"{Get{Pizza(where:{path:[\"name\"] valueString:\"Hawaii\" operator:Equal},nearText:{concepts: [\"good\"]},limit:2){name}}}",
			query.ToString());
	}

	[Fact]
	public void GetWithNearTextWithConcepts()
	{
		var query = new Get
		{
			Class = CLASS_NAME_PIZZA,
			NearText = new() { Concepts = new[] { "good" } },
			Fields = new[] { new Field { Name = "name" } }
		};
		Assert.Equal("{Get{Pizza(nearText:{concepts: [\"good\"]}){name}}}", query.ToString());
	}

	[Fact]
	public void GetWithAsk()
	{
		var query = new Get
		{
			Class = CLASS_NAME_PIZZA,
			Ask = new() { Question = "Who are you?" },
			Fields = new[] { new Field { Name = "name" } }
		};
		Assert.Equal("{Get{Pizza(ask:{question:\"Who are you?\"}){name}}}", query.ToString());
	}

	[Fact]
	public void GetWithAskAndProperties()
	{
		var query = new Get
		{
			Class = CLASS_NAME_PIZZA,
			Ask = new() { Question = "Who are you?", Properties = new[] { "prop1", "prop2" } },
			Fields = new[] { new Field { Name = "name" } }
		};
		Assert.Equal("{Get{Pizza(ask:{question:\"Who are you?\" properties: [\"prop1\",\"prop2\"]}){name}}}",
			query.ToString());
	}

	[Fact]
	public void GetWithAskAndPropertiesAndCertainty()
	{
		var query = new Get
		{
			Class = CLASS_NAME_PIZZA,
			Ask = new() { Question = "Who are you?", Properties = new[] { "prop1", "prop2" }, Certainty = 0.1f },
			Fields = new[] { new Field { Name = "name" } }
		};
		Assert.Equal(
			"{Get{Pizza(ask:{question:\"Who are you?\" properties: [\"prop1\",\"prop2\"] certainty:0.1}){name}}}",
			query.ToString());
	}

	[Fact]
	public void GetWithAskAndPropertiesAndCertaintyAndRerank()
	{
		var query = new Get
		{
			Class = CLASS_NAME_PIZZA,
			Ask = new()
			{
				Question = "Who are you?",
				Properties = new[] { "prop1", "prop2" },
				Certainty = 0.1f,
				Rerank = true
			},
			Fields = new[] { new Field { Name = "name" } }
		};
		Assert.Equal(
			"{Get{Pizza(ask:{question:\"Who are you?\" properties: [\"prop1\",\"prop2\"] certainty:0.1 rerank:true}){name}}}",
			query.ToString());
	}

	[Fact]
	public void GetWithAskAndDistance()
	{
		var query = new Get
		{
			Class = CLASS_NAME_PIZZA,
			Ask = new() { Question = "Who are you?", Distance = 0.1f },
			Fields = new[] { new Field { Name = "name" } }
		};
		Assert.Equal("{Get{Pizza(ask:{question:\"Who are you?\" distance:0.1}){name}}}", query.ToString());
	}

	[Fact]
	public void GetWithAskAndPropertiesAndDistance()
	{
		var query = new Get
		{
			Class = CLASS_NAME_PIZZA,
			Ask = new() { Question = "Who are you?", Properties = new[] { "prop1", "prop2" }, Distance = 0.1f },
			Fields = new[] { new Field { Name = "name" } }
		};
		Assert.Equal(
			"{Get{Pizza(ask:{question:\"Who are you?\" properties: [\"prop1\",\"prop2\"] distance:0.1}){name}}}",
			query.ToString());
	}

	[Fact]
	public void GetWithAskAndPropertiesAndDistanceAndRerank()
	{
		var query = new Get
		{
			Class = CLASS_NAME_PIZZA,
			Ask = new()
			{
				Question = "Who are you?",
				Properties = new[] { "prop1", "prop2" },
				Distance = 0.1f,
				Rerank = true
			},
			Fields = new[] { new Field { Name = "name" } }
		};
		Assert.Equal(
			"{Get{Pizza(ask:{question:\"Who are you?\" properties: [\"prop1\",\"prop2\"] distance:0.1 rerank:true}){name}}}",
			query.ToString());
	}

	[Fact]
	public void GetWithNearImage()
	{
		var query = new Get
		{
			Class = CLASS_NAME_PIZZA,
			NearImage = new() { ImageFile = _imageFile },
			Fields = new[] { new Field { Name = "name" } }
		};
		Assert.Equal($"{{Get{{Pizza(nearImage:{{image:\"{base64File}\"}}){{name}}}}}}", query.ToString());
	}

	[Fact]
	public void GetWithNearImageAndCertainty()
	{
		var query = new Get
		{
			Class = CLASS_NAME_PIZZA,
			NearImage = new() { ImageFile = _imageFile, Certainty = 0.4f },
			Fields = new[] { new Field { Name = "name" } }
		};
		Assert.Equal($"{{Get{{Pizza(nearImage:{{image:\"{base64File}\" certainty:0.4}}){{name}}}}}}",
			query.ToString());
	}

	[Fact]
	public void GetWithNearImageAndCertaintyAndLimit()
	{
		var query = new Get
		{
			Class = CLASS_NAME_PIZZA,
			NearImage = new() { Image = "data:image/png;base64,iVBORw0KGgoAAAANS", Certainty = 0.1f },
			Fields = new[] { new Field { Name = "name" } }
		};

		Assert.Equal("{Get{Pizza(nearImage:{image:\"iVBORw0KGgoAAAANS\" certainty:0.1}){name}}}", query.ToString());
	}

	[Fact]
	public void GetWithNearImageAndDistance()
	{
		var query = new Get
		{
			Class = CLASS_NAME_PIZZA,
			NearImage = new() { ImageFile = _imageFile, Distance = 0.4f },
			Fields = new[] { new Field { Name = "name" } }
		};
		Assert.Equal($"{{Get{{Pizza(nearImage:{{image:\"{base64File}\" distance:0.4}}){{name}}}}}}",
			query.ToString());
	}

	[Fact]
	public void GetWithNearImageAndDistanceAndLimit()
	{
		var query = new Get
		{
			Class = CLASS_NAME_PIZZA,
			NearImage = new() { Image = "data:image/png;base64,iVBORw0KGgoAAAANS", Distance = 0.1f },
			Fields = new[] { new Field { Name = "name" } }
		};
		Assert.Equal("{Get{Pizza(nearImage:{image:\"iVBORw0KGgoAAAANS\" distance:0.1}){name}}}", query.ToString());
	}

	[Fact]
	public void GetWithSort()
	{
		var query = new Get
		{
			Class = CLASS_NAME_PIZZA,
			Sorts = new[] { new Sort { Path = new[] { "property1" } } },
			Fields = new[] { new Field { Name = "name" } }
		};
		Assert.Equal("{Get{Pizza(sort:[{path:[\"property1\"]}]){name}}}", query.ToString());
	}

	[Fact]
	public void GetWith2Sorts()
	{
		var query = new Get
		{
			Class = CLASS_NAME_PIZZA,
			Sorts = new[]
			{
				new Sort { Path = new[] { "property1" } },
				new Sort { Path = new[] { "property2" }, Order = SortOrder.Desc }
			},
			Fields = new[] { new Field { Name = "name" } }
		};
		Assert.Equal("{Get{Pizza(sort:[{path:[\"property1\"]},{path:[\"property2\"] order:desc}]){name}}}",
			query.ToString());
	}

	[Fact]
	public void GetWith3Sorts()
	{
		var query = new Get
		{
			Class = CLASS_NAME_PIZZA,
			Sorts = new[]
			{
				new Sort { Path = new[] { "property1" } },
				new Sort { Path = new[] { "property2" }, Order = SortOrder.Desc },
				new Sort { Path = new[] { "property3" }, Order = SortOrder.Asc }
			},
			Fields = new[] { new Field { Name = "name" } }
		};
		Assert.Equal(
			"{Get{Pizza(sort:[{path:[\"property1\"]},{path:[\"property2\"] order:desc},{path:[\"property3\"] order:asc}]){name}}}",
			query.ToString());
	}
}

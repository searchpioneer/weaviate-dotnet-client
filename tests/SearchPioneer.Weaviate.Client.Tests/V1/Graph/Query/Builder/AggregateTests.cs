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


using Xunit;

namespace SearchPioneer.Weaviate.Client.Tests.V1.Graph.Query.Builder;

public class AggregateTests : TestBase
{
	[Fact]
	public void SimpleAggregate()
	{
		var query = new Aggregate
		{
			Class = CLASS_NAME_PIZZA,
			Fields = new[] { new Field { Name = "meta", Fields = new[] { new Field { Name = "count" } } } }
		};

		Assert.Equal("{Aggregate{Pizza{meta{count}}}}", query.ToString());
	}

	[Fact]
	public void AggregateWithGroupBy()
	{
		var query = new Aggregate
		{
			Class = CLASS_NAME_PIZZA,
			GroupBy = "name",
			Fields = new[]
			{
				new Field { Name = "groupedBy", Fields = new[] { new Field { Name = "value" } } },
				new Field { Name = "name", Fields = new[] { new Field { Name = "count" } } }
			}
		};
		Assert.Equal("{Aggregate{Pizza(groupBy:\"name\"){groupedBy{value} name{count}}}}", query.ToString());
	}

	[Fact]
	public void AggregateWithGroupByAndLimit()
	{
		var query = new Aggregate
		{
			Class = CLASS_NAME_PIZZA,
			GroupBy = "name",
			Fields = new[]
			{
				new Field { Name = "groupedBy", Fields = new[] { new Field { Name = "value" } } },
				new Field { Name = "name", Fields = new[] { new Field { Name = "count" } } }
			},
			Limit = 10
		};
		Assert.Equal("{Aggregate{Pizza(groupBy:\"name\",limit:10){groupedBy{value} name{count}}}}",
			query.ToString());
	}

	[Fact]
	public void AggregateWithWhere()
	{
		var query = new Aggregate
		{
			Class = CLASS_NAME_PIZZA,
			Where = new() { Path = new[] { "name" }, ValueString = "Hawaii", Operator = Operator.Equal },
			Fields = new[] { new Field { Name = "meta", Fields = new[] { new Field { Name = "count" } } } }
		};

		Assert.Equal("{Aggregate{Pizza(where:{path:[\"name\"] valueString:\"Hawaii\" operator:Equal}){meta{count}}}}",
			query.ToString());
	}

	[Fact]
	public void AggregateWithWhereAndGroupedBy()
	{
		var query = new Aggregate
		{
			Class = CLASS_NAME_PIZZA,
			GroupBy = "name",
			Where = new() { Path = new[] { "name" }, ValueString = "Hawaii", Operator = Operator.Equal },
			Fields = new[] { new Field { Name = "meta", Fields = new[] { new Field { Name = "count" } } } }
		};

		Assert.Equal(
			"{Aggregate{Pizza(groupBy:\"name\",where:{path:[\"name\"] valueString:\"Hawaii\" operator:Equal}){meta{count}}}}",
			query.ToString());
	}

	[Fact]
	public void AggregateWithNearVectorAndCertainty()
	{
		var query = new Aggregate
		{
			Class = CLASS_NAME_PIZZA,
			NearVector = new() { Vector = new[] { 0f, 1f, 0.8f }, Certainty = 0.8f },
			Fields = new[] { new Field { Name = "meta", Fields = new[] { new Field { Name = "count" } } } }
		};
		Assert.Equal("{Aggregate{Pizza(nearVector:{vector: [0,1,0.8] certainty:0.8}){meta{count}}}}",
			query.ToString());
	}

	[Fact]
	public void AggregateWithNearVectorAndDistance()
	{
		var query = new Aggregate
		{
			Class = CLASS_NAME_PIZZA,
			NearVector = new() { Vector = new[] { 0f, 1f, 0.8f }, Distance = 0.8f },
			Fields = new[] { new Field { Name = "meta", Fields = new[] { new Field { Name = "count" } } } }
		};
		Assert.Equal("{Aggregate{Pizza(nearVector:{vector: [0,1,0.8] distance:0.8}){meta{count}}}}",
			query.ToString());
	}

	[Fact]
	public void AggregateWithNearObjectAndCertainty()
	{
		var query = new Aggregate
		{
			Class = CLASS_NAME_PIZZA,
			NearObject = new()
			{
				Id = "some-uuid", // TODO! cleanup uuid, some-uuid and id all over the codebase
				Certainty = 0.8f
			},
			Fields = new[] { new Field { Name = "meta", Fields = new[] { new Field { Name = "count" } } } }
		};
		Assert.Equal("{Aggregate{Pizza(nearObject:{id:\"some-uuid\" certainty:0.8}){meta{count}}}}",
			query.ToString());
	}

	[Fact]
	public void AggregateWithNearObjectAndDistance()
	{
		var query = new Aggregate
		{
			Class = CLASS_NAME_PIZZA,
			NearObject = new() { Id = "some-uuid", Distance = 0.8f },
			Fields = new[] { new Field { Name = "meta", Fields = new[] { new Field { Name = "count" } } } }
		};
		Assert.Equal("{Aggregate{Pizza(nearObject:{id:\"some-uuid\" distance:0.8}){meta{count}}}}",
			query.ToString());
	}

	[Fact]
	public void AggregateWithAskAndCertaintyAndRerank()
	{
		var query = new Aggregate
		{
			Class = CLASS_NAME_PIZZA,
			Ask = new() { Question = "question?", Certainty = 0.8f, Rerank = true },
			Fields = new[] { new Field { Name = "meta", Fields = new[] { new Field { Name = "count" } } } }
		};
		Assert.Equal("{Aggregate{Pizza(ask:{question:\"question?\" certainty:0.8 rerank:true}){meta{count}}}}",
			query.ToString());
	}

	[Fact]
	public void AggregateWithAskAndDistanceAndRerank()
	{
		var query = new Aggregate
		{
			Class = CLASS_NAME_PIZZA,
			Ask = new() { Question = "question?", Distance = 0.8f, Rerank = true },
			Fields = new[] { new Field { Name = "meta", Fields = new[] { new Field { Name = "count" } } } }
		};
		Assert.Equal("{Aggregate{Pizza(ask:{question:\"question?\" distance:0.8 rerank:true}){meta{count}}}}",
			query.ToString());
	}

	[Fact]
	public void AggregateWithNearImageAndCertainty()
	{
		var query = new Aggregate
		{
			Class = CLASS_NAME_PIZZA,
			NearImage = new() { Image = "iVBORw0KGgoAAAANS", Certainty = 0.8f },
			Fields = new[] { new Field { Name = "meta", Fields = new[] { new Field { Name = "count" } } } }
		};
		Assert.Equal("{Aggregate{Pizza(nearImage:{image:\"iVBORw0KGgoAAAANS\" certainty:0.8}){meta{count}}}}",
			query.ToString());
	}

	[Fact]
	public void AggregateWithNearImage()
	{
		var query = new Aggregate
		{
			Class = CLASS_NAME_PIZZA,
			NearImage = new() { Image = "iVBORw0KGgoAAAANS", Distance = 0.8f },
			Fields = new[] { new Field { Name = "meta", Fields = new[] { new Field { Name = "count" } } } }
		};
		Assert.Equal("{Aggregate{Pizza(nearImage:{image:\"iVBORw0KGgoAAAANS\" distance:0.8}){meta{count}}}}",
			query.ToString());
	}

	[Fact]
	public void AggregateWithObjectLimitAndCertainty()
	{
		var query = new Aggregate
		{
			Class = CLASS_NAME_PIZZA,
			NearImage = new() { Image = "iVBORw0KGgoAAAANS", Certainty = 0.8f },
			ObjectLimit = 100,
			Fields = new[] { new Field { Name = "meta", Fields = new[] { new Field { Name = "count" } } } }
		};
		Assert.Equal(
			"{Aggregate{Pizza(nearImage:{image:\"iVBORw0KGgoAAAANS\" certainty:0.8},objectLimit:100){meta{count}}}}",
			query.ToString());
	}

	[Fact]
	public void AggregateWithObjectLimitAndDistance()
	{
		var query = new Aggregate
		{
			Class = CLASS_NAME_PIZZA,
			NearImage = new() { Image = "iVBORw0KGgoAAAANS", Distance = 0.8f },
			ObjectLimit = 100,
			Fields = new[] { new Field { Name = "meta", Fields = new[] { new Field { Name = "count" } } } }
		};
		Assert.Equal(
			"{Aggregate{Pizza(nearImage:{image:\"iVBORw0KGgoAAAANS\" distance:0.8},objectLimit:100){meta{count}}}}",
			query.ToString());
	}
}

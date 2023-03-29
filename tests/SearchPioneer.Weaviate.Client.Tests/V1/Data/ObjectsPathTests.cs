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


using Moq;
using Xunit;

namespace SearchPioneer.Weaviate.Client.Tests.V1.Data;

public class ObjectsPathTests
{
	private static readonly ObjectPathParams Empty = new();

	private static readonly ObjectPathParams ClassParams = new() { Class = "someClass" };

	private static readonly ObjectPathParams IdParams = new() { Id = "someId" };

	private static readonly ObjectPathParams ClassQueryParams = new() { Class = "someClass", Limit = 100 };

	private static readonly ObjectPathParams AllParams = new()
	{
		Class = "someClass", Id = "someId", Limit = 100, Additional = new[] { "additional1", "additional2" }
	};

	private static readonly ObjectPathParams ConsistencyLevelClassId = new()
	{
		Class = "someClass", Id = "someId", ConsistencyLevel = ConsistencyLevel.Quorum
	};

	private static readonly ObjectPathParams NodeNameClassId = new()
	{
		Class = "someClass", Id = "someId", NodeName = "node1"
	};

	private static readonly ObjectPathParams ConsistencyLevelAllParams = new()
	{
		Class = "someClass",
		Id = "someId",
		Limit = 100,
		Additional = new[] { "additional1", "additional2" },
		ConsistencyLevel = ConsistencyLevel.Quorum
	};

	private static readonly ObjectPathParams NodeNameAllParams = new()
	{
		Class = "someClass",
		Id = "someId",
		Limit = 100,
		Additional = new[] { "additional1", "additional2" },
		NodeName = "node1",
		ConsistencyLevel = ConsistencyLevel.Quorum
	};

	[Fact]
	public void BuildCreatePathsWhenSupported()
	{
		var support = new Mock<DbVersionSupport>();
		support.Setup(v => v.SupportsClassNameNamespacedEndpoints()).Returns(true);
		Assert.Equal("/objects", new ObjectsPath(support.Object).BuildCreate(Empty, out _));
		Assert.Equal("/objects", new ObjectsPath(support.Object).BuildCreate(AllParams, out _));
	}

	[Fact]
	public void BuildCreatePathsWhenNotSupported()
	{
		var support = new Mock<DbVersionSupport>();
		support.Setup(v => v.SupportsClassNameNamespacedEndpoints()).Returns(false);
		Assert.Equal("/objects", new ObjectsPath(support.Object).BuildCreate(Empty, out _));
		Assert.Equal("/objects", new ObjectsPath(support.Object).BuildCreate(AllParams, out _));
	}

	[Fact]
	public void BuildDeletePathsWhenSupported()
	{
		var support = new Mock<DbVersionSupport>();
		support.Setup(v => v.SupportsClassNameNamespacedEndpoints()).Returns(true);
		Assert.Equal("/objects", new ObjectsPath(support.Object).BuildDelete(Empty, out _));
		Assert.Equal("/objects/someClass", new ObjectsPath(support.Object).BuildDelete(ClassParams, out _));
		Assert.Equal("/objects/someId", new ObjectsPath(support.Object).BuildDelete(IdParams, out _));
		Assert.Equal("/objects/someClass/someId", new ObjectsPath(support.Object).BuildDelete(AllParams, out _));
	}

	[Fact]
	public void BuildDeletePathsWhenNotSupported()
	{
		var support = new Mock<DbVersionSupport>();
		support.Setup(v => v.SupportsClassNameNamespacedEndpoints()).Returns(false);
		Assert.Equal("/objects", new ObjectsPath(support.Object).BuildDelete(Empty, out _));
		Assert.Equal("/objects", new ObjectsPath(support.Object).BuildDelete(ClassParams, out _));
		Assert.Equal("/objects/someId", new ObjectsPath(support.Object).BuildDelete(IdParams, out _));
		Assert.Equal("/objects/someId", new ObjectsPath(support.Object).BuildDelete(AllParams, out _));
	}

	[Fact]
	public void BuildCheckPathsWhenSupported()
	{
		var support = new Mock<DbVersionSupport>();
		support.Setup(v => v.SupportsClassNameNamespacedEndpoints()).Returns(true);
		Assert.Equal("/objects", new ObjectsPath(support.Object).BuildCheck(Empty, out _));
		Assert.Equal("/objects/someClass", new ObjectsPath(support.Object).BuildCheck(ClassParams, out _));
		Assert.Equal("/objects/someId", new ObjectsPath(support.Object).BuildCheck(IdParams, out _));
		Assert.Equal("/objects/someClass/someId", new ObjectsPath(support.Object).BuildCheck(AllParams, out _));
	}

	[Fact]
	public void BuildCheckPathsWhenNotSupported()
	{
		var support = new Mock<DbVersionSupport>();
		support.Setup(v => v.SupportsClassNameNamespacedEndpoints()).Returns(false);
		Assert.Equal("/objects", new ObjectsPath(support.Object).BuildCheck(Empty, out _));
		Assert.Equal("/objects", new ObjectsPath(support.Object).BuildCheck(ClassParams, out _));
		Assert.Equal("/objects/someId", new ObjectsPath(support.Object).BuildCheck(IdParams, out _));
		Assert.Equal("/objects/someId", new ObjectsPath(support.Object).BuildCheck(AllParams, out _));
	}

	[Fact]
	public void BuildGetOnePathsWhenSupported()
	{
		var support = new Mock<DbVersionSupport>();
		support.Setup(v => v.SupportsClassNameNamespacedEndpoints()).Returns(true);
		Assert.Equal("/objects", new ObjectsPath(support.Object).BuildGetOne(Empty, out _));
		Assert.Equal("/objects/someClass", new ObjectsPath(support.Object).BuildGetOne(ClassParams, out _));
		Assert.Equal("/objects/someId", new ObjectsPath(support.Object).BuildGetOne(IdParams, out _));
		Assert.Equal("/objects/someClass/someId?include=additional1,additional2",
			new ObjectsPath(support.Object).BuildGetOne(AllParams, out _));
		Assert.Equal("/objects/someClass/someId?consistency_level=QUORUM",
			new ObjectsPath(support.Object).BuildGetOne(ConsistencyLevelClassId, out _));
		Assert.Equal("/objects/someClass/someId?node_name=node1",
			new ObjectsPath(support.Object).BuildGetOne(NodeNameClassId, out _));
		Assert.Equal("/objects/someClass/someId?include=additional1,additional2&consistency_level=QUORUM",
			new ObjectsPath(support.Object).BuildGetOne(ConsistencyLevelAllParams, out _));
		Assert.Equal("/objects/someClass/someId?include=additional1,additional2&consistency_level=QUORUM&node_name=node1",
			new ObjectsPath(support.Object).BuildGetOne(NodeNameAllParams, out _));
	}

	[Fact]
	public void BuildGetOnePathsWhenNotSupported()
	{
		var support = new Mock<DbVersionSupport>();
		support.Setup(v => v.SupportsClassNameNamespacedEndpoints()).Returns(false);
		Assert.Equal("/objects", new ObjectsPath(support.Object).BuildGetOne(Empty, out _));
		Assert.Equal("/objects", new ObjectsPath(support.Object).BuildGetOne(ClassParams, out _));
		Assert.Equal("/objects/someId", new ObjectsPath(support.Object).BuildGetOne(IdParams, out _));
		Assert.Equal("/objects/someId?include=additional1,additional2",
			new ObjectsPath(support.Object).BuildGetOne(AllParams, out _));
	}

	[Fact]
	public void BuildGetPathsWhenSupported()
	{
		var support = new Mock<DbVersionSupport>();
		support.Setup(v => v.SupportsClassNameNamespacedEndpoints()).Returns(true);
		Assert.Equal("/objects", new ObjectsPath(support.Object).BuildGet(Empty, out _));
		Assert.Equal("/objects?class=someClass", new ObjectsPath(support.Object).BuildGet(ClassParams, out _));
		Assert.Equal("/objects", new ObjectsPath(support.Object).BuildGet(IdParams, out _));
		Assert.Equal("/objects?include=additional1,additional2&limit=100",
			new ObjectsPath(support.Object).BuildGet(AllParams, out _));
		Assert.Equal("/objects?class=someClass&limit=100",
			new ObjectsPath(support.Object).BuildGet(ClassQueryParams, out _));
	}

	[Fact]
	public void BuildGetPathsWhenNotSupported()
	{
		var support = new Mock<DbVersionSupport>();
		support.Setup(v => v.SupportsClassNameNamespacedEndpoints()).Returns(false);
		Assert.Equal("/objects", new ObjectsPath(support.Object).BuildGet(Empty, out _));
		Assert.Equal("/objects", new ObjectsPath(support.Object).BuildGet(ClassParams, out _));
		Assert.Equal("/objects", new ObjectsPath(support.Object).BuildGet(IdParams, out _));
		Assert.Equal("/objects?include=additional1,additional2&limit=100",
			new ObjectsPath(support.Object).BuildGet(AllParams, out _));
		Assert.Equal("/objects?limit=100", new ObjectsPath(support.Object).BuildGet(ClassQueryParams, out _));
	}

	[Fact]
	public void BuildUpdatePathsWhenSupported()
	{
		var support = new Mock<DbVersionSupport>();
		support.Setup(v => v.SupportsClassNameNamespacedEndpoints()).Returns(true);
		Assert.Equal("/objects", new ObjectsPath(support.Object).BuildUpdate(Empty, out _));
		Assert.Equal("/objects/someClass", new ObjectsPath(support.Object).BuildUpdate(ClassParams, out _));
		Assert.Equal("/objects/someId", new ObjectsPath(support.Object).BuildUpdate(IdParams, out _));
		Assert.Equal("/objects/someClass/someId", new ObjectsPath(support.Object).BuildUpdate(AllParams, out _));
	}

	[Fact]
	public void BuildUpdatePathsWhenNotSupported()
	{
		var support = new Mock<DbVersionSupport>();
		support.Setup(v => v.SupportsClassNameNamespacedEndpoints()).Returns(false);
		Assert.Equal("/objects", new ObjectsPath(support.Object).BuildUpdate(Empty, out _));
		Assert.Equal("/objects", new ObjectsPath(support.Object).BuildUpdate(ClassParams, out _));
		Assert.Equal("/objects/someId", new ObjectsPath(support.Object).BuildUpdate(IdParams, out _));
		Assert.Equal("/objects/someId", new ObjectsPath(support.Object).BuildUpdate(AllParams, out _));
	}
}

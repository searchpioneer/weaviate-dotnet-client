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

public class ReferencesPathTests
{
	private static readonly ReferencePathParams Empty = new();

	private static readonly ReferencePathParams ClassParams = new() { Class = "someClass" };

	private static readonly ReferencePathParams IdParams = new() { Id = "someId" };

	private static readonly ReferencePathParams PropertyParams = new() { Property = "someProperty" };

	private static readonly ReferencePathParams AllParams = new()
	{
		Class = "someClass", Id = "someId", Property = "someProperty", ConsistencyLevel = ConsistencyLevel.Quorum
	};

	[Fact]
	public void BuildCreatePathsWhenSupported()
	{
		var support = new Mock<DbVersionSupport>();
		support.Setup(v => v.SupportsClassNameNamespacedEndpoints()).Returns(true);
		Assert.Equal("/objects/references", new ReferencesPath(support.Object).BuildCreate(Empty, out _));
		Assert.Equal("/objects/someClass/references", new ReferencesPath(support.Object).BuildCreate(ClassParams, out _));
		Assert.Equal("/objects/someId/references", new ReferencesPath(support.Object).BuildCreate(IdParams, out _));
		Assert.Equal("/objects/references/someProperty",
			new ReferencesPath(support.Object).BuildCreate(PropertyParams, out _));
		Assert.Equal("/objects/someClass/someId/references/someProperty?consistency_level=QUORUM",
			new ReferencesPath(support.Object).BuildCreate(AllParams, out _));
	}

	[Fact]
	public void BuildCreatePathsWhenNotSupported()
	{
		var support = new Mock<DbVersionSupport>();
		support.Setup(v => v.SupportsClassNameNamespacedEndpoints()).Returns(false);
		Assert.Equal("/objects/references", new ReferencesPath(support.Object).BuildCreate(Empty, out _));
		Assert.Equal("/objects/references", new ReferencesPath(support.Object).BuildCreate(ClassParams, out _));
		Assert.Equal("/objects/someId/references", new ReferencesPath(support.Object).BuildCreate(IdParams, out _));
		Assert.Equal("/objects/references/someProperty",
			new ReferencesPath(support.Object).BuildCreate(PropertyParams, out _));
		Assert.Equal("/objects/someId/references/someProperty?consistency_level=QUORUM",
			new ReferencesPath(support.Object).BuildCreate(AllParams, out _));
	}
}

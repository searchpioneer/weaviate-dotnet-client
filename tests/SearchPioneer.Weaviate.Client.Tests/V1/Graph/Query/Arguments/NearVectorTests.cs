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

namespace SearchPioneer.Weaviate.Client.Tests.V1.Graph.Query.Arguments;

public class NearVectorTests : TestBase
{
	[Fact]
	public void WithCertainty()
	{
		var nearVector = new NearVector { Vector = new[] { 1f, 2f, 3f }, Certainty = 0.8f };
		Assert.Equal("nearVector:{vector: [1,2,3] certainty:0.8}", nearVector.ToString());
	}

	[Fact]
	public void WithDistance()
	{
		var nearVector = new NearVector { Vector = new[] { 1f, 2f, 3f }, Distance = 0.8f };
		Assert.Equal("nearVector:{vector: [1,2,3] distance:0.8}", nearVector.ToString());
	}

	[Fact]
	public void WithNoCertainty()
	{
		var nearVector = new NearVector { Vector = new[] { 1f, 2f, 3f } };
		Assert.Equal("nearVector:{vector: [1,2,3]}", nearVector.ToString());
	}

	[Fact]
	public void WithNoCertaintyDecimal()
	{
		var nearVector = new NearVector { Vector = new[] { 1.1f, 2.1f, 3.1f } };
		Assert.Equal("nearVector:{vector: [1.1,2.1,3.1]}", nearVector.ToString());
	}
}

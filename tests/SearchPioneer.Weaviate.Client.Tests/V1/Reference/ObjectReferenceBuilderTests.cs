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

namespace SearchPioneer.Weaviate.Client.Tests.V1.Reference;

public class ObjectReferenceBuilderTests
{
	[Fact]
	public void BeaconFromBeaconPath()
	{
		var beaconPathMock = new Mock<BeaconPath>();

		var warnings = new List<string>();
		beaconPathMock.Setup(b => b.BuildSingle(It.IsAny<BeaconPathParams>(), out warnings))
			.Returns("weaviate://beacon-mock-single");

		var builder = ReferenceApi.Reference(out warnings, beaconPathMock.Object);

		Assert.Equal("weaviate://beacon-mock-single", builder.Beacon);
	}

	[Fact]
	public void DeprecatedBeacon()
	{
		var warnings = new List<string>();
		var builder = ReferenceApi.Reference(out warnings, null, "someClass", "someId");
		Assert.Equal("weaviate://localhost/someId", builder.Beacon);
	}
}

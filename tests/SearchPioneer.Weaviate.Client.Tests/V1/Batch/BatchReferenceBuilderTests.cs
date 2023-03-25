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

namespace SearchPioneer.Weaviate.Client.Tests.V1.Batch;

public class BatchReferenceBuilderTests : TestBase
{
	[Fact]
	public void BeaconsFromBeaconPath()
	{
		var beaconPathMock = new Mock<BeaconPath>();

		var warnings = new List<string>();
		beaconPathMock.Setup(b => b.BuildBatchFrom(It.IsAny<BeaconPathParams>(), out warnings))
			.Returns("weaviate://beacon-mock-batch-from");
		beaconPathMock.Setup(b => b.BuildBatchTo(It.IsAny<BeaconPathParams>(), out warnings))
			.Returns("weaviate://beacon-mock-batch-to");

		var builder = BatchApi.Reference(out warnings,
			beaconPathMock.Object,
			"someFromClass",
			"someToClass",
			"someFromProperty",
			"someFromId",
			"someToId");

		Assert.Equal("weaviate://beacon-mock-batch-from", builder!.From);
		Assert.Equal("weaviate://beacon-mock-batch-to", builder.To);
	}

	[Fact]
	public void DeprecatedBeacons()
	{
		var builder = BatchApi.Reference(out _,
			null,
			"someFromClass",
			"someToClass",
			"someFromProperty",
			"someFromId",
			"someToId");

		Assert.Equal("weaviate://localhost/someFromClass/someFromId/someFromProperty", builder!.From);
		Assert.Equal("weaviate://localhost/someToId", builder.To);
	}
}

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

namespace SearchPioneer.Weaviate.Client.IntegrationTests.Api.Cluster;

[Collection("Sequential")]
public class ClusterTests : TestBase
{
	[Fact]
	public void NodeStatusWithoutData()
	{
		Client.Schema.DeleteAllClasses();
		var nodeStatus = Client.Cluster.NodeStatus();
		Assert.True(nodeStatus.HttpStatusCode == 200);
		Assert.Equal(ExpectedVersion, nodeStatus.Result.Nodes.First().Version);
		Assert.Equal(ExpectedGithubHash, nodeStatus.Result.Nodes.First().GitHash);
		Assert.Equal(Status.Healthy, nodeStatus.Result.Nodes.First().Status);
		Assert.Equal(0, nodeStatus.Result.Nodes.First().Stats.ObjectCount);
		Assert.Equal(0, nodeStatus.Result.Nodes.First().Stats.ShardCount);
	}

	[Fact]
	public void NodeStatusWithData()
	{
		CreateTestSchemaAndData(Client);
		var nodeStatus = Client.Cluster.NodeStatus();
		Assert.True(nodeStatus.HttpStatusCode == 200);
		Assert.Equal(ExpectedVersion, nodeStatus.Result.Nodes.First().Version);
		Assert.Equal(ExpectedGithubHash, nodeStatus.Result.Nodes.First().GitHash);
		Assert.Equal(Status.Healthy, nodeStatus.Result.Nodes.First().Status);
		Assert.Equal(6, nodeStatus.Result.Nodes.First().Stats.ObjectCount);
		Assert.Equal(2, nodeStatus.Result.Nodes.First().Stats.ShardCount);
		Assert.Contains(CLASS_NAME_PIZZA, nodeStatus.Result.Nodes.First().Shards.Select(s => s.Class));
		Assert.Contains(CLASS_NAME_SOUP, nodeStatus.Result.Nodes.First().Shards.Select(s => s.Class));
		Assert.Equal(4,
			nodeStatus.Result.Nodes.First().Shards.Single(s => s.Class.Equals(CLASS_NAME_PIZZA)).ObjectCount);
		Assert.Equal(2,
			nodeStatus.Result.Nodes.First().Shards.Single(s => s.Class.Equals(CLASS_NAME_SOUP)).ObjectCount);
	}
}

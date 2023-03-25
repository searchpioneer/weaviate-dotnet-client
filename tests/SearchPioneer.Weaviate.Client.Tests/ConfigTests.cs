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

namespace SearchPioneer.Weaviate.Client.Tests;

public class ConfigTests : TestBase
{
	[Fact]
	public void TestConfig()
	{
		// given
		const string scheme = "https";
		const string domain = "localhost:8080";
		// when
		var config = new Config(scheme, domain);
		// then
		Assert.Equal("https://localhost:8080/v1", config.GetBaseUrl());
	}
}

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

namespace SearchPioneer.Weaviate.Client.IntegrationTest.Api.Misc;

[Collection("Sequential")]
public class MiscTests : TestBase
{
    [Fact]
    public void Liveness()
    {
        var liveness = Client.Misc.Live();
        Assert.True(liveness.Result);
    }

    [Fact]
    public void Ready()
    {
        var ready = Client.Misc.Ready();
        Assert.True(ready.Result);
    }

    [Fact]
    public void Meta()
    {
        var meta = Client.Misc.Meta();
        Assert.Equal("http://[::]:8080", meta.Result.Hostname);
        Assert.Equal(ExpectedVersion, meta.Result.Version);
        Assert.Equal(
            "{\"backup-filesystem\":{\"backupsPath\":\"/tmp/backups\"},\"text2vec-contextionary\":{\"version\":\"en0.16.0-v1.1.0\",\"wordCount\":818072}}",
            meta.Result.Modules.ToJsonString());
    }
}
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

using SearchPioneer.Weaviate.Client.Api.Backup.Model;

namespace SearchPioneer.Weaviate.Client.Api.Backup.Request;

public class BackupRequest
{
    public BackupRequest(Backend backend, string id)
    {
        ArgumentNullException.ThrowIfNull(nameof(id));

        Backend = backend;
        Id = id;
    }

    public Backend Backend { get; }
    public string Id { get; }

    public string[]? IncludeClasses { get; set; }
    public string[]? ExcludeClasses { get; set; }
    public bool WaitForCompletion { get; set; }
}
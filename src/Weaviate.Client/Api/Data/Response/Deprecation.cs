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

namespace SearchPioneer.Weaviate.Client.Api.Data.Response;

public class Deprecation
{
    public string ApiType { get; init; }
    public string Id { get; init; }
    public string[] Locations { get; init; }
    public string Mitigation { get; init; }
    public string Msg { get; init; }
    public string PlannedRemovalVersion { get; init; }
    public string RemovedIn { get; init; }
    public DateTime RemovedTime { get; init; }
    public DateTime SinceTime { get; init; }
    public string SinceVersion { get; init; }
    public string Status { get; init; }
}
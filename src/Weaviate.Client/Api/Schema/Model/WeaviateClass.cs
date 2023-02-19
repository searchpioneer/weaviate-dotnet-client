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

using SearchPioneer.Weaviate.Client.Api.Misc.Model;

namespace SearchPioneer.Weaviate.Client.Api.Schema.Model;

public class WeaviateClass
{
    public string Class { get; set; }
    public string Description { get; set; }
    public InvertedIndexConfig InvertedIndexConfig { get; set; }
    public object ModuleConfig { get; set; }
    public Property[] Properties { get; set; }
    public VectorIndexConfig VectorIndexConfig { get; set; }
    public ShardingConfig ShardingConfig { get; set; }
    public string VectorIndexType { get; set; }
    public string Vectorizer { get; set; }
    public ReplicationConfig ReplicationConfig { get; set; }
}
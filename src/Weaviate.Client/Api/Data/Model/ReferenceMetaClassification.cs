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

namespace SearchPioneer.Weaviate.Client.Api.Data.Model;

public class ReferenceMetaClassification
{
    public double ClosestLosingDistance { get; set; }
    public double ClosestOverallDistance { get; set; }
    public double ClosestWinningDistance { get; set; }
    public long LosingCount { get; set; }
    public double LosingDistance { get; set; }
    public double MeanLosingDistance { get; set; }
    public double MeanWinningDistance { get; set; }
    public long OverallCount { get; set; }
    public long WinningCount { get; set; }
    public double WinningDistance { get; set; }
}
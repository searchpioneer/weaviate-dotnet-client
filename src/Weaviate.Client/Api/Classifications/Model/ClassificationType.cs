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

namespace SearchPioneer.Weaviate.Client.Api.Classifications.Model;

public static class ClassificationType
{
    /// <summary>
    ///     KNN (k nearest neighbours) a non parametric classification based on training data
    /// </summary>
    public static string KNN = "knn";

    /// <summary>
    ///     Contextual classification labels a data object with the closest label based on
    ///     their vector position (which describes the context)
    /// </summary>
    public static string Contextual = "text2vec-contextionary";

    /// <summary>
    ///     ZeroShot classification labels a data object with the closest label based on
    ///     their vector position (which describes the context). It can be used with any
    ///     vectorizer or custom vectors.
    /// </summary>
    public static string ZeroShot = "zeroshot";
}
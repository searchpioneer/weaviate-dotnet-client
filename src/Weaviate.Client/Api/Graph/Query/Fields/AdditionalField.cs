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

namespace SearchPioneer.Weaviate.Client.Api.Graph.Query.Fields;

public static class AdditionalField
{
    /// <summary>
    ///     Timestamp of when the data object was created.
    /// </summary>
    public static Field CreationTime => new() { Name = "creationTimeUnix" };

    /// <summary>
    ///     Any time a vector search is involved, the distance can be displayed to show the distance between the query
    ///     vector and each result. The distance is the raw distance metric that was used as part of the vector search.
    ///     For example, if the distance metric is cosine, distance will return a number between 0 and 2.
    /// </summary>
    public static Field Distance => new() { Name = "distance" };

    /// <summary>
    ///     For a class with cosine distance metrics, the certainty is a normalization of the distance using the formula:
    ///     certainty = 1 - distance/2
    /// </summary>
    public static Field Certainty => new() { Name = "certainty" };

    /// <summary>
    ///     Unique UUID of the data object.
    /// </summary>
    public static Field Id => new() { Name = "id" };

    /// <summary>
    ///     Timestamp of when the data object was last updated.
    /// </summary>
    public static Field LastUpdateTime => new() { Name = "lastUpdateTimeUnix" };

    /// <summary>
    ///     Vector representation of the data object
    /// </summary>
    public static Field Vector => new() { Name = "vector" };

    /// <summary>
    ///     When a data-object has been subjected to classification, you can get additional information about
    ///     how the object was classified by running the following command:
    /// </summary>
    public static Field Classification(params AdditionalFieldClassification[] classifications)
    {
        return new()
        {
            Name = "classification",
            Fields = classifications.Select(c =>
            {
                var name = c.ToString("f");
                var camelCased = char.ToLowerInvariant(name[0]) + name[1..];
                return new Field
                {
                    Name = camelCased
                };
            }).ToArray()
        };
    }

    /// <summary>
    ///     The feature projection is intended to reduce the dimensionality of the object’s vector into something easily
    ///     suitable for visualizing, such as 2d or 3d. The underlying algorithm is exchangeable, the first algorithm
    ///     to be provided is t-SNE.
    /// </summary>
    /// <param name="dimensions">Target dimensionality, usually 2 or 3</param>
    /// <param name="algorithm">Algorithm to be used, currently supported: tsne</param>
    /// <param name="perplexity">
    ///     The t-SNE perplexity value, must be smaller than the n-1 where n is the number of results to be visualized
    /// </param>
    /// <param name="learningRate">The t-SNE learning rate</param>
    /// <param name="iterations">
    ///     The number of iterations the t-SNE algorithm runs. Higher values lead to more stable results at the cost of a
    ///     larger response time
    /// </param>
    /// <returns></returns>
    public static Field FeatureProjection(
        int dimensions = 2,
        string algorithm = "tsne",
        int perplexity = 5,
        int learningRate = 25,
        int iterations = 100)
    {
        return new()
        {
            Name =
                $"featureProjection(dimensions:{dimensions} algorithm:{algorithm} perplexity:{perplexity} learningRate:{learningRate} iterations:{iterations})",
            Fields = new Field[]
            {
                new()
                {
                    Name = "vector"
                }
            }
        };
    }
}
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

// ReSharper disable once CheckNamespace

using System.Text.Json.Serialization;

namespace SearchPioneer.Weaviate.Client;

/// <summary>
///     Parameters for vector index configuration.
/// </summary>
public class VectorIndexConfig
{
	/// <summary>
	///     The distance metric to be used to calculate the distance between any two arbitrary vectors. Defaults to
	///     <see>
	///         <cref>Distance.Cosine</cref>
	///     </see>
	///     .
	/// </summary>
	public Distance? Distance { get; set; }

	/// <summary>
	///     How often the async process runs that "repairs" the HNSW graph after deletes and updates.
	///     Prior to the repair/cleanup process, deleted objects are simply marked as deleted, but still a fully connected
	///     member of the HNSW graph. After the repair has run, the edges are reassigned and the datapoints deleted for good.
	///     Typically this value does not need to be adjusted, but if deletes or updates are very frequent it might make
	///     sense to adjust the value up or down.
	///     Higher value means it runs less frequently, but cleans up more in a single batch. Lower value means it runs
	///     more frequently, but might not be as efficient with each run.
	/// </summary>
	public int? CleanupIntervalSeconds { get; set; }

	/// <summary>
	///     If using dynamic  <see cref="Ef" /> (set to -1), this value controls how <see cref="Ef" /> is determined based on
	///     the given limit. E.g. with a factor of 8, <see cref="Ef" /> will be set to 8 * limit as long as this value is
	///     between the lower and upper boundary. It will be capped on either end, otherwise.
	///     Not available prior to v1.10.0.
	///     Defaults to 8.
	///     This setting has no effect if <see cref="Ef" /> has a value other than -1.
	/// </summary>
	public int? DynamicEfFactor { get; set; }

	/// <summary>
	///     If using dynamic <see cref="Ef" /> (set to -1), this value acts as an upper boundary. Even if the limit is
	///     large enough to suggest a lower value, <see cref="Ef" /> will be capped at this value. This helps to keep search
	///     speed reasonable when retrieving massive search result sets, e.g. 500+. Note that the maximum will not have
	///     any effect if the limit itself is higher than this maximum. In this case the limit will be chosen as
	///     <see cref="Ef" /> to avoid a situation where limit would higher than <see cref="Ef" /> which is impossible with
	///     HNSW.
	///     Not available prior to v1.10.0.
	///     Defaults to 500.
	///     This setting has no effect if <see cref="Ef" /> has a value other than -1.
	/// </summary>
	public int? DynamicEfMax { get; set; }

	/// <summary>
	///     If using dynamic <see cref="Ef" /> (set to -1), this value acts as a lower boundary. Even if the limit is
	///     small enough to suggest a lower value, <see cref="Ef" /> will never drop below this value. This helps in keeping
	///     search accuracy high even when setting very low limits, such as 1, 2, or 3. Not available prior to v1.10.0.
	///     Defaults to 100.
	///     This setting has no effect if <see cref="Ef" /> has a value other than -1.
	/// </summary>
	public int? DynamicEfMin { get; set; }

	/// <summary>
	///     Larger values are more accurate at the expense of slower search. This helps in the recall / performance
	///     trade-off that is possible with HNSW. If you omit setting this field it will default to -1 which means
	///     "Let Weaviate pick the right ef value".
	///     This value can be updated over time, and is not immutable like
	///     <see cref="EfConstruction" /> and <see cref="MaxConnections" />.
	/// </summary>
	public int? Ef { get; set; }

	/// <summary>
	///     Controls index search speed / build speed tradeoff. The tradeoff here is on importing. So a larger value means
	///     that you can lower your <see cref="Ef" /> settings but that importing will be slower. Default is set to 128,
	///     the integer should be greater than 0.
	///     This setting is immutable after class initialization.
	/// </summary>
	public int? EfConstruction { get; set; }

	/// <summary>
	/// Absolute number of objects configured as the threshold for a flat-search cutoff. If a filter on a filtered vector
	/// search matches fewer than the specified elements, the HNSW index is bypassed entirely and a flat (brute-force)
	/// search is performed instead. This can speed up queries with very restrictive filters considerably.
	/// Optional, defaults to 40000.
	/// Set to 0 to turn off flat-search cutoff entirely.
	/// </summary>
	public int? FlatSearchCutoff { get; set; }

	/// <summary>
	///     The maximum number of connections per element in all layers. Default is set to 64, the integer should be
	///     greater than 0.
	///     This setting is immutable after class initialization.
	/// </summary>
	public int? MaxConnections { get; set; }

	/// <summary>
	/// There are situations where it doesn't make sense to vectorize a class. For example if the class is just meant
	/// as glue between two other class (consisting only of references) or if the class contains mostly duplicate elements
	/// Note that importing duplicate vectors into HNSW is very expensive as the algorithm uses a check whether a
	/// candidate's distance is higher than the worst candidate's distance for an early exit condition.
	/// With (mostly) identical vectors, this early exit condition is never met leading to an exhaustive search on
	/// each import or query.
	/// In this case, you can skip indexing a vector all-together. To do so, set <see cref="Skip" /> to <c>true</c>.
	/// <see cref="Skip" /> defaults to <c>false</c>; if not set to <c>true</c>, classes will be
	/// indexed normally. This setting is immutable after class initialization. Note that the creation of a vector through
	/// a module is decoupled from storing the vector in Weaviate. So, simply skipping the indexing does not skip the
	/// generation of a vector if a vectorizer other than none is configured on the class (for example through a global
	/// default). It is therefore recommended to always set: <see cref="WeaviateClass.Vectorizer"/> to <value>none</value> explicitly
	/// when skipping the vector indexing. If vector indexing is skipped, but a vectorizer is configured
	/// (or a vector is provided manually) a warning is logged on each import.
	/// </summary>
	public bool? Skip { get; set; }

	/// <summary>
	/// For optimal search and import performance all previously imported vectors need to be held in memory. However,
	/// Weaviate also allows for limiting the number of vectors in memory. By default, when creating a new class, this
	/// limit is set to one trillion (i.e. 1e12) objects. A disk lookup for a vector is orders of magnitudes slower than
	/// memory lookup, so the cache should be used sparingly.
	/// This field is mutable after initially creating the class.
	/// Generally we recommend that:
	/// - During imports set the limit so that all vectors can be held in memory. Each import requires multiple searches
	///   so import performance will drop drastically as not all vectors can be held in the cache.
	///	- When only or mostly querying (as opposed to bulk importing) you can experiment with vector cache limits which
	///   are lower than your total dataset size. Vectors which aren't currently in cache will be added to the cache if
	///   there is still room. If the cache runs full it is dropped entirely and all future vectors need to be read from
	///   disk for the first time. Subsequent queries will be taken from the cache, until it runs full again and the
	///   procedure repeats. Note that the cache can be a very valuable tool if you have a large dataset, but a large
	///   percentage of users only query a specific subset of vectors. In this case you might be able to serve the largest
	///   user group from cache while requiring disk lookups for "irregular" queries.
	/// </summary>
	public long? VectorCacheMaxObjects { get; set; }

	/// <summary>
	/// This is an experimental feature released with 1.18.
	/// Used to enable product quantization which is a technique that allows for Weaviate’s HNSW vector index to store
	/// vectors using fewer bytes. As HNSW stores vectors in memory, this allows for running larger datasets on a
	/// given amount of memory. Weaviate’s HNSW implementation assumes that product quantization will occur after
	/// some data has already been loaded. The reason for this is that the codebook needs to be trained on existing
	/// data. A good recommendation is to have 10,000 to 100,000 vectors per shard loaded before enabling product
	/// quantization
	/// </summary>
	[JsonPropertyName("pq")]
	public ProductQuantizationConfig? ProductQuantization { get; set; }
}

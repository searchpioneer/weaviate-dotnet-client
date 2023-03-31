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
namespace SearchPioneer.Weaviate.Client;

public class ProductQuantizationConfig
{
	/// <summary>
	/// Product quantization is enabled or not (defaults to <c>false</c>). To enable set to <c>true</c>.
	/// </summary>
	public bool? Enabled { get; set; }

	/// <summary>
	/// Bit compression is enabled or not.
	/// </summary>
	public bool? BitCompression { get; set; }

	/// <summary>
	/// The number of segments to use. By default this is equal to the number of dimensions. Reducing the number
	/// of segments will further reduce the size of the quantized vectors. The number of segments must be divisible
	/// by the number of dimensions of each vector.
	/// </summary>
	public int? Segments { get; set; }

	/// <summary>
	/// The number of centroids to use. Reducing the number of centroids will further reduce the size of quantized
	/// vectors at the price of recall. When using the <value>kmeans</value> encoder, centroids is set to 256
	/// or one byte by default in Weaviate.
	/// </summary>
	public int? Centroids { get; set; }

	/// <summary>
	/// Encoder specific information
	/// </summary>
	public Encoder? Encoder { get; set; }
}

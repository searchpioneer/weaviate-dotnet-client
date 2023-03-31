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

/// <summary>
/// Encoder configuration
/// </summary>
public class Encoder
{
	/// <summary>
	/// Type of encoder. If using the <see cref="EncoderType.Tile"/> encoder you can also specify the <see cref="Distribution"/>.
	/// </summary>
	public EncoderType? Type { get; set; }

	/// <summary>
	///  The distribution, can be set to  If using the <see cref="EncoderType.Tile"/> encoder you can specify as
	/// <see cref="DistributionType.LogNormal"/> (default) or <see cref="DistributionType.Normal"/>.
	/// </summary>
	public DistributionType? Distribution { get; set; }
}

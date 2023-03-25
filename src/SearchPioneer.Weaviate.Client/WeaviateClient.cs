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

using Flurl.Http;

// ReSharper disable once CheckNamespace
namespace SearchPioneer.Weaviate.Client;

/// <summary>
///     A client for communicating to a Weaviate instance using HTTP.
///     For more information see https://github.com/searchpioneer/weaviate-dotnet-client
///     Developed by Search Pioneer; learn more about our services and expertise: https://searchpioneer.com/
/// </summary>
public class WeaviateClient
{
	private const string ClientVersion = "1.17.0";
	private static readonly Config DefaultConfig = new("http", "localhost:8080");

	private readonly DbVersionSupport _dbVersionSupport;
	private readonly Transport _transport;

	/// <summary>
	///     Create a new instance of a Weaviate client, using the default configuration and internally created FlurlClient
	/// </summary>
	/// <exception cref="ServerVersionMissingException">Thrown if server version is not available</exception>
	/// <exception cref="VersionMismatchException">Thrown on client and server version mismatch</exception>
	public WeaviateClient() : this(DefaultConfig, new FlurlClient())
	{
	}

	/// <summary>
	///     Create a new instance of a Weaviate client, using the default configuration
	/// </summary>
	/// <param name="flurlClient">Delegate HTTP calls to the provided Flurl client</param>
	/// <exception cref="ServerVersionMissingException">Thrown if server version is not available</exception>
	/// <exception cref="VersionMismatchException">Thrown on client and server version mismatch</exception>
	public WeaviateClient(IFlurlClient flurlClient) : this(DefaultConfig, flurlClient)
	{
	}

	/// <summary>
	///     Create a new instance of a Weaviate client.
	/// </summary>
	/// <param name="config">The configuration to use</param>
	/// <param name="flurlClient">Delegate HTTP calls to the provided Flurl client</param>
	/// <exception cref="ServerVersionMissingException">Thrown if server version is not available</exception>
	/// <exception cref="VersionMismatchException">Thrown on client and server version mismatch</exception>
	public WeaviateClient(Config config, IFlurlClient flurlClient)
	{
		_transport = new(config, flurlClient);

		// Get the version from the server and check compatible
		var serverVersion = Misc.Meta().Result?.Version;
		if (serverVersion == null)
			throw new ServerVersionMissingException("Unable to fetch version from server");

		// Check client and server version match
		if (serverVersion != ClientVersion)
			throw new VersionMismatchException($"Client version is {ClientVersion}, server version is {serverVersion}");

		_dbVersionSupport = new(serverVersion);
	}

	/// <summary>
	///     Backup operations
	/// </summary>
	public BackupApi Backup => new(_transport);

	/// <summary>
	///     Miscellaneous operations
	/// </summary>
	public MiscApi Misc => new(_transport);

	/// <summary>
	///     Schema operations
	/// </summary>
	public SchemaApi Schema => new(_transport);

	/// <summary>
	///     Data operations
	/// </summary>
	public DataApi Data => new(_transport, _dbVersionSupport);

	/// <summary>
	///     Reference operations
	/// </summary>
	public ReferenceApi Reference => new(_transport, _dbVersionSupport);

	/// <summary>
	///     Batch operations
	/// </summary>
	public BatchApi Batch => new(_transport, _dbVersionSupport);

	/// <summary>
	///     Contextionary operations
	/// </summary>
	public ContextionaryApi Contextionary => new(_transport);

	/// <summary>
	///     Classifications operations
	/// </summary>
	public ClassificationsApi Classification => new(_transport);

	/// <summary>
	///     Graph operations
	/// </summary>
	public GraphApi Graph => new(_transport);

	/// <summary>
	///     Cluster operations
	/// </summary>
	public ClusterApi Cluster => new(_transport);
}

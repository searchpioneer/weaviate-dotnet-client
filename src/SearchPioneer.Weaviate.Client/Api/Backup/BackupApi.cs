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
///     Backup operations
/// </summary>
public class BackupApi
{
	private const int WaitIntervalMilliseconds = 1000;
	private readonly Transport _transport;

	internal BackupApi(Transport transport) => _transport = transport;

	// ReSharper disable once MemberCanBePrivate.Global
	/// <summary>
	///     Get the status of a backup
	/// </summary>
	public ApiResponse<BackupStatusResponse> Status(BackupStatusRequest request)
	{
		var endpoint = $"/backups/{BackendJsonConverter.ToString(request.Backend)}/{request.Id}";
		return _transport.GetAsync<BackupStatusResponse>(endpoint).GetAwaiter().GetResult();
	}

	// ReSharper disable once MemberCanBePrivate.Global
	/// <summary>
	///     Get the status of a backup
	/// </summary>
	public Task<ApiResponse<BackupStatusResponse>> StatusAsync(BackupStatusRequest request,
		CancellationToken cancellationToken = default)
	{
		var endpoint = $"/backups/{BackendJsonConverter.ToString(request.Backend)}/{request.Id}";
		return _transport.GetAsync<BackupStatusResponse>(endpoint, cancellationToken);
	}

	// ReSharper disable once MemberCanBePrivate.Global
	/// <summary>
	///     Get the status of a backup restoration
	/// </summary>
	public ApiResponse<BackupStatusResponse> RestoreStatus(BackupStatusRequest request)
	{
		var endpoint = $"/backups/{BackendJsonConverter.ToString(request.Backend)}/{request.Id}/restore";
		return _transport.GetAsync<BackupStatusResponse>(endpoint).GetAwaiter().GetResult();
	}

	// ReSharper disable once MemberCanBePrivate.Global
	/// <summary>
	///     Get the status of a backup restoration
	/// </summary>
	public Task<ApiResponse<BackupStatusResponse>> RestoreStatusAsync(BackupStatusRequest request,
		CancellationToken cancellationToken = default)
	{
		var endpoint = $"/backups/{BackendJsonConverter.ToString(request.Backend)}/{request.Id}/restore";
		return _transport.GetAsync<BackupStatusResponse>(endpoint, cancellationToken);
	}

	// ReSharper disable once MemberCanBePrivate.Global
	/// <summary>
	///     Create a backup
	/// </summary>
	public ApiResponse<BackupResponse> Create(BackupRequest request)
	{
		var payload = new CreateBackup
		{
			Id = request.Id, Include = request.IncludeClasses, Exclude = request.ExcludeClasses
		};

		var endpoint = $"/backups/{BackendJsonConverter.ToString(request.Backend)}";
		var response = _transport.PostAsync<CreateBackup, BackupResponse>(endpoint, payload).GetAwaiter().GetResult();

		if (!request.WaitForCompletion || response.HttpStatusCode > 399)
			return response;

		while (true) // TODO! This is perhaps not the best solution, but mimics the Java client
		{
			var status = Status(new(request.Id, request.Backend));
			if (status.HttpStatusCode > 399) return Merge(status, response);

			if (status.Result?.Status is BackupStatus.Success or BackupStatus.Failed)
				return Merge(status, response);

			Thread.Sleep(WaitIntervalMilliseconds);
		}
	}

	// ReSharper disable once MemberCanBePrivate.Global
	/// <summary>
	///     Create a backup
	/// </summary>
	public async Task<ApiResponse<BackupResponse>> CreateAsync(BackupRequest request,
		CancellationToken cancellationToken = default)
	{
		var payload = new CreateBackup
		{
			Id = request.Id, Include = request.IncludeClasses, Exclude = request.ExcludeClasses
		};

		var endpoint = $"/backups/{BackendJsonConverter.ToString(request.Backend)}";
		var response = await _transport.PostAsync<CreateBackup, BackupResponse>(endpoint, payload, cancellationToken)
			.ConfigureAwait(false);

		if (!request.WaitForCompletion || response.HttpStatusCode > 399)
			return response;

		while (true) // TODO! This is perhaps not the best solution, but mimics the Java client
		{
			var status = await StatusAsync(new(request.Id, request.Backend), cancellationToken).ConfigureAwait(false);
			if (status.HttpStatusCode > 399) return Merge(status, response);

			if (status.Result?.Status is BackupStatus.Success or BackupStatus.Failed)
				return Merge(status, response);

			Thread.Sleep(WaitIntervalMilliseconds);
		}
	}

	// ReSharper disable once MemberCanBePrivate.Global
	/// <summary>
	///     Restore a backup
	/// </summary>
	public ApiResponse<BackupResponse> Restore(BackupRequest request)
	{
		var payload = new RestoreBackup { Include = request.IncludeClasses, Exclude = request.ExcludeClasses };

		var endpoint = $"/backups/{BackendJsonConverter.ToString(request.Backend)}/{request.Id}/restore";
		var response = _transport.PostAsync<RestoreBackup, BackupResponse>(endpoint, payload).GetAwaiter().GetResult();

		if (!request.WaitForCompletion || response.HttpStatusCode > 399)
			return response;

		while (true) // TODO! This is perhaps not the best solution, but mimics the Java client
		{
			var status = RestoreStatus(new(request.Id, request.Backend));
			if (status.HttpStatusCode > 399)
				return Merge(status, response);

			if (status.Result?.Status is BackupStatus.Success or BackupStatus.Failed)
				return Merge(status, response);

			Thread.Sleep(WaitIntervalMilliseconds);
		}
	}

	// ReSharper disable once MemberCanBePrivate.Global
	/// <summary>
	///     Restore a backup
	/// </summary>
	public async Task<ApiResponse<BackupResponse>> RestoreAsync(BackupRequest request,
		CancellationToken cancellationToken = default)
	{
		var payload = new RestoreBackup { Include = request.IncludeClasses, Exclude = request.ExcludeClasses };

		var endpoint = $"/backups/{BackendJsonConverter.ToString(request.Backend)}/{request.Id}/restore";
		var response = await _transport.PostAsync<RestoreBackup, BackupResponse>(endpoint, payload, cancellationToken)
			.ConfigureAwait(false);

		if (!request.WaitForCompletion || response.HttpStatusCode > 399)
			return response;

		while (true) // TODO! This is perhaps not the best solution, but mimics the Java client
		{
			var status = await RestoreStatusAsync(new(request.Id, request.Backend), cancellationToken)
				.ConfigureAwait(false);
			if (status.HttpStatusCode > 399)
				return Merge(status, response);

			if (status.Result?.Status is BackupStatus.Success or BackupStatus.Failed)
				return Merge(status, response);

			Thread.Sleep(WaitIntervalMilliseconds);
		}
	}

	private static ApiResponse<BackupResponse> Merge(
		ApiResponse<BackupStatusResponse> status,
		ApiResponse<BackupResponse> response)
	{
		var apiResponse = new ApiResponse<BackupResponse>
		{
			Error = status.Error,
			Uri = response.Uri,
			HttpMethod = response.HttpMethod,
			HttpStatusCode = response.HttpStatusCode,
			RequestBody = response.RequestBody,
			ResponseBody = status.ResponseBody,
			Result = new()
			{
				Id = status.Result?.Id,
				Backend = status.Result?.Backend,
				Path = status.Result?.Path,
				Status = status.Result?.Status,
				Error = status.Result?.Error,
				Classes = response.Result?.Classes
			}
		};
		apiResponse.Warnings.AddRange(response.Warnings);
		return apiResponse;
	}

	private class CreateBackup
	{
		// ReSharper disable once UnusedAutoPropertyAccessor.Local
		public string? Id { get; set; }

		// ReSharper disable once UnusedAutoPropertyAccessor.Local
		public string[]? Include { get; set; }

		// ReSharper disable once UnusedAutoPropertyAccessor.Local
		public string[]? Exclude { get; set; }
	}

	private class RestoreBackup
	{
		// ReSharper disable once UnusedAutoPropertyAccessor.Local
		public string[]? Include { get; set; }

		// ReSharper disable once UnusedAutoPropertyAccessor.Local
		public string[]? Exclude { get; set; }
	}
}

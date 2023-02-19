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

using SearchPioneer.Weaviate.Client.Api.Backup.Model;
using SearchPioneer.Weaviate.Client.Api.Backup.Request;
using SearchPioneer.Weaviate.Client.Api.Backup.Response;
using SearchPioneer.Weaviate.Client.JsonConvertors;

namespace SearchPioneer.Weaviate.Client.Api.Backup;

public class BackupApi
{
    private const int WaitIntervalMilliseconds = 1000;
    private readonly Transport _transport;

    internal BackupApi(Transport transport)
    {
        _transport = transport;
    }

    public ApiResponse<BackupStatusResponse> GetStatus(GetStatusRequest request)
    {
        var endpoint = $"/backups/{BackendJsonConverter.ToString(request.Backend)}/{request.Id}";
        var response = _transport.Get<BackupStatusResponse>(endpoint);
        return response;
    }

    public ApiResponse<BackupStatusResponse> GetRestoreStatus(GetStatusRequest request)
    {
        var endpoint = $"/backups/{BackendJsonConverter.ToString(request.Backend)}/{request.Id}/restore";
        var response = _transport.Get<BackupStatusResponse>(endpoint);
        return response;
    }

    public ApiResponse<BackupResponse> Create(BackupRequest request)
    {
        var payload = new CreateBackup
        {
            Id = request.Id,
            Include = request.IncludeClasses,
            Exclude = request.ExcludeClasses
        };

        var endpoint = $"/backups/{BackendJsonConverter.ToString(request.Backend)}";
        var response = _transport.Post<CreateBackup, BackupResponse>(endpoint, payload);

        if (!request.WaitForCompletion || response.HttpStatusCode > 399)
            return response;

        while (true) // TODO! danger
        {
            var status = GetStatus(new(request.Backend, request.Id));
            if (status.HttpStatusCode > 399) return Merge(status, response);

            if (status.Result.Status is BackupStatus.Success or BackupStatus.Failed)
                return Merge(status, response);

            Thread.Sleep(WaitIntervalMilliseconds);
        }
    }

    public ApiResponse<BackupResponse> Restore(BackupRequest request)
    {
        var payload = new RestoreBackup
        {
            Include = request.IncludeClasses,
            Exclude = request.ExcludeClasses
        };

        var endpoint = $"/backups/{BackendJsonConverter.ToString(request.Backend)}/{request.Id}/restore";
        var response = _transport.Post<RestoreBackup, BackupResponse>(endpoint, payload);

        if (!request.WaitForCompletion || response.HttpStatusCode > 399)
            return response;

        while (true) // TODO! danger
        {
            var status = GetRestoreStatus(new(request.Backend, request.Id));
            if (status.HttpStatusCode > 399)
                return Merge(status, response);

            if (status.Result.Status is BackupStatus.Success or BackupStatus.Failed)
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
                Id = status.Result.Id,
                Backend = status.Result.Backend,
                Path = status.Result.Path,
                Status = status.Result.Status,
                Error = status.Result.Error,
                Classes = response.Result.Classes
            }
        };
        apiResponse.Warnings.AddRange(response.Warnings);
        return apiResponse;
    }

    private class CreateBackup
    {
        public string? Id { get; set; }
        public string[]? Include { get; set; }
        public string[]? Exclude { get; set; }
    }

    private class RestoreBackup
    {
        public string[]? Include { get; set; }
        public string[]? Exclude { get; set; }
    }
}
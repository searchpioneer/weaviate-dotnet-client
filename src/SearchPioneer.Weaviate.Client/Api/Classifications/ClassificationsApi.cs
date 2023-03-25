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
/// Classifications API operations
/// </summary>
public class ClassificationsApi
{
    private const int WaitIntervalMilliseconds = 1000;
    private readonly Transport _transport;

    internal ClassificationsApi(Transport transport) => _transport = transport;

    public ApiResponse<Classification> Schedule(ScheduleClassificationRequest request)
    {
	    var config = GetClassification(request);

        var response = _transport.PostAsync<Classification, Classification>("/classifications", config).GetAwaiter().GetResult();

        if (!request.WaitForCompletion || response.HttpStatusCode > 399) return response;

        while (true) // TODO! danger
        {
            response = Get(new(response.Result?.Id!));
            var runningClassification = response.Result;
            if (runningClassification?.Status != "running") break;

            Thread.Sleep(WaitIntervalMilliseconds);
        }

        return response;
    }

    public async Task<ApiResponse<Classification>> ScheduleAsync(ScheduleClassificationRequest request, CancellationToken cancellationToken = default)
    {
        var config = GetClassification(request);

        var response = await _transport.PostAsync<Classification, Classification>("/classifications", config, cancellationToken).ConfigureAwait(false);

        if (!request.WaitForCompletion || response.HttpStatusCode > 399) return response;

        while (true) // TODO! danger
        {
            response = await GetAsync(new(response.Result?.Id!), cancellationToken).ConfigureAwait(false);
            var runningClassification = response.Result;
            if (runningClassification?.Status != "running") break;

            Thread.Sleep(WaitIntervalMilliseconds);
        }

        return response;
    }

    public ApiResponse<Classification> Get(GetClassificationRequest request)
    {
        var path = $"/classifications/{request.Id}";
        var response = _transport.GetAsync<Classification>(path).GetAwaiter().GetResult();
        return response;
    }

    public async Task<ApiResponse<Classification>> GetAsync(GetClassificationRequest request, CancellationToken cancellationToken = default)
    {
        var path = $"/classifications/{request.Id}";
        var response = await _transport.GetAsync<Classification>(path, cancellationToken).ConfigureAwait(false);
        return response;
    }

    private static ClassificationFilters? GetClassificationFilters(Where? sourceWhere, Where? targetWhere,
        Where? trainingSetWhere)
    {
        if (sourceWhere == null && targetWhere == null && trainingSetWhere == null)
            return null;

        return new()
        {
            SourceWhere = sourceWhere,
            TargetWhere = targetWhere,
            TrainingSetWhere = trainingSetWhere
        };
    }

    private static Classification GetClassification(ScheduleClassificationRequest request) =>
	    new()
	    {
		    Class = request.Class,
		    BasedOnProperties = request.BasedOnProperties,
		    ClassifyProperties = request.ClassifyProperties,
		    Type = request.ClassificationType,
		    Settings = request.Settings,
		    Filters = GetClassificationFilters(request.SourceWhere, request.TargetWhere, request.TrainingSetWhere)
	    };
}

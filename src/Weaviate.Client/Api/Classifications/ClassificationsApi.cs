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

using SearchPioneer.Weaviate.Client.Api.Classifications.Model;
using SearchPioneer.Weaviate.Client.Api.Classifications.Request;
using SearchPioneer.Weaviate.Client.Api.Filters;

namespace SearchPioneer.Weaviate.Client.Api.Classifications;

public class ClassificationsApi
{
    private const int WaitIntervalMilliseconds = 1000;
    private readonly Transport _transport;

    internal ClassificationsApi(Transport transport)
    {
        _transport = transport;
    }

    public ApiResponse<Classification> Schedule(ScheduleClassificationRequest request)
    {
        var config = new Classification
        {
            Class = request.Class,
            BasedOnProperties = request.BasedOnProperties,
            ClassifyProperties = request.ClassifyProperties,
            Type = request.ClassificationType,
            Settings = request.Settings,
            Filters = GetClassificationFilters(request.SourceWhere, request.TargetWhere, request.TrainingSetWhere)
        };

        var response = _transport.Post<Classification, Classification>("/classifications", config);

        if (!request.WaitForCompletion || response.HttpStatusCode > 399) return response;

        while (true) // TODO! danger
        {
            response = Get(new(response.Result.Id!));
            var runningClassification = response.Result;
            if (runningClassification.Status != "running") break;

            Thread.Sleep(WaitIntervalMilliseconds);
        }

        return response;
    }

    public ApiResponse<Classification> Get(GetClassificationRequest request)
    {
        var path = $"/classifications/{request.Id}";
        var response = _transport.Get<Classification>(path);
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
}
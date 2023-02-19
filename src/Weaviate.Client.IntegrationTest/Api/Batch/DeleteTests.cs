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

using SearchPioneer.Weaviate.Client.Api.Batch.Model;
using SearchPioneer.Weaviate.Client.Api.Batch.Response;
using SearchPioneer.Weaviate.Client.Api.Filters;
using Xunit;

namespace SearchPioneer.Weaviate.Client.IntegrationTest.Api.Batch;

[Collection("Sequential")]
public class DeleteTests : TestBase
{
    [Fact]
    public void DeleteDryRunVerbose()
    {
        CreateTestSchemaAndData(Client);

        var allWeaviateObjects = Client.Data.GetAll().Result.Length;

        var dryRun = Client.Batch.DeleteObjects(new()
        {
            Where = new()
            {
                Operator = Operator.Equal,
                Path = new[] { "name" },
                ValueString = "Hawaii"
            },
            DryRun = true,
            Class = CLASS_NAME_PIZZA,
            Output = BatchOutput.Verbose
        });

        Assert.True(dryRun.HttpStatusCode == 200);

        var remainingWeaviateObjects = Client.Data.GetAll().Result.Length;

        Assert.Equal(remainingWeaviateObjects, allWeaviateObjects);

        Assert.Equal(BatchOutput.Verbose, dryRun.Result.Output);
        Assert.Equal(0, dryRun.Result.Results.Successful);
        Assert.Equal(0, dryRun.Result.Results.Failed);
        Assert.Equal(10000L, dryRun.Result.Results.Limit);
        Assert.Equal(1, dryRun.Result.Results.Matches);
        Assert.Single(dryRun.Result.Results.Objects);
        Assert.Equal(PIZZA_HAWAII_ID, dryRun.Result.Results.Objects.First().Id);
        Assert.Equal(BatchResultStatus.DryRun, dryRun.Result.Results.Objects.First().Status);
        Assert.Null(dryRun.Result.Results.Objects.First().Errors);
    }

    [Fact]
    public void DeleteDryRunMinmal()
    {
        CreateTestSchemaAndData(Client);

        var allWeaviateObjects = Client.Data.GetAll().Result.Length;

        var dryRun = Client.Batch.DeleteObjects(new()
        {
            Where = new()
            {
                Operator = Operator.Equal,
                Path = new[] { "name" },
                ValueString = "Hawaii"
            },
            DryRun = true,
            Class = CLASS_NAME_PIZZA,
            Output = BatchOutput.Minimal
        });

        Assert.True(dryRun.HttpStatusCode == 200);

        var remainingWeaviateObjects = Client.Data.GetAll().Result.Length;

        Assert.Equal(remainingWeaviateObjects, allWeaviateObjects);

        Assert.Equal(BatchOutput.Minimal, dryRun.Result.Output);
        Assert.Equal(0, dryRun.Result.Results.Successful);
        Assert.Equal(0, dryRun.Result.Results.Failed);
        Assert.Equal(10000L, dryRun.Result.Results.Limit);
        Assert.Equal(1, dryRun.Result.Results.Matches);
        Assert.Null(dryRun.Result.Results.Objects);
    }

    [Fact]
    public void DeleteNoMatchWithDefaultOutputAndDryRun()
    {
        CreateTestSchemaAndData(Client);

        var allWeaviateObjects = Client.Data.GetAll().Result.Length;

        var batch = Client.Batch.DeleteObjects(new()
        {
            Where = new()
            {
                Operator = Operator.GreaterThan,
                Path = new[] { "_creationTimeUnix" },
                ValueString = DateTimeOffset.Now.AddSeconds(60).ToUnixTimeMilliseconds().ToString()
            },
            Class = CLASS_NAME_PIZZA
        });

        Assert.True(batch.HttpStatusCode == 200);

        var remainingWeaviateObjects = Client.Data.GetAll().Result.Length;

        Assert.Equal(remainingWeaviateObjects, allWeaviateObjects);

        Assert.Equal(BatchOutput.Minimal, batch.Result.Output);
        Assert.Equal(0, batch.Result.Results.Successful);
        Assert.Equal(0, batch.Result.Results.Failed);
        Assert.Equal(10000L, batch.Result.Results.Limit);
        Assert.Equal(0, batch.Result.Results.Matches);
        Assert.Null(batch.Result.Results.Objects);
    }

    [Fact]
    public void DeleteNoMatchWithVerboseOutput()
    {
        CreateTestSchemaAndData(Client);

        var allWeaviateObjects = Client.Data.GetAll().Result.Length;

        var batch = Client.Batch.DeleteObjects(new()
        {
            Where = new()
            {
                Operator = Operator.LessThan,
                Path = new[] { "_creationTimeUnix" },
                ValueString = DateTimeOffset.Now.AddSeconds(60).ToUnixTimeMilliseconds().ToString()
            },
            Class = CLASS_NAME_PIZZA,
            Output = BatchOutput.Verbose
        });

        Assert.True(batch.HttpStatusCode == 200);

        var remainingWeaviateObjects = Client.Data.GetAll().Result.Length;

        Assert.Equal(allWeaviateObjects - 4, remainingWeaviateObjects);

        Assert.Equal(BatchOutput.Verbose, batch.Result.Output);
        Assert.Equal(4, batch.Result.Results.Successful);
        Assert.Equal(0, batch.Result.Results.Failed);
        Assert.Equal(10000L, batch.Result.Results.Limit);
        Assert.Equal(4, batch.Result.Results.Matches);
        Assert.Equal(4, batch.Result.Results.Objects.Length);

        var pizzas = batch.Result.Results.Objects.Select(o => o.Id).ToArray();
        Assert.Contains(PIZZA_HAWAII_ID, pizzas);
        Assert.Contains(PIZZA_DOENER_ID, pizzas);
        Assert.Contains(PIZZA_QUATTRO_FORMAGGI_ID, pizzas);
        Assert.Contains(PIZZA_FRUTTI_DI_MARE_ID, pizzas);
    }
}
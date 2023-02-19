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

using SearchPioneer.Weaviate.Client.Api.Graph.Query.Arguments;
using Xunit;

namespace SearchPioneer.Weaviate.Client.UnitTest.V1.Graph.Query.Arguments;

public class AskTests : TestBase
{
    [Fact]
    public void WithQuestion()
    {
        var arg = new Ask
        {
            Question = "What's your name?"
        };
        Assert.Equal("ask:{question:\"What's your name?\"}", arg.ToString());
    }

    [Fact]
    public void WithProperties()
    {
        var arg = new Ask
        {
            Question = "What's your name?",
            Properties = new[] { "prop1", "prop2" }
        };
        Assert.Equal("ask:{question:\"What's your name?\" properties: [\"prop1\",\"prop2\"]}", arg.ToString());
    }

    [Fact]
    public void WithPropertiesAndCertainty()
    {
        var arg = new Ask
        {
            Question = "What's your name?",
            Properties = new[] { "prop1", "prop2" },
            Certainty = 0.8f
        };
        Assert.Equal("ask:{question:\"What's your name?\" properties: [\"prop1\",\"prop2\"] certainty:0.8}",
            arg.ToString());
    }

    [Fact]
    public void WithPropertiesAndDistance()
    {
        var arg = new Ask
        {
            Question = "What's your name?",
            Properties = new[] { "prop1", "prop2" },
            Distance = 0.8f
        };
        Assert.Equal("ask:{question:\"What's your name?\" properties: [\"prop1\",\"prop2\"] distance:0.8}",
            arg.ToString());
    }

    [Fact]
    public void WithAutocorrect()
    {
        var arg = new Ask
        {
            Question = "What's your name?",
            Autocorrect = true
        };
        Assert.Equal("ask:{question:\"What's your name?\" autocorrect:true}", arg.ToString());
    }

    [Fact]
    public void WithPropertiesAndCertaintyAndAutocorrect()
    {
        var arg = new Ask
        {
            Question = "What's your name?",
            Properties = new[] { "prop1", "prop2" },
            Certainty = 0.8f,
            Autocorrect = false
        };
        Assert.Equal(
            "ask:{question:\"What's your name?\" properties: [\"prop1\",\"prop2\"] certainty:0.8 autocorrect:false}",
            arg.ToString());
    }

    [Fact]
    public void WithPropertiesAndDistanceAndAutocorrect()
    {
        var arg = new Ask
        {
            Question = "What's your name?",
            Properties = new[] { "prop1", "prop2" },
            Distance = 0.8f,
            Autocorrect = false
        };
        Assert.Equal(
            "ask:{question:\"What's your name?\" properties: [\"prop1\",\"prop2\"] distance:0.8 autocorrect:false}",
            arg.ToString());
    }

    [Fact]
    public void WithPropertiesAndCertaintyAndAutocorrectAndRerank()
    {
        var arg = new Ask
        {
            Question = "What's your name?",
            Properties = new[] { "prop1", "prop2" },
            Certainty = 0.8f,
            Autocorrect = false,
            Rerank = true
        };
        Assert.Equal(
            "ask:{question:\"What's your name?\" properties: [\"prop1\",\"prop2\"] certainty:0.8 autocorrect:false rerank:true}",
            arg.ToString());
    }

    [Fact]
    public void WithPropertiesAndDistanceAndAutocorrectAndRerank()
    {
        var arg = new Ask
        {
            Question = "What's your name?",
            Properties = new[] { "prop1", "prop2" },
            Distance = 0.8f,
            Autocorrect = false,
            Rerank = true
        };
        Assert.Equal(
            "ask:{question:\"What's your name?\" properties: [\"prop1\",\"prop2\"] distance:0.8 autocorrect:false rerank:true}",
            arg.ToString());
    }

    [Fact]
    public void WithRerank()
    {
        var arg = new Ask
        {
            Question = "What's your name?",
            Rerank = false
        };
        Assert.Equal("ask:{question:\"What's your name?\" rerank:false}", arg.ToString());
    }

    [Fact]
    public void WithoutAll()
    {
        var arg = new Ask();
        // builder will return a faulty ask arg in order for Weaviate to error
        // so that user will know that something was wrong
        Assert.Equal("ask:{}", arg.ToString());
    }
}
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

public class Config
{
    private readonly Dictionary<string, string> _headers;
    private readonly string _host;
    private readonly string _scheme;
    private readonly string _version;

    public Config(string scheme, string host, string apiKey)
    {
        _scheme = scheme;
        _host = host;
        _version = "v1";
        _headers = new()
        {
	        { "authorization", $"Bearer {apiKey}" }
        };
    }

    public Config(string scheme, string host, Dictionary<string, string> headers)
    {
        _scheme = scheme;
        _host = host;
        _version = "v1";
        _headers = headers;
    }

    public Config(string scheme, string host) :
        this(scheme, host, new Dictionary<string, string>())
    {
    }

    public string GetBaseUrl() => $"{_scheme}://{_host}/{_version}";

    public Dictionary<string, string> GetHeaders() => _headers;

    /// <summary>
    /// HTTP timeout, defaults to 60 seconds.
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(60);
}

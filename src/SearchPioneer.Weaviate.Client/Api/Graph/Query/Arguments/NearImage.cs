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

public class NearImage
{
    public string? Image { get; set; }
    public FileInfo? ImageFile { get; set; }
    public float? Certainty { get; set; }
    public float? Distance { get; set; }

    public override string ToString()
    {
        var fields = new HashSet<string>();
        var content = GetContent();
        if (!string.IsNullOrEmpty(content)) fields.Add($"image:\"{content}\"");
        if (Certainty != null) fields.Add($"certainty:{Certainty}");
        if (Distance != null) fields.Add($"distance:{Distance}");
        var s = string.Join(" ", fields.ToArray());
        return $"nearImage:{{{s}}}";
    }

    private static string? ReadFile(FileSystemInfo file)
    {
        try
        {
            var content = File.ReadAllBytes(file.FullName);
            return Convert.ToBase64String(content);
        }
        catch
        {
            // TODO: swallow exceptions is bad
            return null;
        }
    }

    private string? GetContent()
    {
        if (!string.IsNullOrEmpty(Image))
        {
            if (Image != null && Image.StartsWith("data:"))
            {
                const string base64 = ";base64,";
                return Image.Substring(Image.IndexOf(base64, StringComparison.Ordinal) + base64.Length);
            }

            return Image;
        }

        if (ImageFile != null) return ReadFile(ImageFile);

        return null;
    }
}

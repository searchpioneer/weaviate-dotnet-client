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

using System.Reflection;
using Xunit;

namespace SearchPioneer.Weaviate.Client.Tests.V1.Graph.Query.Arguments;

public class NearImageTests : TestBase
{
	private const string base64File =
		"iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAIAAACQd1PeAAABhGlDQ1BJQ0MgcHJvZmlsZQAAKJF9kT1Iw0AcxV/TSou0ONhBxCFD62RBVMRRq1CECqFWaNXB5NIvaNKSpLg4Cq4FBz8Wqw4uzro6uAqC4AeIm5uToouU+L+k0CLGg+N+vLv3uHsHCK0q08zAOKDplpFJJcVcflUMviKACEKIwy8zsz4nSWl4jq97+Ph6l+BZ3uf+HBG1YDLAJxLPsrphEW8QT29adc77xFFWllXic+Ixgy5I/Mh1xeU3ziWHBZ4ZNbKZeeIosVjqYaWHWdnQiKeIY6qmU76Qc1nlvMVZqzZY5578heGCvrLMdZojSGERS5AgQkEDFVRhIUGrToqJDO0nPfzDjl8il0KuChg5FlCDBtnxg//B727N4uSEmxROAn0vtv0RB4K7QLtp29/Htt0+AfzPwJXe9ddawMwn6c2uFjsCBraBi+uupuwBlzvA0FNdNmRH8tMUikXg/Yy+KQ8M3gL9a25vnX2cPgBZ6ip9AxwcAqMlyl73eHeot7d/z3T6+wEPO3J/B8olWgAAAAlwSFlzAAAuIwAALiMBeKU/dgAAAAd0SU1FB+UEDQgmFS2naPsAAAAZdEVYdENvbW1lbnQAQ3JlYXRlZCB3aXRoIEdJTVBXgQ4XAAAADElEQVQI12NgYGAAAAAEAAEnNCcKAAAAAElFTkSuQmCC";

	private readonly FileInfo _imageFile = new(Assembly.GetExecutingAssembly().Location + "../../../../../pixel.png");

	[Fact]
	public void WithImageFile()
	{
		var nearImage = new NearImage { ImageFile = _imageFile };
		Assert.Equal($"nearImage:{{image:\"{base64File}\"}}", nearImage.ToString());
	}

	[Fact]
	public void WithCertainty()
	{
		var nearImage = new NearImage { ImageFile = _imageFile, Certainty = 0.5f };
		Assert.Equal($"nearImage:{{image:\"{base64File}\" certainty:0.5}}", nearImage.ToString());
	}

	[Fact]
	public void WithDistance()
	{
		var nearImage = new NearImage { ImageFile = _imageFile, Distance = 0.5f };
		Assert.Equal($"nearImage:{{image:\"{base64File}\" distance:0.5}}", nearImage.ToString());
	}

	[Fact]
	public void WithImage()
	{
		var nearImage = new NearImage { Image = "iVBORw0KGgoAAAANS" };
		Assert.Equal("nearImage:{image:\"iVBORw0KGgoAAAANS\"}", nearImage.ToString());
	}

	[Fact]
	public void WithBase64DataImage()
	{
		var nearImage = new NearImage { Image = "data:image/png;base64,iVBORw0KGgoAAAANS" };
		Assert.Equal("nearImage:{image:\"iVBORw0KGgoAAAANS\"}", nearImage.ToString());
	}

	[Fact]
	public void WithImageAndCertainty()
	{
		var nearImage = new NearImage { Image = "iVBORw0KGgoAAAANS", Certainty = 0.5f };
		Assert.Equal("nearImage:{image:\"iVBORw0KGgoAAAANS\" certainty:0.5}", nearImage.ToString());
	}

	[Fact]
	public void WithImageAndDistance()
	{
		var nearImage = new NearImage { Image = "iVBORw0KGgoAAAANS", Distance = 0.5f };
		Assert.Equal("nearImage:{image:\"iVBORw0KGgoAAAANS\" distance:0.5}", nearImage.ToString());
	}

	[Fact]
	public void WithBadFile()
	{
		var nearImage = new NearImage { ImageFile = new("/not-here.jpg") };

		// builder will return a faulty nearImage arg in order for Weaviate to error
		// so that user will know that something was wrong
		Assert.Equal("nearImage:{}", nearImage.ToString());
	}

	[Fact]
	public void WithoutAll()
	{
		var nearImage = new NearImage();

		// builder will return a faulty nearImage arg in order for Weaviate to error
		// so that user will know that something was wrong
		Assert.Equal("nearImage:{}", nearImage.ToString());
	}
}

using System;
using System.Drawing;
using Dataedo.App.Tools.Export.Universal.Storage;

namespace Dataedo.App.Tools.Export.Universal;

public interface ITemplate : IReadStorage
{
	string Name { get; }

	string Description { get; }

	string TransformerName { get; }

	string CustomizerName { get; }

	string Version { get; }

	Image Thumbnail { get; }

	new bool IsOpenable { get; }

	bool IsValid { get; set; }

	bool IsInUserTemplateDirectory { get; }

	Exception ExceptionWhenTemplateIsNotValid { get; set; }

	new void Open();
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Xml.Serialization;
using Dataedo.App.Enums;
using Dataedo.App.Tools.Export.Universal.Storage;
using Dataedo.App.Tools.Export.Universal.Storage.Providers;

namespace Dataedo.App.Tools.Export.Universal.Templates;

internal class LocalTemplate : ITemplate, IReadStorage
{
	public const string MetadataFile = "meta.xml";

	public const string ThumbnailFile = "thumbnail.png";

	public const string UserThumbnailFile = "thumbnail.user.png";

	public readonly LocalStorage Storage;

	private LocalTemplateMetadata metadata;

	public string Name => GetMetadata()?.Name;

	public string Version => GetMetadata()?.Version;

	public string Description => GetMetadata()?.Description;

	public string TransformerName => GetMetadata()?.Transformer;

	public string CustomizerName => GetMetadata()?.Customizer;

	public bool IsValid { get; set; }

	public Exception ExceptionWhenTemplateIsNotValid { get; set; }

	public Image Thumbnail => Image.FromFile(Path.Combine(Storage.Dir, "thumbnail.png"));

	public bool IsOpenable => Storage.IsOpenable;

	public bool IsInUserTemplateDirectory => Storage.Dir.StartsWith(Paths.GetUserTemplatesPath(DocFormatEnum.DocFormat.HTML));

	public LocalTemplate(string dir)
	{
		Storage = new LocalStorage(dir);
	}

	public LocalTemplate(LocalStorage storage)
	{
		Storage = storage;
	}

	public void Open()
	{
		Storage.Open();
	}

	public IEnumerable<string> ListFiles(string searchDir = "")
	{
		return Storage.ListFiles(searchDir);
	}

	public byte[] ReadFile(string path)
	{
		return Storage.ReadFile(path);
	}

	public string ReadTextFile(string path)
	{
		return Storage.ReadTextFile(path);
	}

	public bool HasFile(string path)
	{
		return Storage.HasFile(path);
	}

	private LocalTemplateMetadata GetMetadata()
	{
		if (metadata == null)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(LocalTemplateMetadata));
			using FileStream stream = new FileStream(Path.Combine(Storage.Dir, "meta.xml"), FileMode.Open, FileAccess.Read);
			try
			{
				metadata = (LocalTemplateMetadata)xmlSerializer.Deserialize(stream);
				IsValid = true;
			}
			catch (Exception exceptionWhenTemplateIsNotValid)
			{
				IsValid = false;
				ExceptionWhenTemplateIsNotValid = exceptionWhenTemplateIsNotValid;
			}
		}
		return metadata;
	}
}

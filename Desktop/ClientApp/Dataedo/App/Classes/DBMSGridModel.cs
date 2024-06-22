using System.Collections.Generic;
using System.Drawing;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Classes;

public class DBMSGridModel
{
	public string Type { get; private set; }

	public string Name { get; private set; }

	public Image Image { get; private set; }

	public string URL { get; private set; }

	public bool IsActive { get; private set; }

	public List<SharedImportFolderEnum.ImportFolder> ImportFolders { get; private set; }

	public bool IsDatabase { get; private set; }

	public bool IsConnectorInLicense { get; private set; }

	public bool IsCloudStorage { get; private set; }

	public DBMSGridModel(string type, string name, Image image, string URL, List<SharedImportFolderEnum.ImportFolder> importFolders, bool isDatabase, bool isActive = true, bool isConnectorInLicense = true, bool isCloudStorage = false)
	{
		Type = type;
		Name = name;
		Image = image;
		this.URL = URL;
		IsActive = isActive;
		ImportFolders = importFolders;
		IsDatabase = isDatabase;
		IsConnectorInLicense = isConnectorInLicense;
		IsCloudStorage = isCloudStorage;
	}
}

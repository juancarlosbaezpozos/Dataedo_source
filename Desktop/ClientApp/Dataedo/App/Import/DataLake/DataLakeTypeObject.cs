using System.Collections.Generic;
using System.Drawing;
using Dataedo.App.Licences;
using Dataedo.App.Tools;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Import.DataLake;

public class DataLakeTypeObject
{
	public DataLakeTypeEnum.DataLakeType Value { get; private set; }

	public string DisplayName => DataLakeTypeEnum.GetDisplayName(Value);

	public Image Image => DataLakeTypeEnum.GetImage(Value);

	public string URL
	{
		get
		{
			if (!Connectors.HasDataLakeTypeConnector(Value))
			{
				return "<href=" + Links.ManageAccounts + ">(upgrade to connect)</href>";
			}
			return "";
		}
	}

	public List<SharedImportFolderEnum.ImportFolder> ImportFolders => DataLakeTypeEnum.GetImportFolders(Value);

	public DataLakeTypeObject(DataLakeTypeEnum.DataLakeType dataLakeType)
	{
		Value = dataLakeType;
	}
}

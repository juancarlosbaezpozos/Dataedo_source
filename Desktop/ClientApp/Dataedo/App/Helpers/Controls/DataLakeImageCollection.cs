using System.Collections.Generic;
using System.Drawing;
using Dataedo.App.Import.DataLake;
using Dataedo.App.Properties;
using DevExpress.Utils;

namespace Dataedo.App.Helpers.Controls;

public class DataLakeImageCollection : ImageCollection
{
	private Dictionary<DataLakeTypeEnum.DataLakeType, int> _imageIndexes;

	public int FolderImageIndex { get; private set; }

	public int UnknownFileImageIndex { get; private set; }

	public void Init()
	{
		Clear();
		_imageIndexes = new Dictionary<DataLakeTypeEnum.DataLakeType, int>();
		FolderImageIndex = base.Images.Add(Resources.folder_16, "folder");
		UnknownFileImageIndex = base.Images.Add(Resources.flat_file_16, "unknown_file");
		DataLakeTypeEnum.DataLakeType[] dataLakeTypes = DataLakeTypeEnum.GetDataLakeTypes();
		foreach (DataLakeTypeEnum.DataLakeType dataLakeType in dataLakeTypes)
		{
			Image image = DataLakeTypeEnum.GetImage(dataLakeType);
			string name = DataLakeTypeEnum.TypeToString(dataLakeType);
			_imageIndexes[dataLakeType] = base.Images.Add(image, name);
		}
	}

	public int GetIndex(DataLakeTypeEnum.DataLakeType value)
	{
		return _imageIndexes[value];
	}
}

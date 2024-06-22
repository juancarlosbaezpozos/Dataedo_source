using System;
using System.Collections.Generic;
using System.IO;
using Dataedo.App.Import.DataLake.Model;
using Dataedo.App.Import.Exceptions;
using Dataedo.Shared.Enums;
using Parquet;
using Parquet.Data;

namespace Dataedo.App.Import.DataLake.Processing;

internal class DeltaLakeImport : ParquetImport, IDataLakeImport
{
	public override DataLakeTypeEnum.DataLakeType DataLakeType => DataLakeTypeEnum.DataLakeType.DELTALAKE;

	public override string DefaultExtension => ".Delta";

	public override IEnumerable<string> Extensions => new string[1] { DefaultExtension };

	public new string DocumentationLink => "https://dataedo.com/docs/delta-lake?utm_source=App&utm_medium=App";

	public DeltaLakeImport(SharedObjectTypeEnum.ObjectType objectType)
		: base(objectType)
	{
	}

	public IEnumerable<ObjectModel> GetObjectsFromFile(string path, string customName = null)
	{
		using FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
		return GetObjectsFromStream(fileStream, path, customName);
	}

	public override IEnumerable<ObjectModel> GetObjectsFromStream(Stream fileStream, string path)
	{
		return GetObjectsFromStream(fileStream, path);
	}

	public IEnumerable<ObjectModel> GetObjectsFromStream(Stream fileStream, string path, string customName = null)
	{
		try
		{
			string directoryName = Path.GetDirectoryName(path);
			ObjectModel objectModel = new ObjectModel(customName ?? Path.GetFileName(directoryName), directoryName, DataLakeType, base.ObjectType, base.ObjectSubtype);
			using (ParquetReader parquetReader = new ParquetReader(fileStream))
			{
				int num = 1;
				foreach (Field field in parquetReader.Schema.Fields)
				{
					AddFieldsToModel(objectModel, field, null, num, 1);
					num++;
				}
			}
			return new ObjectModel[1] { objectModel };
		}
		catch (ParquetException innerException)
		{
			throw new InvalidDataProvidedException("Unable to load Delta Lake data." + Environment.NewLine + "Delta Lake data is invalid.", innerException);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}
}

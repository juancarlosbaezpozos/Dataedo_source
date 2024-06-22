using System;
using Dataedo.App.Import.DataLake.Processing;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Import.DataLake;

public static class DataLakeImportFactory
{
	public static IDataLakeImport GetDataLakeImport(SharedObjectTypeEnum.ObjectType objectType, DataLakeTypeEnum.DataLakeType dataLakeType)
	{
		return dataLakeType switch
		{
			DataLakeTypeEnum.DataLakeType.JSON => new JsonImport(objectType), 
			DataLakeTypeEnum.DataLakeType.CSV => new CsvImport(objectType), 
			DataLakeTypeEnum.DataLakeType.AVRO => new AvroImport(objectType), 
			DataLakeTypeEnum.DataLakeType.PARQUET => new ParquetImport(objectType), 
			DataLakeTypeEnum.DataLakeType.ORC => new OrcImport(objectType), 
			DataLakeTypeEnum.DataLakeType.EXCEL_TABLE => new ExcelImport(objectType), 
			DataLakeTypeEnum.DataLakeType.XML => new XmlImport(objectType), 
			DataLakeTypeEnum.DataLakeType.DELTALAKE => new DeltaLakeImport(objectType), 
			_ => throw new NotSupportedException($"Type \"{dataLakeType}\" is not supported."), 
		};
	}

	public static IDataLakeImport GetDataLakeImport(DataLakeTypeEnum.DataLakeType dataLakeType)
	{
		return GetDataLakeImport(SharedObjectTypeEnum.ObjectType.Structure, dataLakeType);
	}
}

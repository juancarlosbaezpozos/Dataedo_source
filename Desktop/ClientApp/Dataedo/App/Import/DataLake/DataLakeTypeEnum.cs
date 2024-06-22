using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Dataedo.App.Properties;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Import.DataLake;

public class DataLakeTypeEnum
{
	public enum DataLakeType
	{
		JSON = 0,
		CSV = 1,
		XML = 2,
		AVRO = 3,
		PARQUET = 4,
		ORC = 5,
		DELTALAKE = 6,
		EXCEL_TABLE = 7
	}

	public static SharedObjectSubtypeEnum.ObjectSubtype GetObjectSubtype(SharedObjectTypeEnum.ObjectType objectType, DataLakeType dataLakeType)
	{
		switch (dataLakeType)
		{
		case DataLakeType.JSON:
			if (objectType != SharedObjectTypeEnum.ObjectType.Table)
			{
				return SharedObjectSubtypeEnum.ObjectSubtype.Json;
			}
			return SharedObjectSubtypeEnum.ObjectSubtype.Collection;
		case DataLakeType.CSV:
			if (objectType != SharedObjectTypeEnum.ObjectType.Table)
			{
				return SharedObjectSubtypeEnum.ObjectSubtype.Csv;
			}
			return SharedObjectSubtypeEnum.ObjectSubtype.Collection;
		case DataLakeType.AVRO:
			if (objectType != SharedObjectTypeEnum.ObjectType.Table)
			{
				return SharedObjectSubtypeEnum.ObjectSubtype.Structure;
			}
			return SharedObjectSubtypeEnum.ObjectSubtype.Collection;
		case DataLakeType.PARQUET:
			if (objectType != SharedObjectTypeEnum.ObjectType.Table)
			{
				return SharedObjectSubtypeEnum.ObjectSubtype.Parquet;
			}
			return SharedObjectSubtypeEnum.ObjectSubtype.Collection;
		case DataLakeType.XML:
			if (objectType != SharedObjectTypeEnum.ObjectType.Table)
			{
				return SharedObjectSubtypeEnum.ObjectSubtype.Xml;
			}
			return SharedObjectSubtypeEnum.ObjectSubtype.Collection;
		case DataLakeType.ORC:
			if (objectType != SharedObjectTypeEnum.ObjectType.Table)
			{
				return SharedObjectSubtypeEnum.ObjectSubtype.Orc;
			}
			return SharedObjectSubtypeEnum.ObjectSubtype.Collection;
		case DataLakeType.EXCEL_TABLE:
			if (objectType != SharedObjectTypeEnum.ObjectType.Table)
			{
				return SharedObjectSubtypeEnum.ObjectSubtype.ExcelTable;
			}
			return SharedObjectSubtypeEnum.ObjectSubtype.Collection;
		case DataLakeType.DELTALAKE:
			if (objectType != SharedObjectTypeEnum.ObjectType.Table)
			{
				return SharedObjectSubtypeEnum.ObjectSubtype.DeltaLake;
			}
			return SharedObjectSubtypeEnum.ObjectSubtype.Collection;
		default:
			return SharedObjectSubtypeEnum.ObjectSubtype.Object;
		}
	}

	public static string GetDisplayName(DataLakeType dataLakeType)
	{
		return dataLakeType switch
		{
			DataLakeType.JSON => "JSON", 
			DataLakeType.CSV => "CSV", 
			DataLakeType.XML => "XML", 
			DataLakeType.AVRO => "Apache Avro", 
			DataLakeType.PARQUET => "Apache Parquet", 
			DataLakeType.ORC => "Apache ORC", 
			DataLakeType.DELTALAKE => "Delta Lake", 
			DataLakeType.EXCEL_TABLE => "Microsoft Excel", 
			_ => throw new ArgumentException("dataLakeType"), 
		};
	}

	public static Image GetImage(DataLakeType dataLakeType)
	{
		return dataLakeType switch
		{
			DataLakeType.JSON => Resources.json_color_16, 
			DataLakeType.CSV => Resources.csv_color_16, 
			DataLakeType.XML => Resources.xml_color_16, 
			DataLakeType.AVRO => Resources.avro_color_16, 
			DataLakeType.PARQUET => Resources.parquet_color_16, 
			DataLakeType.ORC => Resources.orc_color_16, 
			DataLakeType.DELTALAKE => Resources.delta_color_16, 
			DataLakeType.EXCEL_TABLE => Resources.excel_color_16, 
			_ => throw new ArgumentException("dataLakeType"), 
		};
	}

	public static bool IsVisibleInPasteMode(DataLakeType dataLakeType)
	{
		return dataLakeType switch
		{
			DataLakeType.JSON => true, 
			DataLakeType.CSV => true, 
			DataLakeType.XML => true, 
			DataLakeType.AVRO => true, 
			DataLakeType.PARQUET => false, 
			DataLakeType.ORC => false, 
			DataLakeType.DELTALAKE => false, 
			DataLakeType.EXCEL_TABLE => false, 
			_ => throw new ArgumentException("dataLakeType"), 
		};
	}

	public static string GetTypeFilesFilter(DataLakeType dataLakeType)
	{
		return dataLakeType switch
		{
			DataLakeType.JSON => GetDisplayName(dataLakeType) + " (*.json)|*.json", 
			DataLakeType.CSV => GetDisplayName(dataLakeType) + " (*.csv)|*.csv;*.txt", 
			DataLakeType.XML => GetDisplayName(dataLakeType) + " (*.xml)|*.xml", 
			DataLakeType.AVRO => GetDisplayName(dataLakeType) + " (*.avro, *.avsc)|*.avro;*.avsc", 
			DataLakeType.PARQUET => GetDisplayName(dataLakeType) + " (*.parquet)|*.parquet", 
			DataLakeType.ORC => GetDisplayName(dataLakeType) + " (*.orc)|*.orc", 
			DataLakeType.DELTALAKE => GetDisplayName(dataLakeType) + " (*.parquet)|*.parquet", 
			DataLakeType.EXCEL_TABLE => GetDisplayName(dataLakeType) + " (*.xlsx)|*.xlsx", 
			_ => throw new ArgumentException("dataLakeType"), 
		};
	}

	public static DataLakeType[] GetDataLakeTypes()
	{
		return (DataLakeType[])Enum.GetValues(typeof(DataLakeType));
	}

	public static DataLakeTypeObject[] GetDataLakeTypeObjects()
	{
		return (from x in GetDataLakeTypes()
			select new DataLakeTypeObject(x) into x
			orderby x.DisplayName
			select x).ToArray();
	}

	public static DataLakeTypeObject[] GetDataLakePasteTypeObjects()
	{
		return (from x in GetDataLakeTypes()
			where IsVisibleInPasteMode(x)
			select new DataLakeTypeObject(x) into x
			orderby x.DisplayName
			select x).ToArray();
	}

	public static string GetConnectorLicenseString(DataLakeType value)
	{
		return value switch
		{
			DataLakeType.CSV => "CONNECTOR_CSV", 
			DataLakeType.JSON => "CONNECTOR_JSON", 
			DataLakeType.XML => "CONNECTOR_XML", 
			DataLakeType.AVRO => "CONNECTOR_AVRO", 
			DataLakeType.PARQUET => "CONNECTOR_PARQUET", 
			DataLakeType.ORC => "CONNECTOR_ORC", 
			DataLakeType.EXCEL_TABLE => "CONNECTOR_EXCEL", 
			DataLakeType.DELTALAKE => "CONNECTOR_DELTALAKE", 
			_ => throw new ArgumentException($"Provided connector type ({value}) is not supported."), 
		};
	}

	public static string TypeToString(DataLakeType dataLakeType)
	{
		return dataLakeType switch
		{
			DataLakeType.JSON => "JSON", 
			DataLakeType.CSV => "CSV", 
			DataLakeType.XML => "XML", 
			DataLakeType.AVRO => "AVRO", 
			DataLakeType.PARQUET => "PARQUET", 
			DataLakeType.ORC => "ORC", 
			DataLakeType.DELTALAKE => "DELTALAKE", 
			DataLakeType.EXCEL_TABLE => "EXCEL_TABLE", 
			_ => throw new ArgumentException("dataLakeType"), 
		};
	}

	public static DataLakeType StringToType(string dataLakeType)
	{
		return dataLakeType switch
		{
			"JSON" => DataLakeType.JSON, 
			"CSV" => DataLakeType.CSV, 
			"XML" => DataLakeType.XML, 
			"AVRO" => DataLakeType.AVRO, 
			"PARQUET" => DataLakeType.PARQUET, 
			"ORC" => DataLakeType.ORC, 
			"DELTALAKE" => DataLakeType.DELTALAKE, 
			"EXCEL_TABLE" => DataLakeType.EXCEL_TABLE, 
			_ => throw new ArgumentException("dataLakeType"), 
		};
	}

	public static List<SharedImportFolderEnum.ImportFolder> GetImportFolders(DataLakeType dataLakeType)
	{
		if ((uint)dataLakeType <= 7u)
		{
			return new List<SharedImportFolderEnum.ImportFolder> { SharedImportFolderEnum.ImportFolder.Files };
		}
		throw new ArgumentException("dataLakeType");
	}
}

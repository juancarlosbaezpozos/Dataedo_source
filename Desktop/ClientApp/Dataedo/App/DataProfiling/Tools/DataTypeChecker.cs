using System;
using System.Collections.Generic;
using System.Globalization;
using Dataedo.DataSources.Enums;

namespace Dataedo.App.DataProfiling.Tools;

public static class DataTypeChecker
{
	private const string BFILE = "BFILE";

	private const string BYTEINT = "BYTEINT";

	private const string CIDR = "CIDR";

	private const string ID = "ID";

	private const string TID = "TID";

	private const string INET = "INET";

	private const string JSON = "JSON";

	private const string ROWID = "ROWID";

	private const string SQLVARIANT = "SQL_VARIANT";

	private const string UNIQUEIDENTIFIER = "UNIQUEIDENTIFIER";

	private const string UROWID = "UROWID";

	private const string UUID = "UUID";

	private const string VARIANT = "VARIANT";

	private const string XML = "XML";

	private const string XMLTYPE = "XMLTYPE";

	private const string Long = "LONG";

	private const string LongRaw = "LONG RAW";

	private const string LongVarchar = "LONG VARCHAR";

	private const string HierarchyId = "HIERARCHYID";

	private const string NText = "NTEXT";

	private const string Other = "OTHER";

	public const string Text = "TEXT";

	private const string GEOGRAPHY = "GEOGRAPHY";

	private const string GEOMETRY = "GEOMETRY";

	private const string GEOMETRYCOLLECTION = "GEOMETRYCOLLECTION";

	private const string LINESTRING = "LINESTRING";

	private const string MULTILINESTRING = "MULTILINESTRING";

	private const string MULTIPOINT = "MULTIPOINT";

	private const string MULTIPOLYGON = "MULTIPOLYGON";

	private const string POINT = "POINT";

	private const string POLYGON = "POLYGON";

	private const string Spatial = "SPATIAL";

	private const string BINARY = "BINARY";

	private const string BLOB = "BLOB";

	private const string BYTEA = "BYTEA";

	private const string GRAPHIC = "GRAPHIC";

	private const string IMAGE = "IMAGE";

	private const string LONGBLOB = "LONGBLOB";

	private const string MEDIUMBLOB = "MEDIUMBLOB";

	private const string TINYBLOB = "TINYBLOB";

	private const string VARBINARY = "VARBINARY";

	private const string VARGRAPHIC = "VARGRAPHIC";

	private const string Bit = "BIT";

	private const string Bool = "BOOL";

	private const string Boolean = "BOOLEAN";

	private const string Bigint = "BIGINT";

	private const string Counter = "COUNTER";

	public const string Int = "INT";

	private const string Int32 = "INT32";

	private const string Int64 = "INT64";

	private const string Int8 = "INT8";

	private const string Integer = "INTEGER";

	private const string MediumInt = "MEDIUMINT";

	private const string Serial = "SERIAL";

	private const string Serial8 = "SERIAL8";

	private const string Smallint = "SMALLINT";

	private const string SmallInt = "SMALLINT";

	private const string TinyInt = "TINYINT";

	private const string UnsignedInt = "UNSIGNED INT";

	private const string UnsignedMediumInt = "UNSIGNED MEDIUMINT";

	private const string UnsignedSmallint = "UNSIGNED SMALLINT";

	private const string UnsignedTinyint = "UNSIGNED TINYINT";

	private const string VarInt = "VARINT";

	private const string Varint = "VARINT";

	private const string BinaryDouble = "BINARY_DOUBLE";

	private const string BinaryFloat = "BINARY_FLOAT";

	private const string Dec = "DEC";

	private const string Decfloat = "DECFLOAT";

	private const string Decimal = "DECIMAL";

	private const string Decimal128 = "DECIMAL128";

	private const string Double = "DOUBLE";

	private const string DoublePrecision = "DOUBLE PRECISION";

	private const string Float = "FLOAT";

	private const string Float64 = "FLOAT64";

	private const string Money = "MONEY";

	private const string Number = "NUMBER";

	private const string Numeric = "NUMERIC";

	public const string Real = "REAL";

	private const string SmallFloat = "SMALLFLOAT";

	private const string SmallMoney = "SMALLMONEY";

	private const string TTDecimal = "TT_DECIMAL";

	private const string UnsignedDouble = "UNSIGNED DOUBLE";

	private const string Char = "CHAR";

	private const string Character = "CHARACTER";

	private const string NVarchar = "NVARCHAR";

	private const string NVarchar2 = "NVARCHAR2";

	private const string Varchar = "VARCHAR";

	private const string Varchar2 = "VARCHAR2";

	private const string Ascii = "ASCII";

	private const string CharVarying = "CHAR VARYING";

	private const string CharacterSet = "CHARACTER SET";

	private const string CharacterVarying = "CHARACTER VARYING";

	private const string Charset = "CHARSET";

	private const string Clob = "CLOB";

	private const string Dbclob = "DBCLOB";

	private const string Enum = "ENUM";

	private const string Longtext = "LONGTEXT";

	private const string Mediumblob = "MEDIUMBLOB";

	private const string Mediumtext = "MEDIUMTEXT";

	private const string NationalChar = "NATIONAL CHAR";

	private const string NationalCharVarying = "NATIONAL CHAR VARYING";

	private const string NationalCharacter = "NATIONAL CHARACTER";

	private const string NationalCharacterVarying = "NATIONAL CHARACTER VARYING";

	private const string Nchar = "NCHAR";

	private const string NcharVarying = "NCHAR VARYING";

	private const string Nclob = "NCLOB";

	private const string Nvarchar2 = "NVARCHAR2";

	private const string Raw = "RAW";

	private const string Set = "SET";

	public const string STRING = "STRING";

	private const string TinyText = "TINYTEXT";

	public const string STRING_C = "STRING_C";

	public const string Date = "DATE";

	private const string Date2 = "DATE2";

	private const string Seconddate = "SECONDDATE";

	private const string Datetimeoffset = "DATETIMEOFFSET";

	private const string D_OFFSET = "D_OFFSET";

	public const string Time = "TIME";

	public const string Datetime = "DATETIME";

	private const string Datetime2 = "DATETIME2";

	private const string Interval = "INTERVAL";

	private const string Intervalday = "INTERVAL DAY";

	private const string Intervalyear = "INTERVAL YEAR";

	private const string Smalldatetime = "SMALLDATETIME";

	public const string Timestamp = "TIMESTAMP";

	private const string Timeuuid = "TIMEUUID";

	private const string Year = "YEAR";

	private static List<(string, Func<string, bool>)> ProfilingDataTypes = new List<(string, Func<string, bool>)>
	{
		("INT", IsINTType),
		("REAL", IsRealType),
		("STRING", IsStringType),
		("STRING_C", IsStringCoreType),
		("DATETIME", IsDateTimeType),
		("DATE", IsDateType),
		("OTHER", IsOtherType),
		("SPATIAL", IsSpatialType),
		("BINARY", IsBinaryType),
		("BOOL", IsBoolType)
	};

	public static bool IsINTType(string datatype)
	{
		datatype = datatype?.ToUpper();
		if (!(datatype == "BIGINT") && !(datatype == "COUNTER") && !(datatype == "INT") && !(datatype == "INT32") && !(datatype == "INT64") && !(datatype == "INT8") && !(datatype == "INTEGER") && !(datatype == "MEDIUMINT") && !(datatype == "SERIAL") && !(datatype == "SERIAL8") && !(datatype == "SMALLINT"))
		{
			switch (datatype)
			{
			default:
				return datatype == "VARINT";
			case "SMALLINT":
			case "TINYINT":
			case "UNSIGNED INT":
			case "UNSIGNED MEDIUMINT":
			case "UNSIGNED SMALLINT":
			case "UNSIGNED TINYINT":
			case "VARINT":
				break;
			}
		}
		return true;
	}

	public static bool IsRealType(string datatype)
	{
		datatype = datatype?.ToUpper();
		switch (datatype)
		{
		default:
			return datatype == "UNSIGNED DOUBLE";
		case "BINARY_DOUBLE":
		case "BINARY_FLOAT":
		case "DEC":
		case "DECFLOAT":
		case "DECIMAL":
		case "DECIMAL128":
		case "DOUBLE":
		case "DOUBLE PRECISION":
		case "FLOAT":
		case "FLOAT64":
		case "MONEY":
		case "NUMBER":
		case "NUMERIC":
		case "REAL":
		case "SMALLFLOAT":
		case "SMALLMONEY":
		case "TT_DECIMAL":
			return true;
		}
	}

	public static bool IsStringType(string datatype)
	{
		datatype = datatype?.ToUpper();
		if (!(datatype == "NVARCHAR") && !(datatype == "VARCHAR") && !(datatype == "CHAR") && !(datatype == "VARCHAR2") && !(datatype == "NVARCHAR2"))
		{
			switch (datatype)
			{
			default:
				return datatype == "TINYTEXT";
			case "CHARACTER":
			case "ASCII":
			case "CHAR VARYING":
			case "CHARACTER SET":
			case "CHARACTER VARYING":
			case "CHARSET":
			case "ENUM":
			case "MEDIUMBLOB":
			case "MEDIUMTEXT":
			case "NATIONAL CHAR":
			case "NATIONAL CHAR VARYING":
			case "NATIONAL CHARACTER":
			case "NATIONAL CHARACTER VARYING":
			case "NCHAR":
			case "NCHAR VARYING":
			case "NVARCHAR2":
			case "RAW":
			case "SET":
			case "STRING":
				break;
			}
		}
		return true;
	}

	public static bool IsStringCoreType(string datatype)
	{
		datatype = datatype?.ToUpper();
		switch (datatype)
		{
		default:
			return datatype == "STRING_C";
		case "CLOB":
		case "DBCLOB":
		case "LONGTEXT":
		case "NCLOB":
			return true;
		}
	}

	public static bool IsTypeThatShouldNotFail(string datatype)
	{
		datatype = datatype?.ToUpper();
		switch (datatype)
		{
		default:
			return datatype == "TIME";
		case "TIMESTAMP":
		case "INTERVAL DAY":
		case "INTERVAL YEAR":
		case "SMALLDATETIME":
		case "TIMEUUID":
		case "YEAR":
		case "INTERVAL":
		case "DATETIMEOFFSET":
		case "D_OFFSET":
			return true;
		}
	}

	public static bool IsDateTimeType(string datatype)
	{
		datatype = datatype?.ToUpper();
		if (!(datatype == "DATETIME"))
		{
			switch (datatype)
			{
			default:
				return datatype == "TIME";
			case "DATETIME":
			case "DATETIME2":
			case "TIMESTAMP":
			case "INTERVAL DAY":
			case "INTERVAL YEAR":
			case "SMALLDATETIME":
			case "TIMEUUID":
			case "YEAR":
			case "INTERVAL":
			case "DATETIMEOFFSET":
			case "D_OFFSET":
				break;
			}
		}
		return true;
	}

	public static bool IsDateType(string datatype)
	{
		datatype = datatype?.ToUpper();
		if (!(datatype == "DATE") && !(datatype == "DATE2"))
		{
			return datatype == "SECONDDATE";
		}
		return true;
	}

	public static bool IsOtherType(string datatype)
	{
		datatype = datatype?.ToUpper();
		switch (datatype)
		{
		default:
			return datatype == "TEXT";
		case "BFILE":
		case "BYTEINT":
		case "CIDR":
		case "ID":
		case "TID":
		case "INET":
		case "JSON":
		case "ROWID":
		case "SQL_VARIANT":
		case "UNIQUEIDENTIFIER":
		case "UROWID":
		case "UUID":
		case "VARIANT":
		case "XML":
		case "XMLTYPE":
		case "LONG":
		case "LONG RAW":
		case "LONG VARCHAR":
		case "HIERARCHYID":
		case "NTEXT":
		case "OTHER":
			return true;
		}
	}

	public static bool IsSpatialType(string datatype)
	{
		datatype = datatype?.ToUpper();
		switch (datatype)
		{
		default:
			return datatype == "SPATIAL";
		case "GEOGRAPHY":
		case "GEOMETRY":
		case "GEOMETRYCOLLECTION":
		case "LINESTRING":
		case "MULTILINESTRING":
		case "MULTIPOINT":
		case "MULTIPOLYGON":
		case "POINT":
		case "POLYGON":
			return true;
		}
	}

	public static bool IsBinaryType(string datatype)
	{
		datatype = datatype?.ToUpper();
		switch (datatype)
		{
		default:
			return datatype == "VARGRAPHIC";
		case "BINARY":
		case "BLOB":
		case "BYTEA":
		case "GRAPHIC":
		case "IMAGE":
		case "LONGBLOB":
		case "MEDIUMBLOB":
		case "TINYBLOB":
		case "VARBINARY":
			return true;
		}
	}

	public static bool IsBoolType(string datatype)
	{
		datatype = datatype?.ToUpper();
		if (!(datatype == "BIT") && !(datatype == "BOOL"))
		{
			return datatype == "BOOLEAN";
		}
		return true;
	}

	public static bool TypeIsNotSupportedForProfiling(string datatype, ProfilingDatabaseTypeEnum.ProfilingDatabaseType? databaseTypeEnum)
	{
		if (datatype?.ToUpper(CultureInfo.InvariantCulture) == "TEXT" && databaseTypeEnum.HasValue && databaseTypeEnum.Value == ProfilingDatabaseTypeEnum.ProfilingDatabaseType.Snowflake)
		{
			return false;
		}
		if (!IsBinaryType(datatype) && !IsSpatialType(datatype) && !IsOtherType(datatype))
		{
			return IsStringCoreType(datatype);
		}
		return true;
	}

	public static string GetProfilingDataType(string datatype, ProfilingDatabaseTypeEnum.ProfilingDatabaseType databaseTypeEnum)
	{
		bool num = datatype?.ToUpper(CultureInfo.InvariantCulture) == "TIMESTAMP" && databaseTypeEnum == ProfilingDatabaseTypeEnum.ProfilingDatabaseType.SqlServer;
		bool flag = datatype?.ToUpper(CultureInfo.InvariantCulture) == "TEXT" && databaseTypeEnum == ProfilingDatabaseTypeEnum.ProfilingDatabaseType.Snowflake;
		if (num)
		{
			return "OTHER";
		}
		if (flag)
		{
			return "STRING";
		}
		foreach (var profilingDataType in ProfilingDataTypes)
		{
			if (profilingDataType.Item2(datatype))
			{
				return profilingDataType.Item1;
			}
		}
		return null;
	}
}

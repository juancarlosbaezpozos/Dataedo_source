using ProtoBuf;

namespace Dataedo.App.Import.DataLake.Processing.ORCSupport.Protocol;

[ProtoContract]
public enum ColumnTypeKind
{
	Boolean = 0,
	Byte = 1,
	Short = 2,
	Int = 3,
	Long = 4,
	Float = 5,
	Double = 6,
	String = 7,
	Binary = 8,
	Timestamp = 9,
	List = 10,
	Map = 11,
	Struct = 12,
	Union = 13,
	Decimal = 14,
	Date = 15,
	Varchar = 16,
	Char = 17
}

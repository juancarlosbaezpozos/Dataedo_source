using Dataedo.Shared.Enums;

namespace Dataedo.App.DataProfiling.Models;

public interface IColumnInfoForProfiling
{
	string TableName { get; }

	string TableSchema { get; }

	SharedObjectTypeEnum.ObjectType ParentObjectType { get; }

	string Name { get; }

	string DataType { get; }

	int? ValuesListRowsCount { get; }

	string ValuesListMode { get; }
}

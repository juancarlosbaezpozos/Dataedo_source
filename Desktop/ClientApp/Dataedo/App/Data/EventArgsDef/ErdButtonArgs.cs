using System;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Data.EventArgsDef;

public class ErdButtonArgs : EventArgs
{
	public bool Visible { get; set; }

	public bool IsRelation { get; set; }

	public bool? IsUserRelation { get; set; }

	public bool IsTable { get; }

	public bool HasMultipleLevelColumns { get; set; }

	public SharedObjectTypeEnum.ObjectType? ObjectType { get; set; }

	public SharedDatabaseClassEnum.DatabaseClass? DatabaseClass { get; set; }

	public ErdButtonArgs(bool visible, bool isRelation, bool isTable, bool hasMultipleLevelColumns, SharedObjectTypeEnum.ObjectType? objectType, SharedDatabaseClassEnum.DatabaseClass? databaseClass, bool? isUserRelation = null)
	{
		Visible = visible;
		IsRelation = isRelation;
		IsUserRelation = isUserRelation;
		IsTable = isTable;
		HasMultipleLevelColumns = hasMultipleLevelColumns;
		ObjectType = objectType;
		DatabaseClass = databaseClass;
	}
}
